import axios from "../../axios";
import { IDropdownOption } from "./IDropdownOption";

export const getBranchOptions = async (axiosConfig: any): Promise<IDropdownOption[]> => {
    const response = await axios.get("api/Branch/DropdownOptions", axiosConfig);
    return response.data.options;
}