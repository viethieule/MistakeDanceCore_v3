import axios, { acquireAxiosConfig } from "../../axios";
import React, { useState } from "react";
import { useEffect } from "react";
import moment from "moment";
import { useAppContext } from "../../common/AppContext";
import { getErrorMessage } from "../../utils/Error";
import { IDropdownOption } from "../../common/dropdowns/IDropdownOption";
import { getClassOptions } from "../../common/dropdowns/Class";
import { getTrainerOptions } from "../../common/dropdowns/Trainer";
import { getBranchOptions } from "../../common/dropdowns/Branch";

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

  // Options
  const [classOptions, setClassOptions] = useState(new Array<IDropdownOption>());
  const [trainerOptions, setTrainerOptions] = useState(new Array<IDropdownOption>());
  const [branchOptions, setBranchOptions] = useState(new Array<IDropdownOption>());

  const { appState, appStateDispatch } = useAppContext();

  useEffect(() => {
    const fetchSessions = async () => {
      setLoading(true);
      try {
        const axiosConfig = await acquireAxiosConfig(appState, appStateDispatch);

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

  useEffect(() => {
    if (!showScheduleForm) {
      return;
    }

    const fetchOptions = async () => {
      try {
        const axiosConfig = await acquireAxiosConfig(appState, appStateDispatch);
        const [classOptionsRs, trainerOptionsRs, branchOptionsRs] = await Promise.all([
          getClassOptions(axiosConfig),
          getTrainerOptions(axiosConfig),
          getBranchOptions(axiosConfig)
        ]);

        setClassOptions(classOptionsRs);
        setTrainerOptions(trainerOptionsRs);
        setBranchOptions(branchOptionsRs);
      } catch (error) {
        console.log(error);
      }
    }

    fetchOptions();
  }, [showScheduleForm])


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

  const handleOnSubmit = (event: any) => {
    event.preventDefault();
    console.log(event);
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
          <button key={dayOfWeek.date()}>{dayOfWeek.date()}</button>
        ))}
        <button onClick={handleNextClick}>Next</button>
        {' '}
        <button onClick={handleCreateScheduleClick}>Create</button>
      </div>
    )

    const scheduleForm = (
      <div>
        <form>
          <div>
            <label>Lớp</label>
            <select name="class" id="class">
              {classOptions.map(option => <option key={option.value} value={option.value}>{option.text}</option>)}
            </select>
          </div>
          <div>
            <label>Bài múa</label>
            <input type="text" name="song" id="song" />
          </div>
          <div>
            <label>Ngày bắt đầu</label>
            <input type="date" name="openingDate" id="openingDate" />
          </div>
          <div>
            <label>Giờ bắt đầu</label>
            <input type="time" name="startTime" id="startTime" />
          </div>
          <div>
            <label>Các buổi / tuần</label>
            <input type="checkbox" name="dayOfWeek1" id="dayOfWeek1" value="1" />
            <label>Thứ 2</label>
            <input type="checkbox" name="dayOfWeek2" id="dayOfWeek2" value="2" />
            <label>Thứ 3</label>
            <input type="checkbox" name="dayOfWeek3" id="dayOfWeek3" value="3" />
            <label>Thứ 4</label>
            <input type="checkbox" name="dayOfWeek4" id="dayOfWeek4" value="4" />
            <label>Thứ 5</label>
            <input type="checkbox" name="dayOfWeek5" id="dayOfWeek5" value="5" />
            <label>Thứ 6</label>
            <input type="checkbox" name="dayOfWeek6" id="dayOfWeek6" value="6" />
            <label>Thứ 7</label>
            <input type="checkbox" name="dayOfWeek0" id="dayOfWeek0" value="0" />
            <label>Chủ Nhật</label>
          </div>
          <div>
            <label>Tổng số buổi</label>
            <input type="number" name="totalSession" id="totalSession" />
          </div>
          <div>
            <label>Giáo viên</label>
            <select name="trainer" id="trainer">
              {trainerOptions.map(option => <option key={option.value} value={option.value}>{option.text}</option>)}
            </select>
          </div>
          <div>
            <label>Chi nhánh</label>
            <select name="branch" id="branch">
              {branchOptions.map(option => <option key={option.value} value={option.value}>{option.text}</option>)}
            </select>
          </div>
          <div>
            <button onClick={handleOnSubmit}>Tạo</button>
          </div>
        </form>
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
