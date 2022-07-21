import React from "react";
import { Outlet, Route, Routes } from "react-router-dom";
import Login from "../../pages/Login/Login";

export default function AuthenticationContainer() {
  return (
    <>
      <Routes>
        <Route path="login" element={<Login />} />
      </Routes>
      <Outlet />
    </>
  );
}
