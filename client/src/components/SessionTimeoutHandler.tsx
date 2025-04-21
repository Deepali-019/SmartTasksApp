import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./SessionTimeoutHandler.module.css";

// Function to decode JWT token
const decodeJwt = (token: string) => {
  try {
    const parts = token.split(".");
    if (parts.length !== 3) {
      throw new Error("Invalid JWT token");
    }

    const payload = parts[1];
    const decodedPayload = atob(payload);
    return JSON.parse(decodedPayload);
  } catch (error) {
    console.error("Failed to decode JWT:", error);
    return null;
  }
};

const SessionTimeoutHandler: React.FC = () => {
  const [isExpired, setIsExpired] = useState(false);
  const navigate = useNavigate();

  // Function to check session expiration
  const checkSessionExpiration = () => {
    const token = localStorage.getItem("token");
    if (token) {
      const decodedToken = decodeJwt(token);
      if (decodedToken) {
        const expTime = decodedToken.exp * 1000; // Convert exp to milliseconds
        const currentTime = Date.now();
        if (expTime < currentTime) {
          setIsExpired(true);
        }
      }
    }
  };

  // Function to handle session expired popup "OK" button click
  const handleOk = () => {
    localStorage.removeItem("token");
    setIsExpired(false);
    navigate("/");
  };

  useEffect(() => {
    const intervalId = setInterval(() => {
      checkSessionExpiration();
    }, 60000); // 1-minute interval

    return () => clearInterval(intervalId);
  }, []);

  // If session has not expired, render nothing
  if (!isExpired) return null;

  return (
    <div className={styles.modalStyle}>
      <div className={styles.modalBoxStyle}>
        <h3>Session Expired</h3>
        <p>Your session has expired. Please log in again.</p>
        <button onClick={handleOk}>OK</button>
      </div>
    </div>
  );
};

export default SessionTimeoutHandler;
