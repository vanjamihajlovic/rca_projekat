import axios from 'axios';
import Cookies from 'js-cookie';

const url = "http://localhost";
const port = "8080";
const usePort = false;

const axiosInstance = axios.create({
  baseURL: url + (usePort ? `:${port}` : ''),
  
});


axiosInstance.interceptors.request.use((config) => {
  const token = Cookies.get('jwt-token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
}, (error) => {
  return Promise.reject(error);
});

export default axiosInstance;
