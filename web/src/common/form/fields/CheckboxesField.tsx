import React from 'react';
import { FormDataActionType, useFormContext } from '../FormContext';
import { IFieldProps } from './IFieldProps';

interface ICheckboxFieldProps extends IFieldProps {
    options: ICheckboxOption[]
}

interface ICheckboxOption {
    label: string;
    value: string;
}

export const CheckboxesField: React.FC<ICheckboxFieldProps> = ({
    label, name, onChange = () => { }, options
}) => {
    const { formData, formDataDispatch } = useFormContext();
    const checkboxValues: string[] = formData.values[name];
    const handleChange = (event: any) => {
        const newCheckboxValues = checkboxValues ? [ ...checkboxValues ] : [];
        const value = event.target.value;
        if (event.target.checked) {
            newCheckboxValues.push(value);
        } else {
            const index = newCheckboxValues.indexOf(value);
            if (index > -1) {
                newCheckboxValues.splice(index, 1);
            }
        }
        formDataDispatch({ type: FormDataActionType.SetValues, newValues: { [name]: newCheckboxValues } });
        onChange();
    }
    return (
        <div>
            <label>{label}</label>
            {options.map(option => (
                <span key={option.value}>
                    <input
                        type="checkbox"
                        name={name}
                        value={option.value}
                        onChange={handleChange}
                        checked={checkboxValues.some(value => value === option.value)}
                    />
                    <label>{option.label}</label>
                </span>
            ))}
        </div>
    )
}
