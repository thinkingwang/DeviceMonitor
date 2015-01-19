//采用jquery easyui loading css效果
function ajaxLoading() {
    $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: "100%", height: $(window).height() }).appendTo("body");
    $("<div class=\"datagrid-mask-msg\"></div>").html("正在处理，请稍候。。。").appendTo("body").css({ display: "block", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2 });
}
function ajaxLoadEnd() {
    $(".datagrid-mask").remove();
    $(".datagrid-mask-msg").remove();
}
$(function () {
    var toolbar = [{
        text: '刷新',
        iconCls: 'icon-reload',
        handler: function() {
            $('#deviceTree').tree('reload');
        }
    }];
    $('#deviceTree').treegrid({
        url: 'http://' + window.location.host + '/api/Tree?$orderby=index',
        method: 'GET',
        idField:'id',    
        treeField: 'name',
        columns: [[
            { title: '名称', field: 'name' },
            {
                title: '状态',
                field: 'Alarmed',
                formatter: function format(val, row) {
                    if (val == true) {
                        return "异常";
                    }
                    else if (val == false) {
                        return "正常";
                    }
                    return val;
                }
            }
        ]]
        ,
        onBeforeLoad: function (node, param) {
            if (node == undefined) {
                return;
            }
            param.nodeType = node.nodeType;
        },
        rowStyler: function (row) {
            if (row !== undefined && row !== null) {
                var parent = $('#deviceTree').treegrid("find", row.parentId);
                if (undefined != parent) {
                    var children = $('#deviceTree').treegrid("getChildren", row.parentId);
                    var flag = true;
                    $.each(children, function(n, value) {
                        if (value.Alarmed) {
                            flag = false;
                        }
                    });
                    if (flag) {
                        $('#deviceTree').treegrid('update', {
                            id: parent.id,
                            row: {
                                Alarmed: false
                            }
                        });
                    } else {

                    }
                };
                if (row.Alarmed) {
                    return 'background-color:pink;color:blue;font-weight:bold;';
                } 
            }
        },
        loadFilter: function (data) {
            if (data.length > 0) {
                $.each(data, function (index, device) {
                    if (data[0].nodeType == 2) {
                        device.state = 'open';
                    }
                    device.text = device.name;
                });
            }
            return data;
        },
        onSelect: function (node) {
            if (node.nodeType == 2) {
                addTab(node.id, node.text);
           }
        }
    });
    function cycleInternal(index) {
        return function() {
            cycle(index);
        }
    }

    function cycle(index) {
        //processing 
        if (!$("#cycle").is(':checked')) {
            return;
        }
        //要执行的代码
        var panels = $('#tabs').tabs('tabs');
        if (panels.length != 0) {
            index++;
            index = index % panels.length;
            addTab(null, index, '#');
        }
        setTimeout(cycleInternal(index), 2000);
    }

    $('#cycle').click(function() {
        //判断apple是否被选中
        if ($('#cycle').is(':checked')) {
            cycle(0);
        }

    });
    // Declare a proxy to reference the hub. 
    if ($.connection.ChatHub !== undefined) {
        var chat = $.connection.ChatHub;
        // Create a function that the hub can call to broadcast messages.
        chat.client.broadcastMessage = function (propertyId, name, position, value,alarmed) {
            //// set up the updating of the chart each second
            //var series = chart.highcharts().series[0];
            //var x = (new Date()).getTime(), // current time
            //    y = parseFloat(value);
            //series.addPoint([x, y], true, true);
            $.ajax({
                url: 'http://' + window.location.host + '/api/DeviceInfo',
                cache: false,
                type: 'GET',
                data: { id: propertyId},
                success: function (data) {
                    $('#deviceTree').treegrid('update', {
                        id: propertyId,
                        row: {
                            Alarmed: data.Alarmed
                        }
                    });
                },
                error: function (data) {

                }
            });

            $.ajax({
                url: 'http://' + window.location.host + '/api/DeviceInfo',
                type: 'Get',
                success: function (data) {
                    $('#deviceTree').treegrid('update', {
                        id: propertyId,
                        row: {
                            Alarmed: data.Alarmed
                        }
                    });
                },
                error: function (data) {

                }
            });
            $("#" + propertyId + name).datagrid("updateRow", {
                index: position,
                row: {
                    value: value,
                    alarmed:alarmed
                }
            });
        };
        chat.client.notifyDeviceState = function(id, message) {
            jNotify(message, {
                VerticalPosition: "bottom", // 垂直位置：top, center, bottom 
                ShowOverlay: false, // 是否显示遮罩层 
            });
            $('#deviceTree').treegrid('update', {
                id: id,
                row: {
                    Alarmed: "离线"
                }
            });
        }
        // Start the connection.
        $.connection.hub.start().done(function () { });
    }
});