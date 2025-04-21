// client/src/pages/DashboardPage.tsx
import React, { useContext } from "react";
import { EmailContext } from "../App";

const DashboardPage: React.FC = () => {
  const { email } = useContext(EmailContext);
  return (
    <div style={{ padding: "20px" }}>
      <h1>Welcome to Dashboard</h1>
      <p>This is a protected route.</p>
      <p>
        <strong>Logged in as:</strong> {email}
      </p>
    </div>
  );
};

export default DashboardPage;
