import React, { useReducer } from "react";

interface IAppState {
    jwtAccessToken: string;
    jwtAccessTokenExpiresOn: Date;
}

type AppAction = {

}

const initialAppState: IAppState = {
    jwtAccessToken: '',
    jwtAccessTokenExpiresOn: new Date()
}

function appStateReducer(state: IAppState, action: AppAction): IAppState {
    return state;
}

export function App() {
    const [appState, dispatch] = useReducer(appStateReducer, { ... initialAppState});
    return (
        <>
        </>
    )
}