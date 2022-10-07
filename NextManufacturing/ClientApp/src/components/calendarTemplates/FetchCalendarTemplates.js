import React, {Component} from 'react';
import {Form, Input, InputNumber, Popconfirm, Table, Modal, Typography} from 'antd';
import {withTranslation} from 'react-i18next';
import CalendarTemplatesTable from './CalendarTemplatesTable'
import CalendarTemplateModal from './CalendarTemplateModal'

class FetchCalendarPeriods extends Component {
    static displayName = FetchCalendarPeriods.name;
    
    constructor(props) {
        super(props);

        this.state = {data: [], loading: true};
    }

    componentDidMount() {
        this.populateData();
    }

    onChange = (pagination, filters, sorter, extra) => {
        console.log('params', pagination, filters, sorter, extra);
    };

    update() {

    }

    render() {
        const {t} = this.props;
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : <CalendarTemplatesTable originData={this.state.data}/>;

        return (
            <div>
                <h1 id="tabelLabel">{t('pageName')}</h1>
                <CalendarTemplateModal call={this.update}/>
                {contents}
            </div>
        );
    }

    async populateData() {
        const response = await fetch('https://localhost:7167/CalendarTemplates');
        const dataFetched = await response.json();
        dataFetched.forEach(el => el.key = el.guid)
        this.setState({data: dataFetched, loading: false});
    }
}

export default withTranslation('calendarTemplates')(FetchCalendarPeriods);