import React, { createContext, useContext } from "react";

export interface IAppState {
  jwtAccessToken: string;
  user: IUser | null;
}

export interface IUser {
  id: string;
  userName: string;
  roleName: string;
  isAdmin: boolean;
  isReceptionist: boolean;
  isCollaborator: boolean;
  isMember: boolean;
}

export enum AppActionType {
  RefreshToken = "RefreshToken",
  Login = "Login",
}

export type AppAction =
  | { type: AppActionType.RefreshToken; jwtAccessToken: string }
  | { type: AppActionType.Login; user: IUser; jwtAccessToken: string };

export const initialAppState: IAppState = {
  jwtAccessToken: "",
  user: {
    id: "",
    userName: "",
    roleName: "",
    isAdmin: false,
    isReceptionist: false,
    isCollaborator: false,
    isMember: false,
  },
};

export function appStateReducer(
  state: IAppState,
  action: AppAction
): IAppState {
  switch (action.type) {
    case AppActionType.Login:
      return {
        ...state,
      };
    case AppActionType.RefreshToken:
      return {
        ...state,
        jwtAccessToken: action.jwtAccessToken,
      };
    default:
      throw new Error("Invalid action type");
  }
}

export interface IAppContext {
  appState: IAppState;
  appStateDispatch: React.Dispatch<AppAction>;
}

export const initialAppContext: IAppContext = {
  appState: initialAppState,
  appStateDispatch: (action: AppAction) => {},
};

export const AppContext = createContext(initialAppContext);

export const useAppContext = () => {
    return useContext(AppContext);
}