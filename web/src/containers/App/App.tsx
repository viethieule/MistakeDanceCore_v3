import React, { useEffect, useReducer, useState } from "react";
import axios from "axios";

interface IAppState {
  jwtAccessToken: string;
}

enum AppActionType {
  RefreshToken = "RefreshToken",
}

type AppAction = { type: AppActionType.RefreshToken; jwtAccessToken: string };

const initialAppState: IAppState = {
  jwtAccessToken: "",
};

function appStateReducer(state: IAppState, action: AppAction): IAppState {
  switch (action.type) {
    case AppActionType.RefreshToken:
      return {
        ...state,
        jwtAccessToken: action.jwtAccessToken,
      };
    default:
      throw new Error("Invalid action type");
  }
}

export function App() {
  const [appState, dispatch] = useReducer(appStateReducer, {
    ...initialAppState,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    axios
      .get("/authentication/refreshtoken")
      .then((response) => {
        const { jwtAccessToken } = response.data;
      })
      .catch((error) => {});
  }, []);

  return <></>;
}
