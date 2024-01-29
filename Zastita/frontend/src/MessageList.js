import React from 'react';

const MessageList = React.forwardRef(({ messages }, ref) => {
  return (
    <div className="MessageList" ref={ref}>
      {messages.map((message, index) => (
        <div key={index} className={message.isMyMessage ? 'my-message' : 'other-message'}>
          {message.text}
        </div>
      ))}
    </div>
  );
});

export default MessageList;
