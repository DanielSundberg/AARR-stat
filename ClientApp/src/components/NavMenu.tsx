import * as React from 'react';
import { connect } from 'react-redux';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import { ApplicationState } from '../store';
import * as AuthStore from '../store/Auth';

// At runtime, Redux will merge together...
type AuthProps =
  AuthStore.AuthState // ... state we've requested from the Redux store
  & typeof AuthStore.actionCreators; // ... plus action creators we've requested

class NavMenu extends React.PureComponent<AuthProps, { isOpen: boolean }> {
    public state = {
        isOpen: false
    };

    public render() {
        if (AuthStore.utils.sessionValid(this.props.token, this.props.expires)) {
            return (
                <header>
                    <Navbar className="navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-3" light>
                        <Container>
                            <NavbarBrand tag={Link} to="/">AARR statistics</NavbarBrand>
                            <NavbarToggler onClick={this.toggle} className="mr-2"/>
                            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={this.state.isOpen} navbar>
                                <ul className="navbar-nav flex-grow">
                                    <NavItem>
                                        <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
                                    </NavItem>
                                    <NavItem>
                                        <NavLink tag={Link} className="text-dark" to="/details">Details</NavLink>
                                    </NavItem>
                                    <NavItem>
                                        <NavLink tag={Link} className="text-dark" to="/system">System</NavLink>
                                    </NavItem>
                                </ul>
                            </Collapse>
                        </Container>
                    </Navbar>
                </header>
            );
        } else {
            return null;
        }
    }

    private toggle = () => {
        this.setState({
            isOpen: !this.state.isOpen
        });
    }
}
export default connect(
    (state: ApplicationState) => state.auth, // Selects which state properties are merged into the component's props
    AuthStore.actionCreators // Selects which action creators are merged into the component's props
  )(NavMenu as any);