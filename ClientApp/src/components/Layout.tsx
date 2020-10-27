import * as React from 'react';
import { Container } from 'reactstrap';
import CheckAuth from './CheckAuth';
import NavMenu from './NavMenu';

export default (props: { children?: React.ReactNode }) => (
    <React.Fragment>
        <CheckAuth>
            <NavMenu/>
            <Container>
                {props.children}
            </Container>
        </CheckAuth>
    </React.Fragment>
);
