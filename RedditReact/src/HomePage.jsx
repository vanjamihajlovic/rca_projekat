import React, { useState, useEffect } from 'react';
import axiosInstance from './axiosInstance';
import { useNavigate } from 'react-router-dom';
import './HomePage.css';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

function HomePage() {
    const navigate = useNavigate();
    const [topics, setTopics] = useState([]);
    const [showMyTopics, setShowMyTopics] = useState(false);
    const [sortCriteria, setSortCriteria] = useState(''); 
    const [searchQuery, setSearchQuery] = useState('');

    let userId;

    const navigateToCreateTopic = () => {
        navigate('/create/topic');
    };

    const navigateToTopic = (topicId) => {
        navigate(`/topic/${topicId}`);
    };

    const handleApiResponse = (updatedTopic, isDelete = false, deleteId = null) => {
        setTopics((prevTopics) =>
            isDelete
                ? prevTopics.filter((topic) => topic.Id !== deleteId)
                : prevTopics.map((topic) => (topic.Id === updatedTopic.Id ? updatedTopic : topic))
        );
    };

    const filteredTopics = () => {
        let filtered = topics;
        if (showMyTopics) {
            filtered = filtered.filter(topic => topic.isOwner);
        }

        if (searchQuery.trim() !== '') {
            const lowerCaseQuery = searchQuery.toLowerCase();
            filtered = filtered.filter(topic =>
                topic.title.toLowerCase().includes(lowerCaseQuery) ||
                topic.content.toLowerCase().includes(lowerCaseQuery)
            );
        }

        if (sortCriteria) {
            filtered = [...filtered].sort((a, b) => b[sortCriteria] - a[sortCriteria]);
        }
        return filtered;
    };

    const handleTopicAction = async (topicId, action) => {
    console.log(`${action} Topic ID: ${topicId}`);

    try {
        const endpoint = `/vote/${action}`;
        const response = await axiosInstance.post(endpoint, { topicId });
        if (response.status === 200) {
            const updatedTopic = response.data.topic;
            handleApiResponse(updatedTopic, action === 'delete', topicId);
            toast(response.data.message);
        }
    } catch (error) {
        toast("Error Happened");

        console.error(`Error ${action} topic: `, error);
    }
};

    const fetchTopics = async () => {
        try {
            const response = await axiosInstance.get('post/readall');
            console.log(response);
            console.log(response.data);
            setTopics(response.data);

        } catch (error) {
            console.error("Error fetching data: ", error);
        }
    };


    useEffect(() => {
        fetchTopics();
    }, []);

    return (
        <div className="homepage-container">
            <ToastContainer position="top-right" autoClose={1000} hideProgressBar={false} newestOnTop={false} closeOnClick rtl={false} pauseOnFocusLoss draggable pauseOnHover />
            <button className="create-topic-btn" onClick={navigateToCreateTopic}>Create a new topic</button>
            
            <div className="filter-sort-section">
                <input
                        type="text"
                        placeholder="Search topics..."
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        className="search-input"
                    />
                <div className="filter-option">
                    <input type="checkbox" id="myTopicsCheckbox" checked={showMyTopics} onChange={() => setShowMyTopics(!showMyTopics)} />
                    <label htmlFor="myTopicsCheckbox">Show only my topics</label>
                </div>
                <div className="sort-option">
                    <label htmlFor="sortSelect">Sort by:</label>
                    <select id="sortSelect" value={sortCriteria} onChange={(e) => setSortCriteria(e.target.value)}>
                        <option value="">Default</option>
                        <option value="numOfUpvotes">Upvotes</option>
                        <option value="numOfDownvotes">Downvotes</option>
                        <option value="numOfComments">Comments</option>
                    </select>
                </div>
            </div>

            <h1>{showMyTopics ? 'My Topics' : 'All Topics'}</h1>
            {filteredTopics().map((topic) => (
                <div className={`topic-card ${topic.locked ? 'locked-topic' : ''}`} key={topic.Id} onClick={() => navigateToTopic(topic.Id)}>
                {topic.locked && <span className="lock-icon" role="img" aria-label="Locked">🔒</span>}
                    <h1 className="topic-title">{topic.Naslov}</h1>
                    <p className="topic-content">{topic.Sadrzaj}</p>
                    <p className="topic-info">Created At: {topic.Timestamp}</p>
                    <p className="topic-info">Owner: {topic.ownerFullName}</p>
                    <p className="topic-info">Upvotes: {topic.GlazoviZa ? topic.GlasoviZa.length : 0} | Downvotes: {topic.GlasoviProtiv ? topic.GlasoviProtiv.length : 0}</p>
                    <p className="topic-info">Locked: {topic.locked ? 'Yes' : 'No'}</p>
                    <p className="topic-info">Comments: {topic.Komentari ? topic.Komentari : 0}</p>
                    <div>
                        <button className={`vote-button ${topic.userAction === 'UPVOTED' ? 'active-upvote-button' : ''}`}
                                onClick={(e) => 
                                {
                                    e.stopPropagation(); 
                                    handleTopicAction(topic.Id, "upvote")}
                                }>
                            {topic.userAction === 'UPVOTED' ? '✓ UPVOTED' : 'Upvote'}
                        </button>
                        <button className={`vote-button ${topic.userAction === 'DOWNVOTED' ? 'active-downvote-button' : ''}`}
                                onClick={(e) => 
                                    {
                                        e.stopPropagation(); 
                                        handleTopicAction(topic.Id, "downvote")}
                                    }>
                            {topic.userAction === 'DOWNVOTED' ? '✕ DOWNVOTED' : 'Downvote'}
                        </button>
                        <button className={`vote-button ${topic.isSubscribed ? 'subscribed-button' : 'subscribe-button'}`}
                                onClick={(e) => 
                                    {
                                        e.stopPropagation(); 
                                        handleTopicAction(topic.Id, "subscribe")}
                                    }>
                            {topic.isSubscribed ? '✓ Subscribed' : 'Subscribe'}
                        </button>
                    </div>
                    {topic.isOwner && (
                        <div className="topic-controls">
                            <button
                                className={`control-button ${topic.locked ? 'unlock-button' : 'lock-button'}`}
                                onClick={(e) => 
                                    {
                                        e.stopPropagation(); 
                                        handleTopicAction(topic.Id, "lock")}
                                    }>
                                {topic.locked ? 'Unlock' : 'Lock'}
                            </button>
                            <button
                                className="control-button delete-button"
                                onClick={(e) => 
                                    {
                                        e.stopPropagation(); 
                                        handleTopicAction(topic.Id, "delete")}
                                    }>
                                Delete
                            </button>
                        </div>
                    )}
                </div>
            ))}
    </div>
    );    
}

export default HomePage;
