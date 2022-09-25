import moment from "moment";
import { FieldDataType, IFieldDefs } from "./FieldDefs";
import { IDictionary } from "./FormContext";
import _ from "lodash";

export const serializeForm = (fieldValues: IDictionary, fieldDefs: IFieldDefs): IDictionary => {
    const model: IDictionary = {};

    for (let fieldName in fieldDefs) {
        let fieldValue = fieldValues[fieldName];
        const fieldDef = fieldDefs[fieldName];
        switch (fieldDef.dataType) {
            case FieldDataType.Text:
            case FieldDataType.Dropdown:
            case FieldDataType.Time:
            case FieldDataType.Checkboxes:
                break;
            case FieldDataType.Integer:
                if (fieldValue) {
                    fieldValue = parseInt(fieldValue);
                }
                break;
            case FieldDataType.DateTime:
                if (fieldValue) {
                    fieldValue = moment(fieldValue).format();
                }
                break;
        }
        _.set(model, fieldDef.dataPath, fieldValue);
    }

    return model;
}