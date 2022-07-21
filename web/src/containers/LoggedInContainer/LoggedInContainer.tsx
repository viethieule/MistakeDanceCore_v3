import React from "react";
import { Routes, Route } from "react-router-dom";
import { Schedule } from "../../pages/Schedule/Schedule";

export default function LoggedInContainer() {
  return (
    <>
      <Routes>
        <Route path="/" element={<Schedule />} />
        <Route path="calendar" element={<Schedule />} />
      </Routes>
    </>
  );
}
