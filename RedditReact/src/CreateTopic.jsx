import React, {useState} from 'react';
import { useNavigate } from 'react-router-dom';
import axiosInstance from './axiosInstance';
import './CreateTopic.css';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
const { v4: uuidv4} = require('uuid');

function CreateTopic() {
    const [title, setTitle] = useState('');
    const [content, setContent] = useState('');
    //const [userId, setUserId] = useState('');

    const navigate = useNavigate();

    /*useEffect(() => {
        // Extract token from local storage
        const token = localStorage.getItem('token');
        if (token) {
            // Decode token to get user ID
            const decodedToken = jwtDecode(token);
            setUserId(decodedToken.sub); // Adjust according to your token structure
        }
    }, []);*/

    const handleSubmit = async (e) => {
        e.preventDefault();
        const payload = {
            Title: title,
            Content: content,
            Id: uuidv4(),
            UserId: userId
        };

        try {
            const endpoint = `http://localhost/post/create`;
            const response = await axiosInstance.post(endpoint, payload);
            console.log('RESPONSE');
            console.log(response);
            console.log(response.status);
            if (response.status === 200) {
                navigate("/");
                toast(response.data);
            }
        } catch (error) {
            toast("Error Happened");
            console.error(`Error creating topic: `, error);
        }
    };
    return (
        <div className="create-topic-container">
            <ToastContainer position="top-right" autoClose={2000} hideProgressBar={false} newestOnTop={false} closeOnClick rtl={false} pauseOnFocusLoss draggable pauseOnHover />
            <h1>Create a New Topic</h1>
            <form onSubmit={handleSubmit} className="create-topic-form">
                <div className="form-group">
                    <label htmlFor="title">Title</label>
                    <input type="text" value={title} onChange={(e) => setTitle(e.target.value)} id="title" placeholder="Title" className="form-control" />
                </div>
                <div className="form-group">
                    <label htmlFor="content">Content</label>
                    <textarea value={content} onChange={(e) => setContent(e.target.value)} id="content" placeholder="Content" className="form-control"></textarea>
                </div>
                <button type="submit" className="submit-button">Create Topic</button>
            </form>
        </div>
    );
}

export default CreateTopic;