import React from 'react';
import { NavLink } from 'react-router-dom';
import './NavBar.css';
import Cookies from 'js-cookie';


const handleLogout = () => {
    Cookies.remove('jwt-token');
};

const NavBar = () => {
    return (
        <nav className="navbar">
            <NavLink to="/" className="nav-link">Home</NavLink>
            <NavLink to="/create/topic" className="nav-link">Create Topic</NavLink>
            <NavLink to="/profile" className="nav-link">Profile</NavLink>
            {Cookies.get('jwt-token') && <NavLink to="/login" className="nav-link logout-button" onClick={handleLogout}> Log Out </NavLink>}
        </nav>
    );
};

export default NavBar;
