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
    const [currentPage, setCurrentPage] = useState(1); // State to keep track of current page
    const [pageSize, setPageSize] = useState(5); // State to keep track of page size

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

    const handleTopicSubscribe = async (topicId, action) => {
        console.log(`${action} Topic ID: ${topicId}`);
    
        try {
            const endpoint = `/subscribe/subscribepost/`;
            const response = await axiosInstance.post(endpoint, { PostId: topicId });
            if (response.status === 200) {
                fetchTopics();
            }
        } catch (error) {
            toast("Error happened");
            console.error(`Error ${action} topic: `, error);
        }
    };

    const filteredTopics = () => {
        let filtered = topics;
        if (showMyTopics) {
            filtered = filtered.filter(topic => topic.IsOwner);
        }

        if (searchQuery.trim() !== '') {
            const lowerCaseQuery = searchQuery.toLowerCase();
            filtered = filtered.filter(topic =>
                topic.Naslov.toLowerCase().includes(lowerCaseQuery) ||
                topic.Sadrzaj.toLowerCase().includes(lowerCaseQuery)
            );
        }


        if (sortCriteria) {
            console.log(filtered);
            filtered = [...filtered].sort((a, b) => b[sortCriteria] - a[sortCriteria]);
        }
        return filtered;
    };

    const handleDelete = async (topicId, action) => {
        console.log(`${action} Topic ID: ${topicId}`);
    
        try {
            const endpoint = `post/delete/${topicId}`;
            const response = await axiosInstance.post(endpoint, { topicId });
            if (response.status === 200) {
                fetchTopics();
                toast(response.data.message);
            }
        } catch (error) {
            toast("Error Happened");
    
            console.error(`Error ${action} topic: `, error);
        }
    };

    const handleTopicAction = async (topicId, action) => {
    console.log(`${action} Topic ID: ${topicId}`);

    try {
        const endpoint = `/vote/${action}/${topicId}`;
        const response = await axiosInstance.post(endpoint, { topicId });
        if (response.status === 200) {
            fetchTopics();
            toast(response.data.message);
        }
    } catch (error) {
        toast("Error Happened");

        console.error(`Error ${action} topic: `, error);
    }
};

     const fetchTopics = async () => {
        try {
            const response = await axiosInstance.get(`post/readallpaginated?page=${currentPage}&pageSize=${pageSize}`);
            setTopics(response.data);
        } catch (error) {
            console.error("Error fetching data: ", error);
        }
    };

    useEffect(() => {
        fetchTopics();
    }, [currentPage, pageSize]);

    const handlePageChange = (page) => {
        setCurrentPage(page);
    };

    const handlePageSizeChange = (size) => {
        setPageSize(size);
    };

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
                        <option value="GlasoviZa">Upvotes</option>
                        <option value="GlasoviProtiv">Downvotes</option>
                    </select>
                </div>
            </div>

            <h1>{showMyTopics ? 'My Topics' : 'All Topics'}</h1>
            {filteredTopics().map((topic) => (
                <div className={`topic-card`} key={topic.Id} onClick={() => navigateToTopic(topic.Id)}>
                    <h1 className="topic-title">{topic.Naslov}</h1>
                    <p className="topic-content">{topic.Sadrzaj}</p>
                    <p className="topic-info">Created At: {topic.Timestamp}</p>
                    <p className="topic-info">Owner: {topic.FirstName} {topic.LastName}</p>
                    <p className="topic-info">Upvotes: {topic.GlasoviZa ? topic.GlasoviZa : 0} | Downvotes: {topic.GlasoviProtiv ? topic.GlasoviProtiv : 0}</p>
                    <p className="topic-info">Comments: {topic.Komentari ? topic.Komentari.length : 0}</p>
                    <div>
                        <button className={`vote-button ${topic.PostVoteStatus === 'UPVOTED' ? 'active-upvote-button' : ''}`}
                                onClick={(e) => 
                                {
                                    e.stopPropagation(); 
                                    handleTopicAction(topic.Id, "upvote")}
                                }>
                            {topic.PostVoteStatus === 'UPVOTED' ? '✓ UPVOTED' : 'Upvote'}
                        </button>
                        <button className={`vote-button ${topic.PostVoteStatus === 'DOWNVOTED' ? 'active-downvote-button' : ''}`}
                                onClick={(e) => 
                                    {
                                        e.stopPropagation(); 
                                        handleTopicAction(topic.Id, "downvote")}
                                    }>
                            {topic.PostVoteStatus === 'DOWNVOTED' ? '✕ DOWNVOTED' : 'Downvote'}
                        </button>
                        <button className={`vote-button ${topic.IsSubscribed ? 'subscribed-button' : 'subscribe-button'}`}
                                onClick={(e) => 
                                    {
                                        e.stopPropagation(); 
                                        handleTopicSubscribe(topic.Id, "subscribe")}
                                    }>
                            {topic.IsSubscribed ? '✓ Subscribed' : 'Subscribe'}
                        </button>
                    </div>
                    {topic.IsOwner && (
                        <div className="topic-controls">
                            <button
                                className="control-button delete-button"
                                onClick={(e) => 
                                    {
                                        e.stopPropagation(); 
                                        handleDelete(topic.Id, "delete")}
                                    }>
                                Delete
                            </button>
                        </div>
                    )}
                    {topic.Slika && (
                        <img src={topic.Slika} alt="topic image" className="topic-image"/>
                    )}
                </div>
            ))}
            <div className="pagination">
                <button onClick={() => setCurrentPage(prev => prev > 1 ? prev - 1 : prev)} disabled={currentPage === 1}>
                    Previous
                </button>
                <span>Page {currentPage}</span>
                <button onClick={() => setCurrentPage(prev => prev + 1)}>Next</button>
                <select value={pageSize} onChange={(e) => setPageSize(Number(e.target.value))}>
                    <option value={5}>5 per page</option>
                    <option value={10}>10 per page</option>
                    <option value={15}>15 per page</option>
                </select>
            </div>
    </div>
    );    
}

export default HomePage;
