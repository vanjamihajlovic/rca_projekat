import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import axiosInstance from './axiosInstance';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './HomePage.css';


function TopicPage() {
    const navigate = useNavigate();

    const { topicId } = useParams();
    const { commentId } = useParams();
    const [topic, setTopic] = useState(null);
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState("");
    

    useEffect(() => {
        axiosInstance.get(`/post/read/${topicId}`).then(response => {
            console.log("fetched");
            console.log(response.data);
            setTopic(response.data);
            console.log(response.data.Komentari);
            setComments(response.data.Komentari)
        }).catch(error => {
            console.error("Error fetching topic details: ", error);
        });

        // axiosInstance.get(`/comments/${topicId}`).then(response => {
        //     setComments(response.data.comments);
        // }).catch(error => {
        //     console.error("Error fetching comments: ", error);
        // });
    }, [topicId]);

    const handleAddComment = async () => {
        try {
            const response = await axiosInstance.post("comment/create", { TopicId: topicId, Text: newComment, UserEmail: ""});
            if (response.status === 200) {
                console.log(response);
                setComments([...comments, response.data]);
                setNewComment("");
                toast("Comment added successfully");
            }
        } catch (error) {
            toast("Error adding comment");
            console.error("Error adding comment: ", error);
        }
    };

    
    

    const handleTopicSubscribe = async (topicId, action) => {
        console.log(`${action} Topic ID: ${topicId}`);
    
        try {
            const endpoint = `/subscribe/subscribepost/`;
            const response = await axiosInstance.post(endpoint, { PostId: topicId });
            if (response.status === 200) {
                //setTopic({...topic, IsSubscribed: topic.IsSubscribed})
                setTopic(prevTopic => ({
                    ...prevTopic,
                    IsSubscribed: !prevTopic.IsSubscribed
                }))
            }
        } catch (error) {
            toast("Error happened");
            console.error(`Error ${action} topic: `, error);
        }
    };

    const handleDelete = async (topicId, action) => {
        console.log(`${action} Topic ID: ${topicId}`);
    
        try {
            const endpoint = `post/delete/${topicId}`;
            const response = await axiosInstance.post(endpoint, { topicId });
            if (response.status === 200) {
                navigate("/");
                // const updatedTopic = response.data.topic;
                // handleApiResponse(updatedTopic, action === 'delete', topicId);
                toast(response.data.message);
            }
        } catch (error) {
            toast("Error Happened");
    
            console.error(`Error ${action} topic: `, error);
        }
    };

    const handleDeleteComment = async (commentId) => {
    
        try {
            const endpoint = `comment/delete/${commentId}`;
            const response = await axiosInstance.post(endpoint, { commentId });
            if (response.status === 200) {
                //navigate("/");
                setComments(comments.filter(comment => comment.RowKey !== commentId))
                toast(response.data.message);
            }
        } catch (error) {
            toast("Error Happened");
    
            
        }
    };

    const handleTopicAction = async (topicId, action) => {
        console.log(`${action} Topic ID: ${topicId}`);
    
        try {
            const endpoint = `/vote/${action}/${topicId}`;
            const response = await axiosInstance.post(endpoint, { topicId });
            if (response.status === 200) {


                if (action !== 'delete') {
                    axiosInstance.get(`/post/read/${topicId}`).then(response => {
                        console.log("fetched");
                        console.log(response.data);
                        setTopic(response.data);
                        console.log(response.data.Komentari);
                        setComments(response.data.Komentari)
                    }).catch(error => {
                        console.error("Error fetching topic details: ", error);
                    });
                    toast(`${action} succesful`)
                } else {
                    navigate("/")
                }
            }
        } catch (error) {
            toast("Error happened");
            console.error(`Error ${action} topic: `, error);
        }
    };

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        const options = { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit', second: '2-digit', timeZoneName: 'short' };
        return new Intl.DateTimeFormat('en-US', options).format(date);
      };

    
   
    
    

    if (!topic) {
        return <div>Loading...</div>; 
    }

    return (
        <div className="topic-page">
            <ToastContainer position="top-right" autoClose={1000} hideProgressBar={false} newestOnTop={false} closeOnClick rtl={false} pauseOnFocusLoss draggable pauseOnHover />
                <h1 className="topic-title">{topic.Naslov}</h1>
                <p className="topic-content">{topic.Sadrzaj}</p>
                <p className="topic-info">Updated At: {formatDate(topic.Timestamp)}</p>
                    <p className="topic-info">Owner:  {topic.FirstName} {topic.LastName}</p>
                    <p className="topic-info">Upvotes: {topic.GlasoviZa ? topic.GlasoviZa : 0} | Downvotes: {topic.GlasoviProtiv ? topic.GlasoviProtiv : 0}</p>
                    <p className="topic-info">Comments: {topic.Komentari ? topic.Komentari.length : 0}</p>
                    <div>
                        <button className={`vote-button ${topic.PostVoteStatus === 'UPVOTED' ? 'active-upvote-button' : ''}`}
                                onClick={() => handleTopicAction(topic.Id, "upvote")}>
                            {topic.PostVoteStatus === 'UPVOTED' ? '✓ UPVOTED' : 'Upvote'}
                        </button>
                        <button className={`vote-button ${topic.PostVoteStatus === 'DOWNVOTED' ? 'active-downvote-button' : ''}`}
                                onClick={() => handleTopicAction(topic.Id, "downvote")}>
                            {topic.PostVoteStatus === 'DOWNVOTED' ? '✕ DOWNVOTED' : 'Downvote'}
                        </button>
                        <button className={`vote-button ${topic.IsSubscribed ? 'subscribed-button' : 'subscribe-button'}`}
                                onClick={() => handleTopicSubscribe(topic.Id, "subscribe")}>
                            {topic.IsSubscribed ? '✓ Subscribed' : 'Subscribe'}
                        </button>
                    </div>
                    {topic.IsOwner && (
                        <div className="topic-controls">
                            <button
                                className="control-button delete-button"
                                onClick={() => handleDelete(topicId, "delete")}>
                                DELETE
                            </button>
                        </div>

                    )}
                    {topic.Slika && (
                        <img src={topic.Slika} alt="topic image" className="topic-image"/>
                    )}

            <div className="comment-section">
                {
                    (
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
                <div key={comment?.RowKey} className="comment">
                    <div className="comment-header">
                        <span className="comment-author">{comment?.AuthorName}</span>
                        <span className="comment-date"> at {formatDate(comment?.Timestamp)}</span>
                            <button
                                className="delete-comment-button"
                                onClick={() => handleDeleteComment(comment?.RowKey)}>
                                Delete
                            </button>
                    </div>
                    <p className="comment-content">{comment?.Sadrzaj}</p>
                    
                </div>
        ))}
            </div>
        </div>
    );
}

export default TopicPage;
