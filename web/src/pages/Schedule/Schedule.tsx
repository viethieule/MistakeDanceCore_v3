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
import { FormContext, FormDataActionType, FormMode, IFormDef, useForm } from "../../common/form/FormContext";
import { DropdownField } from "../../common/form/fields/DropdownField";
import { TextField } from "../../common/form/fields/TextField";
import { DateField } from "../../common/form/fields/DateField";
import { TimeField } from "../../common/form/fields/TimeField";
import { CheckboxesField } from "../../common/form/fields/CheckboxesField";
import { IntegerField } from "../../common/form/fields/IntegerField";
import { FieldDataType } from "../../common/form/FieldDefs";
import { serializeForm } from "../../common/form/FormSerializer";

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

const scheduleFormDef: IFormDef = {
  fields: {
    id: {
      dataType: FieldDataType.Integer,
      dataPath: "schedule.id"
    },
    class: {
      dataType: FieldDataType.Dropdown,
      dataPath: "schedule.classId"
    },
    song: {
      dataType: FieldDataType.Text,
      dataPath: "schedule.song"
    },
    openingDate: {
      dataType: FieldDataType.DateTime,
      dataPath: "schedule.openingDate"
    },
    startTime: {
      dataType: FieldDataType.Time,
      dataPath: "schedule.startTime"
    },
    daysPerWeek: {
      dataType: FieldDataType.Checkboxes,
      dataPath: "schedule.daysPerWeek"
    },
    totalSessions: {
      dataType: FieldDataType.Integer,
      dataPath: "schedule.totalSessions"
    },
    trainer: {
      dataType: FieldDataType.Dropdown,
      dataPath: "schedule.trainerId"
    },
    branch: {
      dataType: FieldDataType.Dropdown,
      dataPath: "schedule.branchId"
    }
  }
}

interface IRegistrationListProps {
  sessionId: number
}
interface IRegistration {
  id: number;
  memberId: number
  memberFullName: string;
  status: number;
}
const RegistrationList: React.FC<IRegistrationListProps> = ({ sessionId }) => {
  const [registrations, setRegistrations] = useState<IRegistration[]>([]);
  const { appState, appStateDispatch } = useAppContext();
  useEffect(() => {
    const fetchRegistrations = async () => {
      const axiosConfig = await acquireAxiosConfig(appState, appStateDispatch);
      const response = await axios.post("api/Registration/ListBySessionId", { sessionId }, axiosConfig);
      const { registrations } = response.data;
      setRegistrations(registrations);
    }

    fetchRegistrations();
  }, [sessionId])

  return (
    <div>
      {registrations.map((registration, index) => (
        <div key={registration.id}>{+index + 1} {registration.memberFullName}</div>
      ))}
    </div>
  )
}

