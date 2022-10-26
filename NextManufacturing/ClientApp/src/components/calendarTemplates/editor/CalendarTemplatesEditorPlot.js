import React, { useState, useEffect } from 'react';
import { Bar } from '@ant-design/plots';

const CalendarTemplatesEditorPlot = (originData) => {
  
  const config = {
    data: originData.originData.reverse(),
    isStack: true,
    xField: 'value',
    yField: 'year',
    seriesField: 'type',
    color: ({ type }) => {
    if(type.includes('Работа')){
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

export default CalendarTemplatesEditorPlot;
