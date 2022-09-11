import axios, { acquireAxiosConfig } from "../../axios";
import { IDropdownOption } from "./IDropdownOption";

export const getClassOptions = async (axiosConfig: any): Promise<IDropdownOption[]> => {
    const response = await axios.get("api/Class/DropdownOptions", axiosConfig);
    return response.data.options;
}