export const Schedule = () => {
  const [timetable, setTimetable] = useState<ITimetableRow[]>([]);
  const [daysOfWeek, setDaysOfWeek] = useState<moment.Moment[]>(getDaysOfCurrentWeek());
  const [loading, setLoading] = useState(true);
  const [saveScheduleComplete, setSaveScheduleComplete] = useState(false);
  const [loadingError, setLoadingError] = useState("");
  const [showScheduleForm, setShowScheduleForm] = useState(false);
  const [session, setSession] = useState<ISession | null>(null);

  const scheduleFormContext = useForm(scheduleFormDef);
  const { formMode, formModeDispatch, formData, formDataDispatch } = scheduleFormContext;

  // Options
  const [classOptions, setClassOptions] = useState(new Array<IDropdownOption>());
  const [trainerOptions, setTrainerOptions] = useState(new Array<IDropdownOption>());
  const [branchOptions, setBranchOptions] = useState(new Array<IDropdownOption>());

  const { appState, appStateDispatch } = useAppContext();

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

  useEffect(() => {
    fetchSessions();
  }, [daysOfWeek]);

  useEffect(() => {
    if (saveScheduleComplete) {
      fetchSessions();
    }
  }, [saveScheduleComplete])

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
    formModeDispatch(FormMode.Create);
    formDataDispatch({ type: FormDataActionType.ClearValues });
    setShowScheduleForm(true);
  }

  const handleOnSubmit = async (event: any) => {
    // TODO: can remove preventDefault() ?
    event.preventDefault();
    const model = serializeForm(formData.values, scheduleFormDef.fields);
    console.log(model);
    setLoading(true);
    setSaveScheduleComplete(false);
    try {
      const axiosConfig = await acquireAxiosConfig(appState, appStateDispatch);
      const url = formMode === FormMode.Create ? "api/Schedule/Create" : "api/Schedule/Update";
      const response = await axios.post(url, model, axiosConfig);
      const { schedule } = response.data;
      if (schedule) {
        setSaveScheduleComplete(true);
      }
    } catch (error) {
      console.log(error);
    } finally {
      setLoading(false);
    }
  }

  const handleEmptySlotClick = (selectStartTime: string, selectedDayOfWeek: number) => {
    formModeDispatch(FormMode.Create);
    formDataDispatch({ type: FormDataActionType.ClearValues });
    const openingDate = daysOfWeek.find(day => day.day() === selectedDayOfWeek);
    const newValues = {
      startTime: selectStartTime,
      daysPerWeek: [selectedDayOfWeek],
      openingDate
    }
    formDataDispatch({ type: FormDataActionType.SetValues, newValues });
    setShowScheduleForm(true);
  }

  const handleSessionClick = (session: ISession) => {
    setSession(session);
  }

  const handleEditScheduleClick = () => {
    if (!session) {
      return;
    }

    formModeDispatch(FormMode.Edit);
    formDataDispatch({ type: FormDataActionType.ClearValues });

    const newValues = {
      id: session.scheduleId,
      class: session.classId,
      song: session.song,
      openingDate: session.openingDate,
      startTime: session.startTime,
      daysPerWeek: session.daysPerWeek,
      totalSessions: session.totalSessions,
      trainer: session.trainerId,
      branch: session.branchId
    }
    formDataDispatch({ type: FormDataActionType.SetValues, newValues });
    setShowScheduleForm(true);
  }

  const handleCloseSessionDetails = () => setSession(null);

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
          <FormContext.Provider value={scheduleFormContext}>
            <DropdownField label="Lớp" name="class" options={classOptions} />
            <TextField label="Bài múa" name="song" />
            <DateField label="Ngày bắt đầu" name="openingDate" />
            <TimeField label="Giờ bắt đầu" name="startTime" />
            <CheckboxesField
              label="Các buổi / tuần"
              name="daysPerWeek"
              options={[
                { label: 'Thứ 2', value: "1" },
                { label: 'Thứ 3', value: "2" },
                { label: 'Thứ 4', value: "3" },
                { label: 'Thứ 5', value: "4" },
                { label: 'Thứ 6', value: "5" },
                { label: 'Thứ 7', value: "6" },
                { label: 'Chủ nhật', value: "0" },
              ]}
            />
            <IntegerField label="Tổng số buổi" name="totalSessions" />
            <DropdownField label="Giáo viên" name="trainer" options={trainerOptions} />
            <DropdownField label="Chi nhánh" name="branch" options={branchOptions} />
            <div>
              <button onClick={handleOnSubmit}>Lưu</button>
            </div>
            <div>
              Form mode: {formMode}
            </div>
          </FormContext.Provider>
        </form>
      </div>
    )

    const sessionDetails = session && (
      <div>
        <div>
          <h2>Thông tin buổi học</h2>
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
        <div>
          <h2>Danh sách đăng ký</h2>
          <RegistrationList sessionId={session.id} />
        </div>
        <div>
          <button onClick={handleEditScheduleClick}>Sửa</button>
          <button onClick={handleCloseSessionDetails}>Đóng</button>
        </div>
      </div>
    )

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
              <td
                key={cell.dayOfWeek}
                onClick={() => !cell.sessions.length && handleEmptySlotClick(row.startTime, cell.dayOfWeek)}
              >
                {cell.sessions.map(session => (
                  <div key={session.id} onClick={() => handleSessionClick(session)}>
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
        {session && sessionDetails}
        {schedule}
      </div>
    );
  }

  return <div>{content}</div>;
};
