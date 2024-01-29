import React, { useState } from 'react';
import ChatWindow from './ChatWindow';

function App() {
  const [messages, setMessages] = useState([]);

  const handleSendMessage = (newMessage) => {
    setMessages([...messages, newMessage]);
  };

  return (
    <div className="App">
      <ChatWindow messages={messages} onSendMessage={handleSendMessage} />
    </div>
  );
}

export default App;
