import React from 'react';
import { FormDataActionType, useFormContext } from '../FormContext';
import { IFieldProps } from './IFieldProps';

interface IDateFieldProps extends IFieldProps { }

export const DateField: React.FC<IDateFieldProps> = ({
    label, name, onChange = () => {}
}) => {
    const { formData, formDataDispatch } = useFormContext();
    const value = formData.values[name];
    const handleChange = (event: any) => {
        formDataDispatch({ type: FormDataActionType.SetValues, newValues: { [name]: event.target.value } });
        onChange();
    }
    return (
        <div>
            <label>{label}</label>
            <input type="date" name={name} id={name} onChange={handleChange} value={value} />
        </div>
    )
}
