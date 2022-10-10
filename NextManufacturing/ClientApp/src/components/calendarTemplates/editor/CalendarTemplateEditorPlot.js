import React, { useState, useEffect } from 'react';
import ReactDOM from 'react-dom';
import { Bar } from '@ant-design/plots';

const DemoBar = () => {
  const data = [
    {
      year: '1991',
      value: 1,
      type: 'работа',
      colorText: 'red'
    },
    {
      year: '1991',
      value: 2,
      type: 'простой',
      colorText: 'green'
    },
    {
      year: '1991',
      value: 3,
      type: 'работа2',
      colorText: 'red'
    },
    {
      year: '1991',
      value: 5,
      type: 'работа3',
      colorText: 'green'
    },
    
  ];
  const config = {
    data: data.reverse(),
    isStack: true,
    xField: 'value',
    yField: 'year',
    seriesField: 'type',
    color: ({ type }) => {
    if(type.includes('работа')){
        return '#597ef7';
      }
      return '#ffc069';
    },
    label: {
      // 可手动配置 label 数据标签位置
      position: 'middle',
      // 'left', 'middle', 'right'
      // 可配置附加的布局方法
      layout: [
        // 柱形图数据标签位置自动调整
        {
          type: 'interval-adjust-position',
        }, // 数据标签防遮挡
        {
          type: 'interval-hide-overlap',
        }, // 数据标签文颜色自动调整
        {
          type: 'adjust-color',
        },
      ],
    },
  };
  return <Bar {...config} />;
};

ReactDOM.render(<DemoBar />, document.getElementById('container'));
