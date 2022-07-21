import axios from "axios";
import { AppAction, IAppState } from "./common/AppContext"

const instance = axios.create({
  baseURL: "https://localhost:7171",
});

export const buildAxiosConfig = (jwtAccessToken: string): any => {
  return {
    headers: { Authorization: `Bearer ${jwtAccessToken}` },
  };
};

export const acquireJwtAccessToken = (appState: IAppState , appStateDispatch: React.Dispatch<AppAction>): string => {
  // if not expire, 
  // return jwt access token from appState

  // if expire
  // refresh token
  // -- if cannot refresh (401) --> go to login
  // -- else if (200) return jwt from refreshTokenRs
  // -- else throw error

  return "";
};

export default instance;
