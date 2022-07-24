import React, { createContext, useContext } from "react";

export interface IAppState {
  jwtAccessToken: string;
  jwtAccessTokenExpiresOn: Date | null;
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
  | {
      type: AppActionType.RefreshToken;
      jwtAccessToken: string;
      jwtAccessTokenExpiresOn: Date;
    }
  | {
      type: AppActionType.Login;
      user: IUser;
      jwtAccessToken: string;
      jwtAccessTokenExpiresOn: Date;
    };

export const initialAppState: IAppState = {
  jwtAccessToken: "",
  jwtAccessTokenExpiresOn: null,
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
        user: action.user,
        jwtAccessToken: action.jwtAccessToken,
        jwtAccessTokenExpiresOn: action.jwtAccessTokenExpiresOn,
      };
    case AppActionType.RefreshToken:
      return {
        ...state,
        jwtAccessToken: action.jwtAccessToken,
        jwtAccessTokenExpiresOn: action.jwtAccessTokenExpiresOn,
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
};
