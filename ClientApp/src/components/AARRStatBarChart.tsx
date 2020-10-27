import * as React from 'react';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from 'recharts';

export default (props: { data: any }) => (
    <BarChart
        width={600}
        height={200}
        data={props.data}
        margin={{
        top: 5, right: 30, left: 20, bottom: 5,
        }}
    >
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="key" />
        <YAxis />
        <Tooltip />
        <Legend />
        <Bar dataKey="value" fill="#007BFF" />
    </BarChart>
);