$(function () {
    $('#dd').dialog({
        title: "拷贝已有节点生成新节点",
        model: true,
        width: 400,
        height: 200,
        iconCls:'icon-save',
        closed:true,
        buttons: [
            {
                text: '确定',
                iconCls: 'icon-ok',
                handler: function () {
                    dlgOk();
                }
            },
            {
                text: '取消',
                iconCls: 'icon-cancel',
                handler: function () {
                    $('#dd').dialog('close');
                }
            }
        ],
        toolbar: '#dlg-toolbar'
    });

    $('#selectTree').tree({
        url: 'http://' + window.location.host + '/api/Tree?$orderby=index',
        method: 'GET',
        fit: true,
        onBeforeLoad: function (node, param) {
            if (node != undefined) {
                param.url = node.url;
                param.nodeType = node.nodeType;
            }
        },
        loadFilter: function (data) {
            $.each(data, function (index, device) {
                if (device.name == null) {
                    device.text = '';
                } else {
                    device.text = device.name;
                }
            });
            return data;
        }
    });
    $.extend($.fn.datagrid.methods, {
        editCell: function (jq, param) {
            return jq.each(function () {
                var opts = $(this).datagrid('options');
                var fields = $(this).datagrid('getColumnFields', true).concat($(this).datagrid('getColumnFields'));
                for (var i = 0; i < fields.length; i++) {
                    var col = $(this).datagrid('getColumnOption', fields[i]);
                    col.editor1 = col.editor;
                    if (fields[i] != param.field) {
                        col.editor = null;
                    }
                }
                $(this).datagrid('beginEdit', param.index);
                for (var i = 0; i < fields.length; i++) {
                    var col = $(this).datagrid('getColumnOption', fields[i]);
                    col.editor = col.editor1;
                }
            });
        }
    });

    var editIndex = undefined;

    function endEditing() {
        if (editIndex == undefined) {
            return true;
        }
        if ($('#dg').datagrid('validateRow', editIndex)) {
            $('#dg').datagrid('endEdit', editIndex);
            editIndex = undefined;
            return true;
        } else {
            return false;
        }
    }

    function onClickCell(index, field) {
        if (endEditing()) {
            $('#dg').datagrid('selectRow', index)
                    .datagrid('editCell', { index: index, field: field });
            editIndex = index;
        }
    }

    $('#deviceTree').tree({
        url: 'http://' + window.location.host + '/api/Tree?$orderby=index',
        method: 'GET',
        onBeforeLoad: function (node, param) {
            if (node != undefined) {
                param.url = node.url;
                param.nodeType = node.nodeType;
            } else {
                param.nodeType = 1;
            }
        },
        loadFilter: function (data) {
            if (data.length > 0) {
                $.each(data, function (index, device) {
                    if (device.name == null) {
                        device.text = '';
                    } else {
                        device.text = device.name;
                    }
                });
            }
            return data;
        },
        onSelect: CreateDataGrid
    });

    var toolbar = [{
        text: '添加',
        iconCls: 'icon-add',
        handler: function () { append(); }
    }, {
        text: '添加已有',
        iconCls: 'icon-add',
        handler: function () { appendExist(); }
    }, {
        text: '删除',
        iconCls: 'icon-remove',
        handler: function () { removeit(); }
    }, '-', {
        text: '保存',
        iconCls: 'icon-save',
        handler: function () { accept(); }
    }, '-', {
        text: '撤销',
        iconCls: 'icon-undo',
        handler: function () { reject(); }
    }];


    //厂区表格列定义
    var factoryTableColumn = [
        [
            { field: 'ck', checkbox: true },
            { field: 'id', title: '编号', width: 100 },
            { field: 'name', title: '名称', editor: 'text', width: 100 },
            { field: 'index', title: '索引', editor: 'numberbox', width: 100, sortable: true },
            { field: 'refreshTime', title: '刷新周期', editor: 'numberbox', width: 100 },
            { field: 'ip', title: 'IP地址', editor: 'text', width: 100 },
            { field: 'timeOut', title: '超时时间', editor: 'numberbox', width: 100 }
        ]
    ];

    //设备表格列定义
    var deviceTableColumn = [
                            [
                                { field: 'ck', checkbox: true },
                                { field: 'id', title: '编号', width: 100 },
                                { field: 'name', title: '名称', editor: 'text', width: 100, sortable: true },
                                { field: 'index', title: '索引', editor: 'numberbox', width: 100, sortable: true },
                                { field: 'ip', title: 'IP地址', editor: 'text', width: 100 },
                                { field: 'port', title: '端口号', editor: 'numberbox', width: 100 },
                                { field: 'commandByte', title: '命令字', editor: 'numberbox', width: 100 },
                                { field: 'headerLength', title: '数据头长度', editor: 'numberbox', width: 100 },
                                { field: 'dataLength', title: '数据体长度', editor: 'numberbox', width: 100 }
                            ]
    ];

    //点表表格列定义
    var dataTableColumn = [
        [
            { field: 'ck', checkbox: true },
            { field: 'id', title: '编号', width: 100 },
            { field: 'name', title: '名称', editor: 'text', width: 100, sortable: true },
            { field: 'index', title: '索引', editor: 'numberbox', width: 100, sortable: true },
            { field: 'groupIndex', title: '组内索引', editor: 'numberbox', width: 100, sortable: true },
            { field: 'address', title: '地址', editor: 'numberbox', width: 100, sortable: true },
            { field: 'lengthOrIndex', title: '长度/位地址', editor: 'numberbox', width: 100 },
            { field: 'type', title: '数据类型', editor: 'numberbox', width: 100, sortable: true },
            {
                field: 'AlarmAble',
                title: '是否报警',
                editor: {
                    type: 'checkbox',
                    options: {
                        on: true,
                        off: false
                    }
                },
                width: 100,
                sortable: true
            },
            { field: 'upper', title: '上限', editor: 'numberbox', width: 100 },
            { field: 'lower', title: '下限', editor: 'numberbox', width: 100 },
            { field: 'unit', title: '单位', editor: 'text', width: 100 },
            { field: 'group', title: '组别', editor: 'text', width: 100, sortable: true }
        ]
    ];

    //格式化表格列定义
    var formatTableColumn = [
        [
            { field: 'ck', checkbox: true },
            { field: 'id', title: '编号', width: 100 },
            {
                field: 'changeFormat',
                title: '格式化类型',
                editor: {
                    type: 'combobox',
                    options: {
                        valueField: 'changeFormat',
                        textField: 'value',
                        data: [
                            {
                                changeFormat: '替换',
                                value: '替换'
                            }, {
                                changeFormat: '追加',
                                value: '追加'
                            }, {
                                changeFormat: '相乘',
                                value: '相乘'
                            }, {
                                changeFormat: '相除',
                                value: '相除'
                            }
                        ],
                        required: true
                    }
                },
                width: 100
            },
            { field: 'key', title: '键', editor: 'numberbox', width: 100 },
            { field: 'value', title: '值', editor: 'text', width: 100 }
        ]
    ];


    $('#dg').datagrid({
        fit: true,
        idField: 'id',
        rownumbers: true,
        autoRowHeight: false,
        fitColumns: true,
        checkbox: true,
        sortName: 'index',
        sortOrder: 'asc',
        toolbar: toolbar,
        pagination: true,
        onClickCell: onClickCell,
        onAfterEdit: onAfterEdit
    });

    //生成所点击树节点的详细数据表格信息
    function CreateDataGrid(node) {
        if (node == null) {
            return;
        }
        var type = $('#dg').datagrid('options').queryParams.nodeType;
        if (type==undefined||type != node.nodeType) {
            var columns = [];
            var title = '';
            var sortName = '';
            switch (node.nodeType) {
            case 0:
                title = '厂区列表';
                sortName = 'name';
                columns = factoryTableColumn;
                break;
            case 1:
                title = '设备列表';
                sortName = 'index';
                columns = deviceTableColumn;
                break;
            case 2:
                title = '点表';
                sortName = 'index';
                columns = dataTableColumn;
                break;
            case 3:
                title = '格式化表';
                sortName = 'key';
                columns = formatTableColumn;
                break;
            }
            $('#dg').datagrid({
                url: 'http://' + window.location.host + '/DeviceManager/GetData',
                method: 'POST',
                title: title,
                columns: columns,
                sortName: sortName,
                queryParams: {
                    id: node.id,
                    nodeType: node.nodeType
                }
            });
        } else {
            $('#dg').datagrid('load', {
                id: node.id,
                nodeType: node.nodeType
            });
        }
    }


    function onAfterEdit(index, row, changes) {
        var str = JSON.stringify(row, function (key, value) {
            if (key == 'changeFormat') {
                switch (value) {
                    case '替换':
                        return '0';
                    case '追加':
                        return '1';
                    case '相乘':
                        return '2';
                    case '相除':
                        return '3';
                    default:
                        return value;
                }
            }
            return value;
        });
        $.ajax({
            url: 'http://' + window.location.host + '/DeviceManager/Edit',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: str,
            success: function (data) {
            },
            error: function (data) {
            }
        });
    }

    
   
    var editIndexRow = undefined;
    function endEditingRow() {
        if (editIndexRow == undefined) {
            return true;
        }
        if ($('#dg').datagrid('validateRow', editIndexRow)) {
            $('#dg').datagrid('endEdit', editIndexRow);
            editIndexRow = undefined;
            return true;
        } else {
            return false;
        }
    }

    function append() {
        // Initialize the view-model
        var treeNode = $('#deviceTree').tree('getSelected');
        if (treeNode == null) {
            return;
        }

        $.ajax({
            url: 'http://' + window.location.host + '/DeviceManager/GetInstance',
            cache: false,
            type: 'GET',
            data: { id: treeNode.id, nodeType: treeNode.nodeType },
            success: function(data) {
                $('#dg').datagrid('appendRow', data);
                editIndexRow = $('#dg').datagrid('getRows').length - 1;
                $('#dg').datagrid('beginEdit', editIndexRow);
            },
            error: function(data) {

            }
        });
    }

    function appendExist() {
        $('#dd').dialog('open');
    }

    function dlgOk() {
        var treeNode = $('#selectTree').tree('getSelected');
        var treeNodeParent = $('#deviceTree').tree('getSelected');
        if (treeNodeParent == null) {
            alert("请选中要拷贝到的位置");
            return;
        }
        if (treeNode == null) {
            alert("请选中要拷贝的节点");
            return;
        }
        if (treeNode.nodeType - treeNodeParent.nodeType != 1) {
            alert("当前节点下不允许创建该类型节点，请重新选择");
            return;
        }
        $.ajax({
            url: 'http://' + window.location.host + '/DeviceManager/CopyObject',
            cache: false,
            type: 'POST',
            data: { parentId:treeNodeParent.id,id: treeNode.id, nodeType: treeNode.nodeType },
            success: function (data) {
                $('#dg').datagrid('appendRow', data);
                $('#dg').datagrid('clearChecked');
                $('#deviceTree').tree('reload', treeNodeParent.target);
            },
            error: function (data) {

            }
        });
        $('#dd').dialog('close');
    }
    function removeit() {
        $.messager.confirm('确认', '是否真的删除?', function(r) {
            if (r) {
                var rows = $('#dg').datagrid('getChecked');
                $.each(rows, function (index, item) {
                    $.ajax({
                        url: 'http://' + window.location.host + '/DeviceManager/Delete',
                        data: { id: item.id, nodeType: item.nodeType },
                        cache: false,
                        async: false,
                        type: 'POST'
                    });
                });
                $('#dg').datagrid('clearChecked');
                var treeNode = $('#deviceTree').tree('getSelected');
                if (treeNode == null) {
                    return;
                }
                $('#deviceTree').tree('reload', treeNode.target);
                CreateDataGrid(treeNode);
            }
        });
    }

    function accept() {
        if (endEditingRow()) {
            $('#dg').datagrid('acceptChanges');
            var treeNode = $('#deviceTree').tree('getSelected');
            if (treeNode == null) {
                return;
            }
            $('#deviceTree').tree('reload', treeNode.target);
        }
    }
    function reject() {
        $('#dg').datagrid('rejectChanges');
        editIndexRow = undefined;
    }

   

});