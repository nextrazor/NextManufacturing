import {
  DownloadOutlined,
  UploadOutlined,
  UserOutlined,
  VideoCameraOutlined,
} from '@ant-design/icons';
import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Menu } from 'antd';
import { useTranslation, Trans } from 'react-i18next';
import {LoginMenu} from "./api-authorization/LoginMenu";



export const NavMenu = () => {
  let navigate = useNavigate();
  const { t } = useTranslation('menu');

  function onItemClick(item) {
    if (!item.key.includes('subMenu'))
        navigate(item.key);
  }

  return (
  <div>
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
              label: t('home'),
            },
            {
              key: '/counter',
              icon: <VideoCameraOutlined />,
              label: t('counter'),
            },
            {
              key: '/fetch-data',
              icon: <UploadOutlined />,
              label: t('fetchData'),
            },
            {
              key: '/settings',
              icon: <DownloadOutlined />,
              label: t('settings'),
            },
            {
                key:'subMenu1',
                icon: <DownloadOutlined />,
                label: t('catalogues'),
                children:[
                    {
                      key: '/fetch-resources',
                      icon: <DownloadOutlined />,
                      label: t('resources'),
                    },
                    {
                      key: '/fetch-calendar-periods',
                      icon: <DownloadOutlined />,
                      label: t('calendarPeriods'),
                    },
                    {
                      key: '/fetch-calendar-templates',
                      icon: <DownloadOutlined />,
                      label: t('calendarTemplates'),
                    },
                ]
            
            }
            
          ]}
      />
      <div style={{bottom: 0, left: 0, position: "absolute", marginBottom: "15px"}}>
          <LoginMenu>
          </LoginMenu>
      </div>
  </div>
  );
}