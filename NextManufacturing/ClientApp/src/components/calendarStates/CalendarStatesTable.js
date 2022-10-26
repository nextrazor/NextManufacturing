import {Form, Input, InputNumber, Popconfirm, Table, Typography, Modal, Button, message} from 'antd';
import React, {useState} from 'react';
import { useTranslation, Trans } from 'react-i18next';

const EditableCell = ({
                          editing,
                          dataIndex,
                          title,
                          inputType,
                          record,
                          index,
                          children,
                          ...restProps
                      }) => {
    const inputNode = inputType === 'number' ? <InputNumber/> : <Input/>;
    return (
        <td {...restProps}>
            {editing ? (
                <Form.Item
                    name={dataIndex}
                    style={{
                        margin: 0,
                    }}
                >
                    {inputNode}
                </Form.Item>
            ) : (
                children
            )}
        </td>
    );
};

const CalendarStatesTable = (originData) => {
    const [form] = Form.useForm();
    const [modalForm] = Form.useForm();
    const [data, setData] = useState(originData.originData);
    const [editingKey, setEditingKey] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const isEditing = (record) => record.key === editingKey;
    const { t } = useTranslation('calendarStatesTable');

    const showModal = () => {
        setIsModalOpen(true);
    };

    const logErrorFromResponse = (response) => {
        response.json().then(parsedResponse => {
            console.log(parsedResponse);
            error(t('messages.error') + parsedResponse.error);
        })
    }

    const handleOk = async () => {
        const row = await modalForm.validateFields();
        if (row.name && row.name != undefined && row.name != '') {
            fetch(`https://localhost:7167/CalendarStates/CreateCalendarState/${row.name}`, {method: 'POST'}).then(response => {
                if (response.ok) {
                    response.json().then(parsedResponse => {
                        parsedResponse.key = parsedResponse.guid;
                        const newData = [...data];
                        newData.push(parsedResponse);
                        setData(newData);
                        modalForm.resetFields();
                        setIsModalOpen(false);
                        success(t('messages.addSuccess'));
                    });
                } else
                    logErrorFromResponse(response);
            });
        } else
            error(t('messages.noNameError'));
    };

    const handleCancel = async () => {
        modalForm.resetFields();
        setIsModalOpen(false);
    };

    const edit = (record) => {
        form.setFieldsValue({
            name: '',
            age: '',
            address: '',
            ...record,
        });
        setEditingKey(record.key);
    };

    const cancel = () => {
        setEditingKey('');
    };

    const deleteItem = async (key) => {
        try {
            const newData = [...data];
            const index = newData.findIndex((item) => key === item.key);

            if (index > -1) {
                fetch(`https://localhost:7167/CalendarStates/DeleteCalendarState/${key}`, {method: 'DELETE'}).then(response => {
                    if (response.ok) {
                        newData.splice(index, 1);
                        setData(newData);
                        success(t('messages.deleteSuccess'));
                    } else {
                        logErrorFromResponse(response);
                    }
                })
            } else {
                error(t('messages.noDeleteItem'));
            }
        } catch (errInfo) {
            console.log('Validate Failed:', errInfo);
        }
    };

    const save = async (key) => {
        try {
            const row = await form.validateFields();
            const newData = [...data];
            const index = newData.findIndex((item) => key === item.key);

            if (index > -1) {
                var response = await fetch(`https://localhost:7167/CalendarStates/UpdateCalendarState/${key}/${row.name}`, {method: 'POST'})

                if (response.ok) {
                    const item = newData[index];
                    newData.splice(index, 1, {...item, ...row});
                    setData(newData);
                    setEditingKey('');
                    success(t('messages.updateSuccess'));
                } else {
                    logErrorFromResponse(response);
                }
            } else {
                error(t('messages.noUpdateItem'));
            }
        } catch (errInfo) {
            error('Validate Failed: ' + errInfo);
        }
    };

    const columns = [
        {
            title: 'GUID',
            dataIndex: 'guid',
            width: '30%',
        },
        {
            title: 'Name',
            dataIndex: 'name',
            sorter: (a, b) => a.name.length - b.name.length,
            editable: true,
        },
        {
            title: t('edit'),
            dataIndex: 'operation',
            render: (_, record) => {
                const editable = isEditing(record);
                return editable ? (
                    <span>
            <Typography.Link
                onClick={() => save(record.guid)}
                style={{
                    marginRight: 8,
                }}
            >
              Save
            </Typography.Link>
            <Popconfirm title={t('confirmCancel')} onConfirm={cancel}>
              <a>Cancel</a>
            </Popconfirm>
          </span>
                ) : (
                    <span>
          <Typography.Link disabled={editingKey !== ''} onClick={() => edit(record)}>
            {t('edit')}
          </Typography.Link>
        </span>
                );
            },
        },
        {
            title: t('delete'),
            dataIndex: 'operation2',
            render: (_, record) => {
                return (
                    <span>
        <Popconfirm title={t('confirmDelete')} onConfirm={() => deleteItem(record.guid)}>
          <a>{t('delete')}</a>
        </Popconfirm>
        </span>
                );
            },
        }
    ];

    const error = (messageText) => {
        message.error(messageText);
    };

    const success = (messageText) => {
        message.success(messageText);
    };

    const mergedColumns = columns.map((col) => {
        if (!col.editable) {
            return col;
        }

        return {
            ...col,
            onCell: (record) => ({
                record,
                inputType: col.dataIndex === 'age' ? 'number' : 'text',
                dataIndex: col.dataIndex,
                title: col.title,
                editing: isEditing(record),
            }),
        };
    });
    return (
        <span>
    <Button type="primary" onClick={showModal} style={{marginBottom: "15px", float: 'right'}}>
      {t('new')}
    </Button>
    <Form form={form} component={false}>
      <Table
          components={{
              body: {
                  cell: EditableCell,
              },
          }}
          bordered
          dataSource={data}
          columns={mergedColumns}
          rowClassName="editable-row"
          pagination={{
              onChange: cancel,
          }}
      />
    </Form>
    <Modal title={t('new')} open={isModalOpen} onOk={handleOk} onCancel={handleCancel}>
      <Form form={modalForm} component={false}>
        <Form.Item label={t('name')} name="name">
          <Input onPressEnter={handleOk}/>
        </Form.Item>
      </Form>
    </Modal>
  </span>
    );
};

export default CalendarStatesTable;