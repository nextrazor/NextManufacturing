import {Form, Input, InputNumber, DatePicker, Cascader, Button, Modal, message} from 'antd';
import React, {useState, useEffect} from 'react';
import {useTranslation, Trans} from 'react-i18next';//formatter
import {eventEmitter} from '../../systems/Events.ts'
import {formatter} from '../../systems/Formatter'

const CalendarTemplateModal = () => {
    const [modalForm] = Form.useForm();
    const [data, setData] = useState();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const {t} = useTranslation('calendarTemplatesModal');

    const showModal = () => {
        setIsModalOpen(true);
    };

    const handleOk = async () => {
        const row = await modalForm.validateFields();
        if (row.length == undefined)
            row.length = 1;
        if (row.refDate == undefined)
            row.refDate = Date.now();
        if (row.name && row.name != undefined && row.name != '') {
            fetch(`https://localhost:7167/CalendarTemplates/CreateCalendarTemplate/${row.name}/${row.spanGuid[0]}/${row.length}/${formatter.formatAntDateTime(row.refDate)}`
                , {method: 'POST'}).then(response => {
                if (response.ok) {
                    response.json().then(parsedResponse => {
                        eventEmitter.dispatch('updateData_CT', parsedResponse);
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

    
    useEffect(() => {  fetch(`https://localhost:7167/CalendarStates`, {method: 'GET'}).then(response => {
        if (response.ok) {
            response.json().then(data=> {
                data.forEach(el => {el.value = el.guid; el.label = el.name})
                console.log(data);
                setData(data);
                success(t('messages.deleteSuccess'));
            })
            
        } else {
            logErrorFromResponse(response);
        }
    }) },[]);

    const onChangeCascader = (value) => {
        console.log(value);
    }


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
                    <Form.Item label={t('defaultSpan')} name="spanGuid">
                        <Cascader options={data} onChange={onChangeCascader} placeholder={t('defaultSpanPlaceholder')}/>
                    </Form.Item>
                    <Form.Item label={t('timeFieldName')} name="length">
                        <InputNumber addonAfter={t("hours")} defaultValue={1} />
                    </Form.Item>
                    <Form.Item label={t('referenceDate')} name="refDate">
                        <DatePicker showTime format='DD.MM.YYYY HH:mm' placeholder={t('defaultTimePlaceholder')}/>
                    </Form.Item>
                </Form>
            </Modal>
        </span>
    );
};

export default CalendarTemplateModal;