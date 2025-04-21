// // client/src/utils/axiosInstance.ts
// import axios from "axios";

// const axiosInstance = axios.create({
//   baseURL: "http://localhost:5173/api",
// });

// // Add token to every request
// axiosInstance.interceptors.request.use(
//   (config) => {
//     const token = localStorage.getItem("token");
//     if (token) {
//       //config.headers.Authorization = `Bearer ${token}`;
//       config.headers.Authorization = `${token}`;
//     }
//     return config;
//   },
//   (error) => Promise.reject(error)
// );

// export default axiosInstance;
