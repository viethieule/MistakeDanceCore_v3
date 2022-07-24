import axios, { acquireAxiosConfig } from "../../axios";
import React, { useState } from "react";
import { useEffect } from "react";
import moment from "moment";
import { useAppContext } from "../../common/AppContext";
import { getErrorMessage } from "../../utils/Error";

export interface ISession {
  id: number;
  date: Date;
  number: number;
  scheduleId: number;
  song: string;
  openingDate: Date;
  daysPerWeek: number[];
  totalSessions: number | null;
  startTime: string;
  branchId: number;
  branchName: string;
  classId: number;
  className: string;
  trainerId: number;
  trainerName: string;
  totalRegistered: number;
}

export const Schedule = () => {
  const [sessions, setSessions] = useState<ISession[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadingError, setLoadingError] = useState("");
  const { appState, appStateDispatch } = useAppContext();

  useEffect(() => {
    const fetchSessions = async () => {
      try {
        const firstDateOfCurrentWeek = moment(new Date())
          .clone()
          .startOf("isoWeek");

        const axiosConfig = await acquireAxiosConfig(
          appState,
          appStateDispatch
        );

        const response = await axios.post(
          "api/Session/GetByDateRange",
          { start: firstDateOfCurrentWeek },
          axiosConfig
        );

        const { sessions } = response.data;
        setSessions(sessions);
        setLoading(false);
      } catch (error: any) {
        console.log(error);
        const errorMessage = getErrorMessage(error);
        setLoadingError(errorMessage);
      }
    };
    fetchSessions();
  }, []);

  let content = null;
  if (loading && loadingError) {
    content = `Error: ${loadingError}`;
  } else if (loading) {
    content = "Loading...";
  } else {
    content = (
      <div>
        {sessions.map((session) => (
          <div key={session.id}>
            <p>Id: {session.id}</p>
            <p>Date: {session.date.toString()}</p>
            <p>Number: {session.number}</p>
            <p>ScheduleId: {session.scheduleId}</p>
            <p>Song: {session.song}</p>
            <p>Opening date: {session.openingDate.toString()}</p>
            <p>Days per week: {session.daysPerWeek.join(",")}</p>
            <p>Total sessions: {session.totalSessions}</p>
            <p>Start time: {session.startTime}</p>
            <p>Branch: {session.branchName}</p>
            <p>Trainer: {session.trainerName}</p>
            <p>Class: {session.className}</p>
          </div>
        ))}
      </div>
    );
  }

  return <div>{content}</div>;
};
