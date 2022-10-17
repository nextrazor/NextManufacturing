import {Form, Input, InputNumber, Popconfirm, Cascader, DatePicker, Table, Typography, Button, message} from 'antd';
import React, {useState} from 'react';
import { useTranslation, Trans } from 'react-i18next';
import {eventEmitter} from '../../systems/Events.ts'

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

const CalendarTemplatesTable = (originData) => {
    const [form] = Form.useForm();
    const [modalForm] = Form.useForm();
    const [data, setData] = useState(originData.originData.templates);
    const [periods, setPeriods] = useState(originData.originData.periods);
    const [editingKey, setEditingKey] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const isEditing = (record) => record.key === editingKey;
    const { t } = useTranslation('calendarTemplatesTable');

    console.log(originData)

    eventEmitter.subscribe('updateData_CT', (event) => addItem(event));

    const addItem = (item) =>{
        item.key = item.guid;
        const newData = [...data];
        newData.push(item);
        setData(newData);
    }


    const logErrorFromResponse = (response) => {
        response.json().then(parsedResponse => {
            error(t('messages.error') + parsedResponse.error);
        })
    }

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
                fetch(`https://localhost:7167/CalendarTemplates/DeleteCalendarTemplates/${key}`, {method: 'DELETE'}).then(response => {
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
                var response = await fetch(`https://localhost:7167/CalendarTemplates/UpdateCalendarTemplates/${key}/${row.name}`, {method: 'POST'})

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
            title: 'Length',
            dataIndex: 'periodDuration',
            sorter: (a, b) => a.periodDuration - b.periodDuration,
            editable: true,
        },
        {
            title: 'Date',
            dataIndex: 'date',
            sorter: (a, b) => a.periodDuration - b.periodDuration,
            render: (_, record) => {
                        const editable = isEditing(record);
            return editable ? (
                            <span>
                                <Form.Item name="date">
                                    <DatePicker showTime format='DD.MM.YYYY HH:mm' />
                                </Form.Item>
                            </span>
                        ) : (
                            <span>
                                {record.date.format('DD.MM.YYYY HH:mm')}
                            </span>
                            );
                        },
        },
        {
        title: 'Default Template',
        dataIndex: 'defaultPeriodName',
        sorter: (a, b) => a.periodDuration - b.periodDuration,
        render: (_, record) => {
                    const editable = isEditing(record);
                    return editable ? (
                        <span>
                            <Form.Item name="defaultStateName">
                                <Cascader options={periods}/>
                            </Form.Item>
                        </span>
                    ) : (
                        <span>
                            {record.defaultStateName}
                        </span>
                        );
                    },
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
            title: t('configure'),
            dataIndex: 'operation3',
            render: (_, record) => {
                return(
                <span>
                    <Typography.Link disabled={editingKey !== ''} onClick={() => edit(record)}>
                        {t('configure')}
                    </Typography.Link>
                </span>)}
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
  </span>
    );
};

export default CalendarTemplatesTable;