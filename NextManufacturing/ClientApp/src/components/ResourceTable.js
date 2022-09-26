import { Form, Input, InputNumber, Popconfirm, Table, Typography, Modal, Button } from 'antd';
import React, { useState } from 'react';

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
  const inputNode = inputType === 'number' ? <InputNumber /> : <Input />;
  return (
    <td {...restProps}>
      {editing ? (
        <Form.Item
          name={dataIndex}
          style={{
            margin: 0,
          }}
          rules={[
            {
              required: true,
              message: `Please Input ${title}!`,
            },
          ]}
        >
          {inputNode}
        </Form.Item>
      ) : (
        children
      )}
    </td>
  );
};

const ResourceTable = (originData) => {
  const [form] = Form.useForm();
  const [modalForm] = Form.useForm();
  const [data, setData] = useState(originData.originData);
  const [editingKey, setEditingKey] = useState('');
  const [deletingKey, setDeletingKey] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  
  var newResourceName = '';
  
  const isEditing = (record) => record.key === editingKey;
  const isDeleting = (record) => record.key === deletingKey;


    const showModal = () => {
      setIsModalOpen(true);
    };
  
    const handleOk = async () => {
      const row = await modalForm.validateFields();
      fetch(`https://localhost:7167/Resources/CreateResource/${row.name}`,{method: 'POST'})
      modalForm.resetFields();
      setIsModalOpen(false);
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
  
  const deleteButton = (record) => {
      setDeletingKey(record.key);
    };

  const cancel = () => {
    setEditingKey('');
  };
  
  const cancelDelete = () => {
    setDeletingKey('');
  };
  
  const deleteItem = async (key) => {
      try {
        const row = await form.validateFields();
        const newData = [...data];
        const index = newData.findIndex((item) => key === item.key);
          
        if (index > -1) {
          const item = newData[index];
          newData.splice(index, 1, { ...item, ...row });
          setData(newData);
          setEditingKey('');
        } else {
          newData.push(row);
          setData(newData);
          setEditingKey('');
        }
        
        fetch(`https://localhost:7167/Resources/DeleteResource/${key}`,{method: 'POST'})
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
        const item = newData[index];
        newData.splice(index, 1, { ...item, ...row });
        setData(newData);
        setEditingKey('');
      } else {
        newData.push(row);
        setData(newData);
        setEditingKey('');
      }
      
      fetch(`https://localhost:7167/Resources/UpdateResource/${key}/${row.name}`,{method: 'POST'})
    } catch (errInfo) {
      console.log('Validate Failed:', errInfo);
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
        title: 'Edit',
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
              <Popconfirm title="Sure to cancel?" onConfirm={cancel}>
                <a>Cancel</a>
              </Popconfirm>
            </span>
          ) : (
          <span>
            <Typography.Link disabled={editingKey !== ''} onClick={() => edit(record)}>
              Edit
            </Typography.Link>
          </span>
          );
        },
      },
      {
              title: 'Delete',
              dataIndex: 'operation',
              render: (_, record) => {
                const editable = isDeleting(record);
                return editable ? (
                  <span>
                    <Typography.Link
                      onClick={() => deleteItem(record.guid)}
                      style={{
                        marginRight: 8,
                      }}
                    >
                      Save
                    </Typography.Link>
                    <Popconfirm title="Sure to delete?" onConfirm={deleteItem}>
                      <a>Cancel</a>
                    </Popconfirm>
                  </span>
                ) : (
                <span>
                  <Typography.Link disabled={deletingKey !== ''} onClick={() => deleteButton(record)}>
                    Delete
                  </Typography.Link>
                </span>
                );
              },
            }
    ];

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
    <Button type="primary" onClick={showModal} style={{marginBottom: "15px"}}>
      New Resources
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
    <Modal title="New resource" open={isModalOpen} onOk={handleOk} onCancel={handleCancel}>
      <Form form={modalForm} component={false}>
        <Form.Item label="Name" name="name">
          <Input onPressEnter={handleOk} />
        </Form.Item>
      </Form>
    </Modal>
  </span>
  );
};

export default ResourceTable;