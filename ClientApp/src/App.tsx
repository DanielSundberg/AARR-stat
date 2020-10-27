import * as React from 'react';
import { Route, Redirect, Switch } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Login from './components/Login';

import './custom.css'

const NotFound = () => (
    <React.Fragment>
        <h2>Invalid route</h2>
    </React.Fragment>
)

export default () => (
    <Layout>
        {/* <Route exact path='/' component={Home} /> */}
        <Switch>
            <Route exact path='/login' component={Login} />
            <Route exact path='/home' component={Home} />
            <Route exact path='/details' component={Home} />
            <Route exact path='/system' component={Home} />
            <Route exact path='/logout' component={Home} />
            <Redirect from="/" to="/home" />
            <Route exact component={NotFound} />
        </Switch>
    </Layout>
);
