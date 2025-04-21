// client/src/App.tsx
import React, { createContext, useEffect, useState } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import LoginPage from "./pages/LoginPage";
import DashboardPage from "./pages/DashboardPage";
import SessionTimeoutHandler from "./components/SessionTimeoutHandler";

export const EmailContext = createContext<{
  email: string;
  setEmail: React.Dispatch<React.SetStateAction<string>>;
}>({
  email: "",
  setEmail: () => {},
});

const App: React.FC = () => {
  const [email, setEmail] = useState(() => {
    return localStorage.getItem("email") || "";
  });

  useEffect(() => {
    if (email) {
      localStorage.setItem("email", email);
    }
  }, [email]);
  return (
    <Router>
      <>
        <EmailContext.Provider value={{ email, setEmail }}>
          <SessionTimeoutHandler />
          <Routes>
            <Route path="/" element={<LoginPage />} />
            <Route path="/dashboard" element={<DashboardPage />} />
          </Routes>
        </EmailContext.Provider>
      </>
    </Router>
  );
};

export default App;
