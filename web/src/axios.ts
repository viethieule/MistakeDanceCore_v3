import axios from "axios";
import React from "react";
import { AppAction, AppActionType, IAppState } from "./common/AppContext";

const instance = axios.create({
  baseURL: "https://localhost:7171",
});

export const buildAxiosConfig = (jwtAccessToken: string): any => {
  return {
    headers: { Authorization: `Bearer ${jwtAccessToken}` },
  };
};

export const acquireAxiosConfig = async (appState: IAppState, appStateDispatch: React.Dispatch<AppAction>) => {
  const jwtAccessToken = await acquireJwtAccessToken(appState, appStateDispatch);
  return buildAxiosConfig(jwtAccessToken);
}

export const acquireJwtAccessToken = async (
  appState: IAppState,
  appStateDispatch: React.Dispatch<AppAction>
): Promise<string> => {
  debugger;
  if (
    appState.jwtAccessToken &&
    appState.jwtAccessTokenExpiresOn &&
    new Date() < new Date(appState.jwtAccessTokenExpiresOn)
  ) {
    return appState.jwtAccessToken;
  }
  try {
    const response = await instance.get("/api/authentication/refreshtoken");
    const { jwtAccessToken, jwtAccessTokenExpiresOn } = response.data;
    appStateDispatch({
      type: AppActionType.RefreshToken,
      jwtAccessToken,
      jwtAccessTokenExpiresOn,
    });

    return jwtAccessToken;
  } catch (error: any) {
    if (error && error.response && error.response.status === 401) {
      const currentPath = window.location.pathname.toLowerCase();
      window.location.href = `/auth/login?return_url=${encodeURIComponent(
        currentPath
      )}`;
      throw "User not logged in";
    }

    throw error;
  }
};

export default instance;
