import axios, { acquireAxiosConfig } from "../../axios";
import React, { useState } from "react";
import { useEffect } from "react";
import moment from "moment";
import { useAppContext } from "../../common/AppContext";
import { getErrorMessage } from "../../utils/Error";

export interface ITimetableRow {
  startTime: string;
  endTime: string;
  startToEnd: string;
  sessionCells: ITimetableCell[];
}

export interface ITimetableCell {
  dayOfWeek: number;
  dayOfWeekString: string;
  sessions: ISession[];
}

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

function getDaysOfCurrentWeek() {
  const currentDate = moment(new Date());
  const weekStart = currentDate.clone().startOf('isoWeek');

  const daysOfCurrentWeek = [];
  for (var i = 0; i <= 6; i++) {
    const date = moment(weekStart).add(i, 'days');
    daysOfCurrentWeek.push(date);
  }

  return daysOfCurrentWeek;
}

export const Schedule = () => {
  const [timetable, setTimetable] = useState<ITimetableRow[]>([]);
  const [daysOfWeek, setDaysOfWeek] = useState<moment.Moment[]>(getDaysOfCurrentWeek());
  const [loading, setLoading] = useState(true);
  const [loadingError, setLoadingError] = useState("");
  const [showScheduleForm, setShowScheduleForm] = useState(false);

  const { appState, appStateDispatch } = useAppContext();

  useEffect(() => {
    const fetchSessions = async () => {
      setLoading(true);
      try {
        const axiosConfig = await acquireAxiosConfig(
          appState,
          appStateDispatch
        );

        const response = await axios.post(
          "api/Session/Timetable",
          { start: daysOfWeek[0] },
          axiosConfig
        );

        const { timetable } = response.data;
        setTimetable(timetable);
        setLoading(false);
      } catch (error: any) {
        console.log(error);
        const errorMessage = getErrorMessage(error);
        setLoadingError(errorMessage);
      }
    };
    fetchSessions();
  }, [daysOfWeek]);

  const handlePreviousClick = () => {
    const previousDaysOfWeek = daysOfWeek.map(dayOfWeek => dayOfWeek.clone().subtract(7, 'days'));
    setDaysOfWeek(previousDaysOfWeek);
  }

  const handleNextClick = () => {
    const nextDaysOfWeek = daysOfWeek.map(dayOfWeek => dayOfWeek.clone().add(7, 'days'));
    setDaysOfWeek(nextDaysOfWeek);
  }

  const handleCreateScheduleClick = () => {
    setShowScheduleForm(true);
  }

  let content = null;
  if (loading && loadingError) {
    content = `Error: ${loadingError}`;
  } else if (loading) {
    content = "Loading...";
  } else {
    const controls = (
      <div>
        <button onClick={handlePreviousClick}>Prev</button>
        {daysOfWeek.map(dayOfWeek => (
          <button>{dayOfWeek.date()}</button>
        ))}
        <button onClick={handleNextClick}>Next</button>
        {' '}
        <button onClick={handleCreateScheduleClick}>Create</button>
      </div>
    )

    const scheduleForm = (
      <div>

      </div>
    );

    const timetableHeader = (
      <thead>
        <tr>
          <th></th>
          {daysOfWeek.map(dayOfWeek => (
            <th key={dayOfWeek.day()}>{dayOfWeek.format("ddd D/M")}</th>
          ))}
        </tr>
      </thead>
    )

    const timetableBody = (
      <tbody>
        {timetable.map(row => (
          <tr key={row.startToEnd}>
            <td>{row.startToEnd}</td>
            {row.sessionCells.map(cell => (
              <td key={cell.dayOfWeek}>
                {cell.sessions.map(session => (
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
              </td>
            ))}
          </tr>
        ))}
      </tbody>
    )

    const schedule = (
      <div>
        <table>
          {timetableHeader}
          {timetableBody}
        </table>
      </div>
    )

    content = (
      <div>
        {controls}
        {showScheduleForm && scheduleForm}
        {schedule}
      </div>
    );
  }

  return <div>{content}</div>;
};
