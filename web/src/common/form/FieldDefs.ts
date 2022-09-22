export interface IFieldDefs {
    [name: string]: IFieldDef
}

export interface IFieldDef {
    dataType: FieldDataType;
    dataPath: string;
}

export enum FieldDataType {
    Text = "Text",
    Integer = "Integer",
    Dropdown = "Dropdown",
    Checkboxes = "Checkboxes",
    DateTime = "DateTime",
    Time = "Time"
}

