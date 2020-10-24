import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as AuthStore from '../store/Auth';
import { Redirect } from 'react-router';

type AuthProps = AuthStore.AuthState & typeof AuthStore.actionCreators;

class CheckAuth extends React.PureComponent<AuthProps> {
    public render() {
    if (AuthStore.utils.sessionValid(this.props.token, this.props.expires)) {
        return (<Redirect to="/home" />);
    } else {
        return (<Redirect to="/login" />);
    }
    // } else {
    //     return (
    //         <React.Fragment>
    //             <div className="ui grid container">
    //                 <div className="ui active dimmer">
    //                     <p/>
    //                     <div className="ui text loader">Checking for saved login...</div>
    //                 </div>
    //             </div>
    //         </React.Fragment>
    //     );
    // }
  }
}

export default connect(
  (state: ApplicationState) => state.auth, // Selects which state properties are merged into the component's props
  AuthStore.actionCreators // Selects which action creators are merged into the component's props
)(CheckAuth as any);
