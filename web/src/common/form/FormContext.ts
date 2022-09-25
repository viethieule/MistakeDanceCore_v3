import React, { useState } from "react";
import { useReducer } from "react";
import { IFieldDefs } from "./FieldDefs";

export interface IDictionary {
    [name: string]: any;
}

export interface IFormData {
    values: IDictionary;
}

export interface IFormDef {
    fields: IFieldDefs
}

const blankFormState: IFormData = {
    values: {}
}

export enum FormDataActionType {
    SetValues = "SetValues",
    ClearValues = "ClearValues"
}

export type FormDataAction =
    | { type: FormDataActionType.SetValues, newValues: IDictionary }
    | { type: FormDataActionType.ClearValues }

const formDataReducer = (state: IFormData, action: FormDataAction): IFormData => {
    switch (action.type) {
        case FormDataActionType.SetValues:
            return { ...state, values: { ...state.values, ...action.newValues } };
        case FormDataActionType.ClearValues:
            return { ...state, values: {} };
        default:
            return state;
    }
}

export enum FormMode {
    Create = "Create",
    Edit = "Edit"
}

interface IFormContext {
    formDef: IFormDef;
    formMode: FormMode;
    formModeDispatch: React.Dispatch<FormMode>;
    formData: IFormData;
    formDataDispatch: React.Dispatch<FormDataAction>;
}

export const FormContext = React.createContext<IFormContext>({
    formDef: { fields: {} },
    formMode: FormMode.Create,
    formModeDispatch: (formMode: FormMode) => {}, 
    formData: blankFormState,
    formDataDispatch: (value: FormDataAction) => { }
})

export const useForm = (formDef: IFormDef): IFormContext => {
    const [formData, formDataDispatch] = useReducer(formDataReducer, blankFormState);
    const [formMode, formModeDispatch] = useState(FormMode.Create);
    return {
        formDef, formMode, formModeDispatch, formData, formDataDispatch
    };
}

export const useFormContext = () => React.useContext(FormContext);