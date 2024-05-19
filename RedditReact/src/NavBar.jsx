import React from 'react';
import { NavLink } from 'react-router-dom';
import './NavBar.css'; // Import your CSS file for styling

const NavBar = () => {
    return (
        <nav className="navbar">
            <NavLink to="/" className="nav-link" activeClassName="active-link">Home</NavLink>
            <NavLink to="/create/topic" className="nav-link" activeClassName="active-link">Create Topic</NavLink>
            <NavLink to="/profile" className="nav-link" activeClassName="active-link">Profile</NavLink>
        </nav>
    );
};

export default NavBar;
