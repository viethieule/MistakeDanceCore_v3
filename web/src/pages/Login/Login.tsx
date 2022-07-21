import axios from "../../axios";
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppActionType, useAppContext } from "../../common/AppContext";
import { getErrorMessage } from "../../utils/Error";

export default function Login() {
  const { appStateDispatch } = useAppContext();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const handleUsernameChange = (event: any) => {
    setUsername(event.target.value);
  };
  const handlePasswordChange = (event: any) => {
    setPassword(event.target.value);
  };

  const [loading, setLoading] = useState(false);
  const [loadingError, setLoadingError] = useState("");

  const navigate = useNavigate();

  const login = (event: any) => {
    event.preventDefault();
    setLoading(true);
    axios
      .post("/api/authentication/login", { username, password }, { withCredentials: true })
      .then((response) => {
        const { jwtAccessToken, user } = response.data;
        appStateDispatch({ type: AppActionType.Login, user, jwtAccessToken });
        navigate("/");
      })
      .catch((error) => {
        const errorMessage = getErrorMessage(error);
        setLoadingError(errorMessage);
      })
      .finally(() => {
        setLoading(false);
      });
  };

  return (
    <>
      <form>
        <input
          type="text"
          placeholder="username"
          onChange={handleUsernameChange}
        />
        <input
          type="password"
          placeholder="password"
          onChange={handlePasswordChange}
        />
        <button onClick={login}>{!loading ? "Login" : "Logging in..."}</button>
      </form>
      {loadingError ? <div>{loadingError}</div> : null}
    </>
  );
}
