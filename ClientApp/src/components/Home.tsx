import * as React from 'react';
import { connect } from 'react-redux';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from 'recharts';

const usersPerDay =  [
    {
        "key": "tor okt 15",
        "value": 0
    },
    {
        "key": "fre okt 16",
        "value": 0
    },
    {
        "key": "lör okt 17",
        "value": 0
    },
    {
        "key": "sön okt 18",
        "value": 0
    },
    {
        "key": "mån okt 19",
        "value": 0
    },
    {
        "key": "tis okt 20",
        "value": 0
    },
    {
        "key": "ons okt 21",
        "value": 0
    },
    {
        "key": "tor okt 22",
        "value": 0
    },
    {
        "key": "fre okt 23",
        "value": 0
    },
    {
        "key": "lör okt 24",
        "value": 1
    }
];
const totPerDay = [
    {
        "key": "tor okt 15",
        "value": 0
    },
    {
        "key": "fre okt 16",
        "value": 0
    },
    {
        "key": "lör okt 17",
        "value": 0
    },
    {
        "key": "sön okt 18",
        "value": 0
    },
    {
        "key": "mån okt 19",
        "value": 0
    },
    {
        "key": "tis okt 20",
        "value": 0
    },
    {
        "key": "ons okt 21",
        "value": 0
    },
    {
        "key": "tor okt 22",
        "value": 0
    },
    {
        "key": "fre okt 23",
        "value": 0
    },
    {
        "key": "lör okt 24",
        "value": 31
    }
];
const avgLongSessionPerDay = [
    {
        "key": "tor okt 15",
        "value": 0
    },
    {
        "key": "fre okt 16",
        "value": 0
    },
    {
        "key": "lör okt 17",
        "value": 0
    },
    {
        "key": "sön okt 18",
        "value": 0
    },
    {
        "key": "mån okt 19",
        "value": 0
    },
    {
        "key": "tis okt 20",
        "value": 0
    },
    {
        "key": "ons okt 21",
        "value": 0
    },
    {
        "key": "tor okt 22",
        "value": 0
    },
    {
        "key": "fre okt 23",
        "value": 0
    },
    {
        "key": "lör okt 24",
        "value": 1800
    }
];

const Home = () => (
  <div>
    <h5>Users per day</h5>
    <BarChart
        width={600}
        height={200}
        data={usersPerDay}
        margin={{
        top: 5, right: 30, left: 20, bottom: 5,
        }}
    >
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="key" />
        <YAxis />
        <Tooltip />
        <Legend />
        <Bar dataKey="value" fill="#8884d8" />
    </BarChart>

    <h5>Total time per day (minutes)</h5>
    <BarChart
        width={600}
        height={200}
        data={totPerDay}
        margin={{
        top: 5, right: 30, left: 20, bottom: 5,
        }}
    >
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="key" />
        <YAxis />
        <Tooltip />
        <Legend />
        <Bar dataKey="value" fill="#8884d8" />
    </BarChart>

    <h5>Average long session time (seconds)</h5>
    <BarChart
        width={600}
        height={200}
        data={avgLongSessionPerDay}
        margin={{
        top: 5, right: 30, left: 20, bottom: 5,
        }}
    >
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="key" />
        <YAxis />
        <Tooltip />
        <Legend />
        <Bar dataKey="value" fill="#8884d8" />
    </BarChart>

  </div>
);

export default connect()(Home);
