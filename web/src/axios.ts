import axios from "axios";

const instance = axios.create({
  baseURL: "https://localhost:7171",
});

export const buildAxiosConfig = (jwtAccessToken: string): any => {
  return {
    headers: { Authorization: `Bearer ${jwtAccessToken}` }
  }
}

export default instance;
