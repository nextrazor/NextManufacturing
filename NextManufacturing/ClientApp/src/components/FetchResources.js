import React, { Component } from 'react';
import { Form, Input, InputNumber, Popconfirm, Table, Typography } from 'antd';
import ResourceTable from './ResourceTable'

export class FetchResources extends Component {
  static displayName = FetchResources.name;

  constructor(props) {
    super(props);
    
    this.state = { data: [], loading: true };
  }

  componentDidMount() {
    this.populateData();
  }
  
  onChange = (pagination, filters, sorter, extra) => {
    console.log('params', pagination, filters, sorter, extra);
  };

  static renderTable(data) {
    return (
      <ResourceTable originData={data}  />
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : <ResourceTable originData={this.state.data}/>;

    return (
      <div>
        <h1 id="tabelLabel" >Resources</h1>
        {contents}
      </div>
    );
  }

  async populateData() {
    const response = await fetch('https://localhost:7167/Resources');
    const dataFetched = await response.json();
    dataFetched.forEach(el => el.key = el.guid)
    this.setState({ data: dataFetched, loading: false });
  }
}
