import React from 'react';
import { FormDataActionType, useFormContext } from '../FormContext';
import { IDropdownOption } from '../../dropdowns/IDropdownOption';
import { IFieldProps } from './IFieldProps';

interface IDropdownFieldProps extends IFieldProps {
    options: IDropdownOption[];
}

export const DropdownField: React.FC<IDropdownFieldProps> = ({
    label, name, options, onChange = () => {}
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
            <select
                id={name}
                name={name}
                value={value}
                onChange={handleChange}
            >
                <option></option>
                {options.map(option => <option key={option.value} value={option.value}>{option.text}</option>)}
            </select>
        </div>
    )
}
