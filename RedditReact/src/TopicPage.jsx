import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import axiosInstance from './axiosInstance';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './HomePage.css';


function TopicPage() {
    const navigate = useNavigate();

    const { topicId } = useParams();
    const [topic, setTopic] = useState(null);
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState("");

    useEffect(() => {
        axiosInstance.get(`/post/read/${topicId}`).then(response => {
            console.log("fetched");
            console.log(response.data);
            setTopic(response.data);
        }).catch(error => {
            console.error("Error fetching topic details: ", error);
        });

        axiosInstance.get(`/comments/${topicId}`).then(response => {
            setComments(response.data.comments);
        }).catch(error => {
            console.error("Error fetching comments: ", error);
        });
    }, [topicId]);

    const handleAddComment = async () => {
        try {
            const response = await axiosInstance.post(`http://localhost:8080/comment/create`, { TopicId: topicId, Text: newComment, UserEmail: ""});
            if (response.status === 200) {
                setComments([...comments, response.data.comment]);
                setNewComment("");
                toast("Comment added successfully");
            }
        } catch (error) {
            toast("Error adding comment");
            console.error("Error adding comment: ", error);
        }
    };

    const handleVoteComment = async (commentId, action) => {
        try {
            const endpoint = `/comments/${action}`;
            const response = await axiosInstance.post(endpoint, { commentId });
            if (response.status === 200) {
                setComments(comments.map(comment => comment.Id === commentId ? response.data.comment : comment));
                toast(response.data.message);
            }
        } catch (error) {
            toast("Error voting on comment");
            console.error(`Error ${action} comment: `, error);
        }
    };


    const handleTopicAction = async (topicId, action) => {
        console.log(`${action} Topic ID: ${topicId}`);
    
        try {
            const endpoint = `/topic/${action}`;
            const response = await axiosInstance.post(endpoint, { topicId });
            if (response.status === 200) {
                if (action !== 'delete') {
                    setTopic(response.data.topic);
                    toast(response.data.message);
                } else {
                    navigate("/")
                }
            }
        } catch (error) {
            toast("Error happened");
            console.error(`Error ${action} topic: `, error);
        }
    };

    if (!topic) {
        return <div>Loading...</div>; 
    }

    return (
        <div className="topic-page">
            <ToastContainer position="top-right" autoClose={1000} hideProgressBar={false} newestOnTop={false} closeOnClick rtl={false} pauseOnFocusLoss draggable pauseOnHover />
            <div className={`topic-card ${topic.locked ? 'locked-topic' : ''}`}>
                <h1 className="topic-title">{topic.Naslov}</h1>
                <p className="topic-content">{topic.Sadrzaj}</p>
                <p className="topic-info">Created At: {topic.Timestamp}</p>
                    <p className="topic-info">Owner:  {topic.FirstName} {topic.LastName}</p>
                    <p className="topic-info">Upvotes: {topic.GlazoviZa ? topic.GlasoviZa.length : 0} | Downvotes: {topic.GlasoviProtiv ? topic.GlasoviProtiv.length : 0}</p>
                    <p className="topic-info">Locked: {topic.locked ? 'Yes' : 'No'}</p>
                    <p className="topic-info">Comments: {topic.Komentari ? topic.Komentari : 0}</p>
                    <div>
                        <button className={`vote-button ${topic.userAction === 'UPVOTED' ? 'active-upvote-button' : ''}`}
                                onClick={() => handleTopicAction(topic.Id, "upvote")}>
                            {topic.userAction === 'UPVOTED' ? '✓ UPVOTED' : 'Upvote'}
                        </button>
                        <button className={`vote-button ${topic.userAction === 'DOWNVOTED' ? 'active-downvote-button' : ''}`}
                                onClick={() => handleTopicAction(topic.Id, "downvote")}>
                            {topic.userAction === 'DOWNVOTED' ? '✕ DOWNVOTED' : 'Downvote'}
                        </button>
                        <button className={`vote-button ${topic.isSubscribed ? 'subscribed-button' : 'subscribe-button'}`}
                                onClick={() => handleTopicAction(topic.Id, "subscribe")}>
                            {topic.isSubscribed ? '✓ Subscribed' : 'Subscribe'}
                        </button>
                    </div>
                    {topic.isOwner && (
                        <div className="topic-controls">
                            <button
                                className={`control-button ${topic.locked ? 'unlock-button' : 'lock-button'}`}
                                onClick={() => handleTopicAction(topic.Id, "lock")}>
                                {topic.locked ? 'UNLOCK' : 'LOCK'}
                            </button>
                            <button
                                className="control-button delete-button"
                                onClick={() => handleTopicAction(topic.Id, "delete")}>
                                DELETE
                            </button>
                        </div>
                    )}
            </div>
            <div className="comment-section">
                {
                    topic.locked ? (
                        <p className="locked-message">This topic is locked. Comments are disabled.</p>
                    ) : (
                        <>
                            <textarea 
                                className="comment-box" 
                                placeholder="Write a comment..." 
                                value={newComment}
                                disabled={topic.locked} 
                                onChange={(e) => setNewComment(e.target.value)}>
                            </textarea>
                            <button className="submit-comment" onClick={handleAddComment} disabled={topic.locked}>Submit Comment</button>
                        </>
                    )
                }
                {comments.map(comment => (
                <div key={comment?.Id} className="comment">
                    <div className="comment-header">
                        <span className="comment-author">{comment?.ownerFullName}</span>
                        <span className="comment-date"> at {new Date(comment?.createdAt).toLocaleDateString()} {new Date(comment?.createdAt).toLocaleTimeString()}</span>
                    </div>
                    <p className="comment-content">{comment?.Text}</p>
                    <div className="comment-votes">
                        <button onClick={() => handleVoteComment(comment?.Id, "upvote")}>Upvote</button>
                        <span>{comment?.numOfUpvotes - comment?.numOfDownvotes}</span>
                        <button onClick={() => handleVoteComment(comment?.Id, "downvote")}>Downvote</button>
                    </div>
                </div>
        ))}
            </div>
        </div>
    );
}

export default TopicPage;
