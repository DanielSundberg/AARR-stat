import { Action, Reducer } from 'redux';
import { AppThunkAction } from './';

export interface AuthState {
    isLoading: boolean;
    token: string;
    expires: Date;
    error: string;
}

interface RequestLoginAction {
    type: 'REQUEST_LOGIN';
    username: string;
    password: string;
}

interface ReceiveLoginAction {
    type: 'RECEIVE_LOGIN';
    token: string;
    expires: Date;
    error: string;
}

type KnownAction = RequestLoginAction | ReceiveLoginAction;

const headers = new Headers({
    'Accept': 'application/json',
    'Content-Type': 'application/json'
});

export const utils = {
    sessionValid: (token: string, expires: Date) =>  {
        const sessionValid = token.length > 0 && expires > new Date();
        // tslint-disable-next-line
        // console.log("Session valid: ", sessionValid);
        return sessionValid;
    }
}

export const actionCreators = {
    requestLogin: (username: string, password: string): AppThunkAction<KnownAction> => (dispatch, getState) => {
        const body = {
            username: username, 
            password: password
        };
        fetch(`app/login`, { 
            method: 'POST', 
            headers: headers, 
            body: JSON.stringify(body)
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
                let expires = new Date(Date.now() + data.expiresInSeconds * 1000);
                localStorage.setItem("sessionToken", data.identityToken);
                localStorage.setItem("sessionExpires", expires.toISOString());
                dispatch({ type: 'RECEIVE_LOGIN', token: data.identityToken, expires: expires, error: "" });
            } else {
                dispatch({ type: 'RECEIVE_LOGIN', token: "", expires: new Date(0), error: data.message });
            }
        })
        .catch(err => {
            dispatch({ type: 'RECEIVE_LOGIN', token: "", expires: new Date(0), error: err.message });
        });

        dispatch({ type: 'REQUEST_LOGIN', username: username, password: password });
    }
};

const unloadedState: AuthState = { 
    token: localStorage.getItem("sessionToken") || '', 
    expires: new Date(localStorage.getItem("sessionExpires") || ''), 
    error: "", 
    isLoading: false 
};

export const reducer: Reducer<AuthState> = (state: AuthState | undefined, incomingAction: Action): AuthState => {
    if (state === undefined) {
        return unloadedState;
    }

    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'REQUEST_LOGIN':
            return {
                token: state.token,
                expires: state.expires,
                error: state.error,
                isLoading: true
            };
        case 'RECEIVE_LOGIN':
            return {
                token: action.token,
                expires: action.expires,
                error: action.error,
                isLoading: false
            };
    }

    return state;
};
