using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using DeviceMonitor.Controllers;
using DeviceMonitor.Dao;
using DeviceMonitor.Models.DeviceModels;
using DeviceMonitor.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;

namespace DeviceMonitor.HUB
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        private static readonly Dictionary<Guid, Timer> Timers = new Dictionary<Guid, Timer>();
        private readonly Random _random;

        private static readonly Dictionary<Guid, Tuple<bool, Int16, UdpClient>> ReceiveClients =
            new Dictionary<Guid, Tuple<bool, Int16, UdpClient>>();

        private IPEndPoint _remoteQqep;
        public static ManualResetEvent AllDone = new ManualResetEvent(false);
        private static int TryTimes;

        public ChatHub()
        {
            try
            {
                _random = new Random();
                using (var context = new DeviceContext())
                {
                    foreach (var deviceGroup in context.DeviceGroups.Include("DeviceInfos"))
                    {
                        if (Timers.ContainsKey(deviceGroup.id))
                        {
                            continue;
                        }
                        Timers.Add(deviceGroup.id,
                            new Timer(CallBack, deviceGroup.id, deviceGroup.refreshTime, Timeout.Infinite));
                        foreach (var deviceInfo in deviceGroup.DeviceInfos)
                        {
                            if (ReceiveClients.ContainsKey(deviceInfo.id))
                            {
                                continue;
                            }
                            ReceiveClients.Add(deviceInfo.id,
                                new Tuple<bool, Int16, UdpClient>(true, 0,
                                    new UdpClient(new IPEndPoint(IPAddress.Parse(deviceGroup.ip), deviceInfo.port))));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ControllerBase.Log.Debug(e.Message);
            }

        }

        private void CallBack(object state)
        {
            try
            {
                var groupid = Guid.Parse(state.ToString());
                using (var context = new DeviceContext())
                {
                    var group = context.DeviceGroups.Find(groupid);
                    if (group == null)
                    {
                        return;
                    }
                    var timer = Timers[group.id];
                    if (timer == null)
                    {
                        return;
                    }
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    for (var i = 0; i < ReceiveClients.Count; i++)
                    {
                        var deviceInfo = context.DeviceInfos.Find(ReceiveClients.ElementAt(i).Key);
                        RefreshData(ReceiveClients.ElementAt(i).Key, deviceInfo.ip, deviceInfo.commandByte,
                            group.timeOut);
                    }
                    timer.Change(group.refreshTime, Timeout.Infinite);
                }
            }
            catch (Exception e)
            {

                ControllerBase.Log.Debug(e.Message);
            }
        }

        private void RefreshData(Guid id, string ip, byte commandByte, int timeOut)
        {
            if (!ReceiveClients[id].Item1)
            {
                return;
            }
            ReceiveClients[id] = new Tuple<bool, Int16, UdpClient>(false, Convert.ToInt16(ReceiveClients[id].Item2 + 1),
                ReceiveClients[id].Item3);
            var sn = BitConverter.GetBytes(ReceiveClients[id].Item2);
            var sendBytes = new List<byte> {0xEB, 0x90, 0x52, commandByte, 0x00, 0x00, 0x00, 0x00, 0x00, sn[0], sn[1]};
            byte result = 0;
            sendBytes.ForEach(m => result += m);
            var ngCheck = (byte) (0 - result);
            sendBytes.Add(ngCheck);
            ReceiveClients[id].Item3.Send(sendBytes.ToArray(), sendBytes.Count,
                new IPEndPoint(IPAddress.Parse(ip), 2000));
            ReceiveMethod(id, timeOut);
        }

        private void ReceiveMethod(Guid id, int timeOut)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;
            var task = new Task(() =>
            {
                using (var context = new DeviceContext())
                {
                    var recvbytes = ReceiveClients[id].Item3.Receive(ref _remoteQqep);
                    byte result = 0;
                    Array.ForEach(recvbytes, m => result += m);
                    if (0 != result)
                    {
                        return;
                    }
                    var deviceInfo =
                        context.DeviceInfos.Include("DeviceDatas")
                            .Include("DeviceGroup")
                            .FirstOrDefault(m => m.id.Equals(id));
                    if (deviceInfo == null || BitConverter.ToInt16(recvbytes, 5) != deviceInfo.dataLength)
                    {
                        ReceiveClients[id] = new Tuple<bool, Int16, UdpClient>(true,
                            ReceiveClients[id].Item2, ReceiveClients[id].Item3);
                        return;
                    }
                    if (BitConverter.ToInt16(recvbytes, 9) != ReceiveClients[id].Item2)
                    {
                        TryTimes++;
                        if (TryTimes >= 20)
                        {
                            TryTimes = 0;
                            ReceiveClients[id] = new Tuple<bool, Int16, UdpClient>(true,
                                ReceiveClients[id].Item2, ReceiveClients[id].Item3);
                        }
                        else
                        {
                            ReceiveMethod(deviceInfo.id, deviceInfo.DeviceGroup.timeOut);
                        }
                        return;
                    }
                    ReceiveClients[id] = new Tuple<bool, Int16, UdpClient>(true,
                        ReceiveClients[id].Item2, ReceiveClients[id].Item3);

                    foreach (var deviceData in deviceInfo.DeviceDatas)
                    {
                        var realValue = 0;
                        if (deviceData.type == DataType.DeviceReal)
                        {

                            if (deviceData.name.Contains("温"))
                            {
                                var data = recvbytes[deviceInfo.headerLength + deviceData.address + 1];
                                switch (data)
                                {
                                    case 0:
                                        realValue = recvbytes[deviceInfo.headerLength + deviceData.address]/2;
                                        break;
                                    case 0xFF:
                                        realValue = (recvbytes[deviceInfo.headerLength + deviceData.address] - 256)/
                                                    2;
                                        break;
                                    case 0x6C:
                                        realValue = BitConverter.ToInt16(recvbytes,
                                            deviceInfo.headerLength + deviceData.address);
                                        break;
                                }
                            }
                            else
                            {
                                if (deviceData.lengthOrIndex == 2)
                                {
                                    realValue = BitConverter.ToInt16(recvbytes,
                                        deviceInfo.headerLength + deviceData.address);
                                }
                                else
                                {
                                    realValue = recvbytes[deviceInfo.headerLength + deviceData.address];
                                }
                            }
                        }
                        else
                        {
                            realValue = (recvbytes[deviceInfo.headerLength + deviceData.address] &
                                         (1 << deviceData.lengthOrIndex)) >> deviceData.lengthOrIndex;
                        }
                        if (deviceData.value.Equals(realValue))
                        {
                            continue;
                        }
                        deviceData.value = realValue;
                        context.SaveChanges();
                        var device = DataViewModel.GetDeviceData(deviceData);
                        Clients.All.broadcastMessage(deviceInfo.id, deviceData.@group ?? "一般参数",
                            device.groupIndex, device.value, device.Alarmed);
                        var requestJson =
                            JsonConvert.SerializeObject(new DataDto()
                            {
                                name = deviceData.@group ?? "一般参数",
                                index = device.groupIndex,
                                value = device.value,
                                Alarmed = device.Alarmed
                            });

                        HttpContent httpContent = new StringContent(requestJson);
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        var httpClient = new HttpClient();
                        httpClient.PutAsync("http://192.192.1.119:81/api/DeviceData?id=" + deviceInfo.id, httpContent);

                    }
                }

            }, token);
            token.Register(() =>
            {
                if (!task.IsCompleted)
                {
                    using (var context = new DeviceContext())
                    {
                        var deviceInfo = context.DeviceInfos.Find(id);
                        Clients.All.notifyDeviceState(deviceInfo.id,"通讯超时，设备:“" + deviceInfo.name + "”已离线");
                        //ControllerBase.Log.Warn("通讯超时，设备:“" + deviceInfo.name + "”已离线");
                    }
                }
                ReceiveClients[id] = new Tuple<bool, Int16, UdpClient>(true,
                    ReceiveClients[id].Item2, ReceiveClients[id].Item3);
            });
            task.Start();
            source.CancelAfter(timeOut);
        }

        public void Send(string ip, int addr)
        {
            Clients.All.broadcastMessage(ip, addr);
        }
    }
}