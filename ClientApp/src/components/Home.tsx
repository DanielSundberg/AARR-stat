import * as React from 'react';
import { connect } from 'react-redux';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from 'recharts';

interface HomeState {
    usersPerDay: [];
    totPerDay: [];
    avgLongSessionPerDay: [];
    error: string;
    isLoading: boolean;
}

const headers = new Headers({
    'Accept': 'application/json',
});

class Home extends React.PureComponent<{}, HomeState> {
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
        // We're only going to fetch read only data
        // No need for a store or to communicate state
        fetch(`app/dashboard`, { 
            method: 'Get', 
            headers: headers
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
                this.setState({
                    usersPerDay: data.usersPerDay,
                    totPerDay: data.totPerDay,
                    avgLongSessionPerDay: data.avgLongSessionPerDay, 
                    isLoading: false,
                    error: ''
                });
            } else {
                this.setState({
                    usersPerDay: [],
                    totPerDay: [],
                    avgLongSessionPerDay: [], 
                    isLoading: false,
                    error: data.message
                })
            }
        })
        .catch(err => {
            this.setState({
                usersPerDay: [],
                totPerDay: [],
                avgLongSessionPerDay: [], 
                isLoading: false,
                error: err
            })
        });
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
                  <BarChart
                      width={600}
                      height={200}
                      data={this.state.usersPerDay}
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
                      data={this.state.totPerDay}
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
                      data={this.state.avgLongSessionPerDay}
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
        }
    }
}

export default connect()(Home);
