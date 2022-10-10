import React, {Component} from 'react';
import {Form, Input, InputNumber, Popconfirm, Table, Modal, Typography} from 'antd';
import {withTranslation} from 'react-i18next';
import CalendarTemplatessEditorTable from './CalendarTemplatesEditorTable'
import CalendarTemplatesEditorModal from './CalendarTemplatesEditorModal'
import CalendarTemplatesEditorPlot from './CalendarTemplatesEditorPlot'
import {eventEmitter} from '../../../systems/Events'

class FetchCalendarTemplatesEditor extends Component {
    static displayName = FetchCalendarTemplatesEditor.name;
    
    constructor(props) {
        super(props);

        this.state = {data: [ ], templateToEdit: props.template, defaultState: {}, loading: true};
    }

    componentDidMount() {
        this.populateData();
        eventEmitter.subscribe('updateData_CTEditor', (event) => this.populateData(event));
    }

    render() {
        const {t} = this.props;
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            :<div><CalendarTemplatesEditorPlot originData={this.state}/><CalendarTemplatesEditorTable originData={this.state.data}/></div> ;

        return (
            <div>
                <h1 id="tabelLabel">{t('pageName')}</h1>
                <CalendarTemplateEditorModal call={this.update}/>
                {contents}
            </div>
        );
    }

    async populateData() {
        const response = await fetch('https://localhost:7167/CalendarTemplateSpans/GetSpansByTemplate/'+this.state.templateToEdit.guid);
        const templateSpansFetched = await response.json();
        templateSpansFetched.forEach(el => el.key = el.guid)
        this.setState({data: templateSpansFetched});

        const response2 = await fetch('https://localhost:7167/CalendarStates/GetByGuid/'+this.state.templateToEdit.defaultStateGuid);
        const defaultStateFetched = await response.json();
        this.setState({defaultState: defaultStateFetched, loading: false});
    }
}

export default withTranslation('calendarTemplatesEditor')(FetchCalendarTemplatesEditor);