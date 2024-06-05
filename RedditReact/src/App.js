import React from "react";
import PropTypes from 'prop-types';
import './App.css';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import LoginForm from './LoginForm';
import RegisterForm from './RegisterForm';
import TopicPage from "./TopicPage";
import HomePage from "./HomePage";
import CreateTopic from "./CreateTopic";
import Cookies from 'js-cookie';
import Profile from "./Profile";
import NavBar from "./NavBar";

function App() {

  const checkAuth = () => {
    const token = Cookies.get('jwt-token');
    return token && token !== '';
  };

  const ProtectedRoute = ({ children }) => {
    if (!checkAuth()) {
      return <Navigate to="/login" />;
    }
    return children;
  };

  ProtectedRoute.propTypes = {
    children: PropTypes.node.isRequired,
  };

  return (
    <Router>
      <NavBar />
      <Routes>
        <Route path="/" element={<ProtectedRoute><HomePage /></ProtectedRoute>} />
        <Route path="/login" element={<LoginForm/>}/>
        <Route path="/register" element={<RegisterForm/>}/>
        <Route path="/create/topic" element={<ProtectedRoute><CreateTopic/></ProtectedRoute>} />
        <Route path="/topic/:topicId" element={<ProtectedRoute><TopicPage/></ProtectedRoute>} />
        <Route path="/profile" element={<ProtectedRoute><Profile/></ProtectedRoute>}/>
      </Routes>
    </Router>
  );
}


export default App;


