


function addTab(id, subtitle, url) {
    var tab = $('#tabs').tabs('getSelected');  // 获取选择的面板
    $('#tabs').tabs('update', {
        tab: tab,
        options: {
            title: subtitle
        }
    });

    $('#mainContent').empty();
        $.ajax({
            url: 'http://' + window.location.host + '/api/DeviceInfo/' + id + '/group',
            type: 'get',
            success: function(devices) {
                $.each(devices, function(position, group) {
                    var title = group;
                    if (group == null) {
                        title = '一般参数';
                    }
                    $('#mainContent').append(" <div id='" + id + title + "' title='" + title + "'  data-options='style:{&quot;float&quot;:&quot;left&quot;},width:&quot;" + 100 / devices.length + "%&quot;,fitColumns:&quot;true&quot;,height:&quot;100%&quot;'></div>");
                    $('#' + id + title).datagrid({
                        url: 'http://' + window.location.host + '/api/DeviceData/' + id + '/' + group,
                        method: 'get',
                        columns: mycolumns,
                        //showGroup: true,
                        rowStyler: function(i, row) {
                            if (row !== undefined && row !== null) {
                                if (row.Alarmed) {
                                    return 'background-color:pink;color:blue;font-weight:bold;';
                                }
                            }
                        }
                    });
                });
            }
        });
        tabClose();
        tabCloseEven();
    }

    var mycolumns = [
        [
            { field: 'name', title: '点名', width: 100, sortable: true },
            { field: 'value', title: '值/状态', width: 100, resizable: false,formatter:format }
        ]
    ];

    function format(val, row) {
        if (row.unit != null && row.unit != 'null'&&!isNaN(parseInt(val,10))) {
            return val + row.unit;
        }
        return val;
    }
    function createFrame(url) {
        var s = '<iframe name="mainFrame" scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:100%;"></iframe>';
        return s;
    }

    function tabClose() {
        /*双击关闭TAB选项卡*/
        $(".tabs-inner").dblclick(function () {
            var subtitle = $(this).children("span").text();
            $('#tabs').tabs('close', subtitle);
        });

        $(".tabs-inner").bind('contextmenu', function (e) {
            $('#mm').menu('show', {
                left: e.pageX,
                top: e.pageY,
            });
            var subtitle = $(this).children("span").text();
            $('#mm').data("currtab", subtitle);
            return false;
        });
    }

    //绑定右键菜单事件
    function tabCloseEven() {
        //关闭当前
        $('#mm-tabclose').click(function () {
            var currtab_title = $('#mm').data("currtab");
            $('#tabs').tabs('close', currtab_title);
        });
        //全部关闭
        $('#mm-tabcloseall').click(function () {
            $('.tabs-inner span').each(function (i, n) {
                var t = $(n).text();
                $('#tabs').tabs('close', t);
            });
        });
        //关闭除当前之外的TAB
        $('#mm-tabcloseother').click(function () {
            var currtab_title = $('#mm').data("currtab");
            $('.tabs-inner span').each(function (i, n) {
                var t = $(n).text();
                if (t != currtab_title)
                    $('#tabs').tabs('close', t);
            });
        });

        //关闭当前右侧的TAB
        $('#mm-tabcloseright').click(function () {
            var nextall = $('.tabs-selected').nextAll();
            if (nextall.length == 0) {
                //msgShow('系统提示','后边没有啦~~','error');
                alert('后边没有啦~~');
                return false;
            }

            nextall.each(function (i, n) {
                var t = $('a:eq(0) span', $(n)).text();
                $('#tabs').tabs('close', t);
            });
            return false;
        });

        //关闭当前左侧的TAB
        $('#mm-tabcloseleft').click(function () {
            var prevall = $('.tabs-selected').prevAll();
            if (prevall.length == 0) {
                alert('到头了，前边没有啦~~');
                return false;
            }
            prevall.each(function (i, n) {
                var t = $('a:eq(0) span', $(n)).text();
                $('#tabs').tabs('close', t);
            });
            return false;
        });

        //退出
        $("#mm-exit").click(function () {
            $('#mm').menu('hide');

        });
    }

    
    //Highcharts.setOptions({
    //    global: {
    //        useUTC: false
    //    }
    //});

    //var chart = $('#container').highcharts({
    //    chart: {
    //        type: 'spline',
    //        animation: Highcharts.svg, // don't animate in old IE
    //        marginRight: 10,
    //        events: {
    //            load: function () {

    //                // set up the updating of the chart each second
    //                //var series = this.series[0];
    //                //setInterval(function () {
    //                //    chat.server.send("192.168.1.1", 1);
    //                //}, 1000);
    //            }
    //        }
    //    },
    //    title: {
    //        text: 'Live random data'
    //    },
    //    xAxis: {
    //        type: 'datetime',
    //        tickPixelInterval: 150
    //    },
    //    yAxis: {
    //        title: {
    //            text: 'Value'
    //        },
    //        plotLines: [
    //            {
    //                value: 0,
    //                width: 1,
    //                color: '#808080'
    //            }
    //        ]
    //    },
    //    tooltip: {
    //        formatter: function () {
    //            return '<b>' + this.series.name + '</b><br/>' +
    //                Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x) + '<br/>' +
    //                Highcharts.numberFormat(this.y, 2);
    //        }
    //    },
    //    legend: {
    //        enabled: false
    //    },
    //    exporting: {
    //        enabled: false
    //    },
    //    series: [
    //        {
    //            name: '设备实时数据',
    //            data: (function () {
    //                // generate an array of random data
    //                var data = [],
    //                    time = (new Date()).getTime(),
    //                    i;

    //                for (i = -19; i <= 0; i++) {
    //                    data.push({
    //                        x: time + i * 1000,
    //                        y: Math.random()
    //                    });
    //                }
    //                return data;
    //            })()
    //        }
    //    ]
    //});
