import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import CheckAuth from './components/CheckAuth';
import Login from './components/Login';

import './custom.css'

export default () => (
    <Layout>
        <Route exact path='/' component={CheckAuth} />
        <Route exact path='/login' component={Login} />
        <Route exact path='/home' component={Home} />
        <Route component={CheckAuth} />
    </Layout>
);
