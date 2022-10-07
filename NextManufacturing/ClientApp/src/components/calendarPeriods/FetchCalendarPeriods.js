import React, {Component} from 'react';
import {Form, Input, InputNumber, Popconfirm, Table, Typography} from 'antd';
import {withTranslation} from 'react-i18next';
import CalendarPeriodsTable from './CalendarPeriodsTable'

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

    render() {
        const {t} = this.props;
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : <CalendarPeriodsTable originData={this.state.data}/>;

        return (
            <div>
                <h1 id="tabelLabel">{t('pageName')}</h1>
                {contents}
            </div>
        );
    }

    async populateData() {
        const response = await fetch('https://localhost:7167/CalendarStates');
        const dataFetched = await response.json();
        dataFetched.forEach(el => el.key = el.guid)
        this.setState({data: dataFetched, loading: false});
    }
}

export default withTranslation('calendarPeriods')(FetchCalendarPeriods);