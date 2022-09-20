import {
    MenuFoldOutlined,
    MenuUnfoldOutlined,
} from '@ant-design/icons';
import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import { Layout } from 'antd';

import '../custom.css'
import {LoginMenu} from "./api-authorization/LoginMenu";
const { Header, Sider, Content } = Layout;

export class LocalLayout extends Component {
    static displayName = LocalLayout.name;

    constructor(props) {
        super(props);
        this.state = {
            collapsed: false
        };
    }

    render () {
        return (
            <Layout>
                <Sider trigger={null} collapsible collapsed={this.state.collapsed}>
                    <div className="logo" />
                    <NavMenu />
                </Sider>
                <Layout className="site-layout">
                    <Header className="site-layout-background" style={{ padding: 0 }}>
                        
                    <LoginMenu>
                    </LoginMenu>
                    </Header>
                    <Content
                        className="site-layout-background"
                        style={{
                            margin: '24px 16px',
                            padding: 24,
                            minHeight: 280,
                        }}
                    >
                        <Container>
                            {this.props.children}
                        </Container>
                    </Content>
                </Layout>
            </Layout>
        );
    }
}
