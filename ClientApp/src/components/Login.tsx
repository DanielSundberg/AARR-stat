import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as AuthStore from '../store/Auth';

type AuthProps = AuthStore.AuthState & typeof AuthStore.actionCreators;

class Login extends React.PureComponent<AuthProps, {}> {

  loginClicked(self: any) { // tslint:disable-line
    self.props.requestLogin(self.state.username, self.state.password);
    self.setState({ 
        username: '',
        password: ''
    });
    // this.state = ;
  }

  public render() {
    return (
        <React.Fragment>
            <div className="sidenav">
                <div className="login-main-text">
                    <svg width="7em" height="7em" viewBox="0 0 16 16" className="bi bi-bar-chart-line" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                        <path fillRule="evenodd" d="M4 11H2v3h2v-3zm5-4H7v7h2V7zm5-5h-2v12h2V2zm-2-1a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1h2a1 1 0 0 0 1-1V2a1 1 0 0 0-1-1h-2zM6 7a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v7a1 1 0 0 1-1 1H7a1 1 0 0 1-1-1V7zm-5 4a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v3a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1v-3z"/>
                    </svg>
                    <h2>AARRStat<br/> Login Page</h2>
                    <p>Login to access the<br/>AARRStat app.</p>
                </div>
            </div>
            <div className="main">
                <div className="col-md-6 col-sm-12">
                    <div className="login-form">
                        <form>
                            <div className="form-group">
                                <label>User Name</label>
                                <input 
                                    type="text"
                                    className="form-control"
                                    placeholder="User Name"
                                    // tslint:disable-next-line
                                    onChange={(ev: any) => this.setState({ username: ev.target.value }) } 
                                />
                            </div>
                            <div className="form-group">
                                <label>Password</label>
                                <input 
                                    type="password"
                                    className="form-control"
                                    placeholder="Password"
                                    // tslint:disable-next-line
                                    onChange={(ev: any) => this.setState({ password: ev.target.value })}
                                />
                            </div>
                            <div
                                className="btn btn-black"
                                // tslint:disable-next-line
                                onClick={(ev: any) => this.loginClicked(this)}
                            >
                                Login
                            </div>
                        </form>
                        
                    </div>
                </div>
                <br/>
                <div className="col-md-12 col-sm-12">
                    {this.props.error && (
                        <div className="alert alert-danger alert-dismissible fade show">
                            <strong>Error!</strong> {this.props.error}
                        </div>
                    )}
                </div>
            </div>
      </React.Fragment>
    );
  }
}

export default connect(
  (state: ApplicationState) => state.auth, // Selects which state properties are merged into the component's props
  AuthStore.actionCreators // Selects which action creators are merged into the component's props
)(Login as any);
