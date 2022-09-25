import { type } from '@testing-library/user-event/dist/type';
import moment from 'moment';
import React from 'react';
import { FormDataActionType, useFormContext } from '../FormContext';
import { IFieldProps } from './IFieldProps';

interface IDateFieldProps extends IFieldProps { }

export const DateField: React.FC<IDateFieldProps> = ({
    label, name, onChange = () => {}
}) => {
    const { formData, formDataDispatch } = useFormContext();
    let value = formData.values[name];
    if (value) {
        value = moment(value).format('YYYY-MM-DD');
    }
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
