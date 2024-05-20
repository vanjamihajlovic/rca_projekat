import React, {useState} from 'react';
import { useNavigate } from 'react-router-dom';
import axiosInstance from './axiosInstance';
import './CreateTopic.css';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

function CreateTopic() {
    const [title, setTitle] = useState('');
    const [content, setContent] = useState('');

    const navigate = useNavigate();
    const handleSubmit = async (e) => {
        e.preventDefault();
        const payload = {
            title,
            content,
        };

        try {
            const endpoint = `/topic/create`;
            const response = await axiosInstance.post(endpoint, payload);
            if (response.status === 200) {
                navigate("/");
                toast(response.data.message);
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