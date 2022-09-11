import axios from "../../axios";
import { IDropdownOption } from "./IDropdownOption";

export const getTrainerOptions = async (axiosConfig: any): Promise<IDropdownOption[]> => {
    const response = await axios.get("api/Trainer/DropdownOptions", axiosConfig);
    return response.data.options;
}