import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as AuthStore from '../store/Auth';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import Login from './Login';

type AuthProps = AuthStore.AuthState & typeof AuthStore.actionCreators & { children?: React.ReactNode };

class CheckAuth extends React.PureComponent<AuthProps> {
    public render() {
        if (AuthStore.utils.sessionValid(this.props.token, this.props.expires)) {
            return (<React.Fragment>{this.props.children}</React.Fragment>);
        } else {
            return (
                <React.Fragment>
                    <NavMenu/>
                    <Container>
                        <Login />
                    </Container>
                </React.Fragment>
            );
        }
  }
}

export default connect(
  (state: ApplicationState) => state.auth, // Selects which state properties are merged into the component's props
  AuthStore.actionCreators // Selects which action creators are merged into the component's props
)(CheckAuth as any);
