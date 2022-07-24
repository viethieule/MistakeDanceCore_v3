import React, { useEffect, useReducer, useState } from "react";
import axios, { buildAxiosConfig } from "../../axios";
import { getErrorMessage } from "../../utils/Error";
import { Routes, Route, BrowserRouter } from "react-router-dom";
import AuthenticationContainer from "../AuthenticationContainer/AuthenticationContainer";
import LoggedInContainer from "../LoggedInContainer/LoggedInContainer";
import {
  AppActionType,
  AppContext,
  appStateReducer,
  initialAppState,
} from "../../common/AppContext";

export default function App() {
  const [appState, appStateDispatch] = useReducer(appStateReducer, {
    ...initialAppState,
  });
  const [loading, setLoading] = useState(true);
  const [loadingError, setLoadingError] = useState("");

  useEffect(() => {
    const currentPath = window.location.pathname.toLowerCase();
    let jwtAccessToken = "";
    let jwtAccessTokenExpiresOn = new Date();
    axios
      .get("api/authentication/refreshtoken", { withCredentials: true })
      .then((response) => {
        jwtAccessToken = response.data.jwtAccessToken;
        jwtAccessTokenExpiresOn = response.data.jwtAccessTokenExpiresOn;
        appStateDispatch({ type: AppActionType.RefreshToken, jwtAccessToken, jwtAccessTokenExpiresOn });
        const config = buildAxiosConfig(jwtAccessToken);
        return axios.get("api/user/current", config);
      })
      .then((response) => {
        const { user } = response.data;
        appStateDispatch({ type: AppActionType.Login, user, jwtAccessToken, jwtAccessTokenExpiresOn });
        setLoading(false);
      })
      .catch((error) => {
        if (error.response && error.response.status === 401) {
          if (currentPath.startsWith("/auth")) {
            setLoading(false);
          } else {
            window.location.href = `/auth/login?return_url=${encodeURIComponent(
              currentPath
            )}`;
          }
        } else {
          const errorMessage = getErrorMessage(error);
          setLoadingError(errorMessage);
        }
      });
  }, []);

  let content = null;
  if (loading && loadingError) {
    content = <div>Error: {loadingError}</div>;
  } else if (loading) {
    content = <div>Loading ...</div>;
  } else {
    content = (
      <Routes>
        <Route path="/" element={<LoggedInContainer />} />
        <Route path="auth/*" element={<AuthenticationContainer />} />
      </Routes>
    );
  }

  return (
    <AppContext.Provider value={{ appState, appStateDispatch }}>
      <BrowserRouter>{content}</BrowserRouter>
    </AppContext.Provider>
  );
}
