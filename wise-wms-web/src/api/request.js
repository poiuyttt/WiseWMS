import axios from "axios";
import { ElMessage } from "element-plus";
import { useUserStore } from "@/stores/user";

const request = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "https://localhost:7176",
  timeout: 10000,
});

request.interceptors.request.use((config) => {
  const token = useUserStore().token;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

request.interceptors.response.use(
  (res) => res.data,
  (err) => {
    if (err.response?.status === 401) {
      useUserStore().logout();
      window.location.href = "/#/login";
    }
    ElMessage.error(err.response?.data?.message || "请求失败");
    return Promise.reject(err);
  },
);

export default request;
