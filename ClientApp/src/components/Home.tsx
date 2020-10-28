import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as AuthStore from '../store/Auth';
import AARRStatBarChart from './AARRStatBarChart';

interface HomeState {
    usersPerDay: [];
    totPerDay: [];
    avgLongSessionPerDay: [];
    error: string;
    isLoading: boolean;
}

type AuthProps = AuthStore.AuthState & typeof AuthStore.actionCreators;

class Home extends React.PureComponent<AuthProps, HomeState> {
    abortController : AbortController = new AbortController();

    constructor(props: any) {
        super(props);
        this.state = { 
            usersPerDay: [],
            totPerDay: [], 
            avgLongSessionPerDay: [], 
            error: '', 
            isLoading: true,
        };
    }

    componentDidMount() {
        this.refreshData(this);
    }

    refreshData(self: any) { // tslint:disable-line
        // We're only going to fetch read only data
        // No need for a store or to communicate state
        const headers = new Headers({
            'Accept': 'application/json',
            'Authorization': self.props.token
        });
        fetch(`app/dashboard`, { 
            method: 'Get', 
            headers: headers, 
            signal: self.abortController.signal
        })
        .then(res => {
            if (res.status !== 401 && !res.ok) {
                return res.text().then(text => {
                    throw new Error(`Http status ${res.status}.`);
                })
            } else {
                return res.json();
            }
        })
        .then(data => {
            if (data.result === "ok") {
                self.setState({
                    usersPerDay: data.usersPerDay,
                    totPerDay: data.totPerDay,
                    avgLongSessionPerDay: data.avgLongSessionPerDay, 
                    isLoading: false,
                    error: ''
                });
            } else {
                self.setState({
                    usersPerDay: [],
                    totPerDay: [],
                    avgLongSessionPerDay: [], 
                    isLoading: false,
                    error: data.message
                })
            }
        })
        .catch(err => {
            self.setState({
                usersPerDay: [],
                totPerDay: [],
                avgLongSessionPerDay: [], 
                isLoading: false,
                error: err
            })
        });
    }

    componentWillUnmount() {
        this.abortController.abort();
    }

    render() {
        if (this.state.isLoading) {
            return (
                <div>
                    <div className="spinner-border" role="status">
                        <span className="sr-only">Loading...</span>
                    </div>
                    Loading...
                </div>
            );
        } else if (this.state.error.length > 0) {
            return (
                <div className="col-md-12 col-sm-12">
                    <div className="alert alert-danger alert-dismissible fade show">
                        <strong>Error!</strong> {this.state.error}
                    </div>
                </div>
            );
        } else {
            return (
                <div>
                    <p>AARR RSS Reader usage data. This is usage data collected from AARR users during the last 10 days.</p>
                    <h5>Users per day</h5>
                    <AARRStatBarChart data={this.state.usersPerDay} />

                    <h5>
                        Total time per day
                        <small className="text-muted"> - minutes spent in the app across all users</small>
                    </h5>
                    
                    <AARRStatBarChart data={this.state.totPerDay} />

                    <h5>
                        Average long session time 
                        <small className="text-muted"> - seconds of focused session time across all users</small>
                    </h5>
                    <AARRStatBarChart data={this.state.avgLongSessionPerDay} />

                    <button 
                        type="button"
                        className="btn btn-primary"
                        onClick={(ev: any) => this.refreshData(this)}
                    >
                        Refresh
                    </button>
                </div>
              );
        }
    }
}

export default connect(
    (state: ApplicationState) => state.auth, // Selects which state properties are merged into the component's props
    AuthStore.actionCreators // Selects which action creators are merged into the component's props
)(Home as any);
