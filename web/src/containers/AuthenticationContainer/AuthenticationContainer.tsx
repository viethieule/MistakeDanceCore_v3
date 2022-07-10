import React from "react";
import { Route, Routes } from "react-router-dom";
import Login from "../../components/Login/Login";

export default function AuthenticationContainer() {
  return (
    <Routes>
      <Route path="auth/login">
        <Login />
      </Route>
    </Routes>
  );
}
