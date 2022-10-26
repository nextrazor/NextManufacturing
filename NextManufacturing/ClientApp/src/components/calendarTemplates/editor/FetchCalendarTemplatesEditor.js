import React, {Component} from 'react';
import {Form, Input, InputNumber, Popconfirm, Table, Modal, Typography} from 'antd';
import {withTranslation} from 'react-i18next';
import CalendarTemplatesEditorTable from './CalendarTemplatesEditorTable'
//import CalendarTemplatesEditorModal from './CalendarTemplatesEditorModal'
import CalendarTemplatesEditorPlot from './CalendarTemplatesEditorPlot'
import {eventEmitter} from '../../../systems/Events.ts'
import {useParams} from 'react-router-dom'
import dayjs from 'dayjs';
var moment = require('moment');

function withParams(Component) {
  return props => <Component {...props} params={useParams()} />;
}

class FetchCalendarTemplatesEditor extends Component {
    static displayName = FetchCalendarTemplatesEditor.name;
    
    constructor(props) {
        super(props);
        
        this.state = {data: [],  templateToEdit: props.params.template, defaultState: {}, loading: true};
    }

    componentDidMount() {
        this.populateData();
        eventEmitter.subscribe('updateData_CTEditor', (event) => this.populateData(event));
    }
    
    
    render() {
        const {t} = this.props;
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            :<div><CalendarTemplatesEditorPlot originData={this.state.dataView}/><CalendarTemplatesEditorTable originData={this.state.data}/></div> ;

        return (
            <div>
                <h1 id="tabelLabel">{t('pageName')}</h1>

                {contents}
            </div>
        );//                <CalendarTemplateEditorModal call={this.update}/>
    }
    
    async loadData() 
    {
        const templateDataCall = await fetch('https://localhost:7167/CalendarTemplates/GetByGuid/'+this.state.templateToEdit);
        const templateDataFetched = await templateDataCall.json();
        
        var refDate = templateDataFetched[0].startDate;
        templateDataFetched[0].startDate = moment(refDate);
        templateDataFetched[0].periodDurationObject = moment.duration(templateDataFetched[0].periodDuration);
        templateDataFetched[0].endDate = moment(refDate).add(templateDataFetched[0].periodDurationObject);
        
        this.setState({ templateData: templateDataFetched[0] });
        
        
        
        const response2 = await fetch('https://localhost:7167/CalendarStates');
        const dataFetched2 = await response2.json();
        dataFetched2.forEach(el => {
           el.value = el.guid;
           el.label = el.name;
        })
        this.setState({ states: dataFetched2 });
        
        const response = await fetch('https://localhost:7167/CalendarTemplateSpans/GetSpansByTemplate/'+this.state.templateToEdit);
        const templateSpansFetched = await response.json();
        
        templateSpansFetched.forEach(el => {
            el.key = el.guid;
            el.datetimeFrom = moment(refDate).add(moment.duration(el.fromTime));
            el.dateFrom = dayjs(el.datetimeFrom.format());
            el.datetimeFromText = el.datetimeFrom.format();
            el.datetimeTo = moment(refDate).add(moment.duration(el.toTime));
            el.dateTo = dayjs(el.datetimeTo.format());
            el.datetimeToText = el.datetimeTo.format();
            el.stateName = dataFetched2.filter(state => state.guid == el.stateGuid)[0].name
        });
        this.setState({ data: templateSpansFetched.reverse() });
        
//        const response2 = await fetch('https://localhost:7167/CalendarStates/GetByGuid/'+this.state.templateToEdit.defaultStateGuid);
//        const defaultStateFetched = await response2.json();
//        this.setState({defaultState: defaultStateFetched});
        
        console.log('here-----------------------------------------------');
        console.log(templateDataFetched[0]);
        console.log(templateSpansFetched);
        console.log(dataFetched2);
        console.log(this.state);
    }

    async populateData() {
        await this.loadData();
        console.log(this.state);
        var template = this.state.templateData;
        var states = this.state.states;
        var defaultState = this.state.states.filter(el => el.guid == this.state.templateData.defaultStateGuid)[0]
        
        var drawerData = [];
        
        if (this.state.data.length == 0)
        {
            drawerData.push({
                year: '1',
                value: parseFloat(template.periodDuration),
                type: template.defaultStateName,
                colorText: 'red'
            });
        }
        else
        {
            var buffer = [];
           
            if (template.startDate.format() != this.state.data[0].datetimeFrom.format())
                buffer.push({ startDate: template.startDate, endDate: this.state.data[0].datetimeFrom, state: defaultState.name + ' 0'});
                
            for (var i = 0; i < this.state.data.length; i++){
            console.log(states.filter(el => el.guid == this.state.data[0].stateGuid)[0].name);
                buffer.push({ startDate: this.state.data[i].datetimeFrom, endDate: this.state.data[i].datetimeTo, state: states.filter(el => el.guid == this.state.data[0].stateGuid)[0].name + ' ' + (i+1)});
                if (i < this.state.data.length - 1){
                    if (this.state.data[i].datetimeTo.format() != this.state.data[i + 1].datetimeFrom.format())
                        buffer.push({ startDate: this.state.data[i].datetimeTo, endDate: this.state.data[i + 1].datetimeFrom, state: defaultState.name + ' ' + (i+1)});
                }
                else if (i == this.state.data.length - 1){
                    if (this.state.data[i].datetimeTo.format() != this.state.templateData.endDate.format())
                        buffer.push({ startDate: this.state.data[i].datetimeTo, endDate: this.state.templateData.endDate, state: defaultState.name + ' ' + (i+1)});
                }
            }
            
            
            
            buffer.forEach(el => {
                drawerData.push({
                    year: el.startDate.date(),
                    value: -Math.round(el.startDate.diff(el.endDate, 'minutes')/60 * 100) / 100, 
                    type: el.state, 
                })
            })
            
            console.log('buffer++++++++++++++++++++++++');
            console.log(buffer);
        }
        
        
        
        this.setState({ dataView:drawerData });

        this.setState({loading: false});
    }
}

export default withTranslation('calendarTemplatesEditor')(withParams(FetchCalendarTemplatesEditor));