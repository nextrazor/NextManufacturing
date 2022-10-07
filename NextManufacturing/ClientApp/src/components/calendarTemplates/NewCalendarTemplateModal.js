import {Form, Input, InputNumber, Popconfirm, Table, Typography, Button, Modal, message} from 'antd';
import React, {useState} from 'react';
import {useTranslation, Trans} from 'react-i18next';

const CalendarTemplateModal = (originData) => {
    const [modalForm] = Form.useForm();
    const [data, setData] = useState(originData.originData);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const {t} = useTranslation('calendarTemplatesModal');

    const showModal = () => {
        setIsModalOpen(true);
    };

    const handleOk = async () => {
        const row = await modalForm.validateFields();
        if (row.name && row.name != undefined && row.name != '') {
            fetch(`https://localhost:7167/CalendarTemplates/CreateCalendarTemplates/${row.name}`
                , {method: 'POST'}).then(response => {
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


    const logErrorFromResponse = (response) => {
        response.json().then(parsedResponse => {
            console.log(parsedResponse);
            error(t('messages.error') + parsedResponse.error);
        })
    }

    const error = (messageText) => {
        message.error(messageText);
    };

    const success = (messageText) => {
        message.success(messageText);
    };


    return (
        <span>
            <Button type="primary" onClick={showModal} style={{marginBottom: "15px", float: 'right'}}>
                {t('new')}
            </Button>
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

export default CalendarTemplateModal;