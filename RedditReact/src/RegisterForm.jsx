import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from 'react-router-dom';
import axiosInstance from "./axiosInstance";

function RegisterForm() {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [pass, setPass] = useState('');
  const [country, setCountry] = useState('');
  const [city, setCity] = useState('');
  const [address, setAddress] = useState('');
  const [number, setNumber] = useState('');
  const [image, setImage] = useState('');
  const navigate = useNavigate();


  const handleImageUploaded = (e) => {
    const file = e.target.files[0];
    const reader = new FileReader();
    reader.onloadend = () => {
      setImage(reader.result);
    }
    reader.readAsDataURL(file);
  }

  const handleSubmit = async (e) => {
      e.preventDefault();
      const userData = {
        Email: email,
        Password: pass,
        FirstName: firstName,
        LastName: lastName,
        Country: country,
        City: city,
        Address: address,
        Phone: number,
        Image: image,
      };

      try {
          const response = await axiosInstance.post("auth/register", {...userData});
          navigate("/login")
      } catch (error) {
          console.error("There was an error!", error);
      }
  };  

  const navigateToLogin = () => {
    navigate('/login');
};
  
  return (
    <div className="App">
    <div className="auth-form-container">
        <h2>Register</h2>
        <form className="register-form" onSubmit={handleSubmit}>
            <label htmlFor="firstname">First Name</label>
            <input value={firstName} onChange={(e) => setFirstName(e.target.value)} id="firstname" placeholder="First Name" />

            <label htmlFor="lastname">Last Name</label>
            <input value={lastName} onChange={(e) => setLastName(e.target.value)} id="lastname" placeholder="Last Name" />

            <label htmlFor="email">Email</label>
            <input value={email} onChange={(e) => setEmail(e.target.value)} type="email" placeholder="youremail@gmail.com" id="email" />

            <label htmlFor="password">Password</label>
            <input value={pass} onChange={(e) => setPass(e.target.value)} type="password" placeholder="********" id="password" />

            <label htmlFor="country">Country</label>
            <input value={country} onChange={(e) => setCountry(e.target.value)} id="country" placeholder="Country" />

            <label htmlFor="city">City</label>
            <input value={city} onChange={(e) => setCity(e.target.value)} id="city" placeholder="City" />

            <label htmlFor="address">Address</label>
            <input value={address} onChange={(e) => setAddress(e.target.value)} id="address" placeholder="Address" />

            <label htmlFor="number">Phone Number</label>
            <input value={number} onChange={(e) => setNumber(e.target.value)} id="number" placeholder="Phone Number" />

            <label htmlFor="image">Image</label>
            <input type="file" onChange={handleImageUploaded} />

            <button type="submit">Register</button>
        </form>
        <button className="link-btn" onClick={navigateToLogin}>Already have an account? Login here.</button>
    </div>
    </div>
);
}

export default RegisterForm;
