import {
  DownloadOutlined,
  UploadOutlined,
  UserOutlined,
  VideoCameraOutlined,
} from '@ant-design/icons';
import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Menu } from 'antd';



export const NavMenu = () => {
  let navigate = useNavigate();

  function onItemClick(item) {
    console.log(item);
    navigate(item.key);
  }

  return (
      <Menu
          onClick={onItemClick}
          style={{ height: '100%' }}
          theme="dark"
          mode="inline"
          defaultSelectedKeys={['1']}
          items={[
            {
              key: '/',
              icon: <UserOutlined />,
              label: 'Home',
            },
            {
              key: '/counter',
              icon: <VideoCameraOutlined />,
              label: 'Counter',
            },
            {
              key: '/fetch-data',
              icon: <UploadOutlined />,
              label: 'Fetch data',
            },
            {
              key: '/settings',
              icon: <DownloadOutlined />,
              label: 'Settings',
            },
          ]}
      />
  );
}