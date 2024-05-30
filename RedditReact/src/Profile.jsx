import React, { useState, useEffect } from 'react';
import axiosInstance from './axiosInstance';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './Profile.css'; // Make sure you have this CSS file

function Profile() {
    const [profile, setProfile] = useState({
        first_name: '',
        last_name: '',
        country: '',
        address: '',
        city: '',
        phone: ''
    });

    useEffect(() => {
        axiosInstance.get('/profile').then(response => {
            setProfile(response.data.profile);
        }).catch(error => {
            console.error("Error fetching profile: ", error);
            toast("Error fetching profile");
        });
    }, []);

    const handleInputChange = (e) => {
        setProfile({
            ...profile,
            [e.target.name]: e.target.value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axiosInstance.post('/profile', profile);
            if (response.status === 200) {
                toast("Profile updated successfully");
            }
        } catch (error) {
            toast("Error updating profile");
            console.error("Error updating profile: ", error);
        }
    };

    const labelMap = {
        first_name: 'First Name',
        last_name: 'Last Name',
        country: 'Country',
        address: 'Address',
        city: 'City',
        phone: 'Phone'
    };

    return (
        <div className="profile-container">
            <ToastContainer position="top-right" autoClose={2000} />
            <h1>Edit Profile</h1>
            <form onSubmit={handleSubmit} className="profile-form">
                {Object.entries(profile).map(([key, value]) => (
                    <div key={key} className="form-group">
                        <label htmlFor={key}>{labelMap[key] || key.replace('_', ' ')}</label>
                        <input
                            type="text"
                            id={key}
                            name={key}
                            value={value}
                            onChange={handleInputChange}
                            className="form-control"
                            disabled={(key === "Id" || key === "userId")}
                        />
                    </div>
                ))}
                <button type="submit" className="submit-button">Update Profile</button>
            </form>
        </div>
    );
}

export default Profile;
