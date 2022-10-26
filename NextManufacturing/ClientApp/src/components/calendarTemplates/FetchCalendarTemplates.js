import React, {Component} from 'react';
import {Form, Input, InputNumber, Popconfirm, Table, Modal, Typography} from 'antd';
import {withTranslation} from 'react-i18next';
import CalendarTemplatesTable from './CalendarTemplatesTable'
import CalendarTemplateModal from './CalendarTemplateModal'
import dayjs from 'dayjs';

class FetchCalendarTemplates extends Component {
    static displayName = FetchCalendarTemplates.name;
    
    constructor(props) {
        super(props);

        this.state = {data: [], loading: true};
    }

    componentDidMount() {
        this.populateData();
        console.log(this.props.params);
    }

    onChange = (pagination, filters, sorter, extra) => {
        console.log('params', pagination, filters, sorter, extra);
    };

    render() {
        const {t} = this.props;
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : <CalendarTemplatesTable originData={this.state.data}/>;

        return (
            <div>
                <h1 id="tabelLabel">{t('pageName')}</h1>
                <CalendarTemplateModal/>
                {contents}
            </div>
        );
    }

    async populateData() {
        const response = await fetch('https://localhost:7167/CalendarTemplates');
        const dataFetched = await response.json();
        dataFetched.forEach(el => {
            el.key = el.guid;
            el.date = dayjs(el.referenceDate); //Date(el.referenceDate)
        });
        
         const response2 = await fetch('https://localhost:7167/CalendarStates');
         const dataFetched2 = await response2.json();
         dataFetched2.forEach(el => {
            el.value = el.guid;
            el.label = el.name;
         })
         
        this.setState({data: { templates: dataFetched, periods: dataFetched2  }, loading: false});
    }
}

export default withTranslation('calendarTemplates')(FetchCalendarTemplates);