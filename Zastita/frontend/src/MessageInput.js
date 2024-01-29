import React, { useState } from "react";

const MessageInput = ({ onSendMessage, connection }) => {
  const [inputText, setInputText] = useState("");
  const [selectedAlgorithm, setSelectedAlgorithm] = useState("XTEA");
  const [fileContent, setFileContent] = useState(null);

  const handleInputChange = (event) => {
    setInputText(event.target.value);
  };

  const handleSendClick = async () => {
    if (inputText.trim() !== "") {
      const hash = await calculateTigerHash(inputText);
      onSendMessage(inputText, selectedAlgorithm, hash);
      setInputText("");
    } else if (fileContent) {
      const hash = await calculateTigerHash(fileContent);
      onSendMessage(fileContent, selectedAlgorithm, hash);
      setFileContent(null);
    }
  };

  const handleFileChange = (event) => {
    const file = event.target.files[0];

    if (file) {
      const reader = new FileReader();
      reader.onload = (e) => {
        const fileText = e.target.result;
        console.log("Tekst iz fajla:", fileText);

        setFileContent(fileText);
      };

      reader.readAsText(file);
    }
  };

  const calculateTigerHash = async (message) => {
    const messageBytes = new TextEncoder().encode(message);
    try {
      const response = await connection.invoke(
        "CalculateTigerHash",
        Array.from(messageBytes)
      );

      const tigerHashBytes = Array.from(response);

      return tigerHashBytes;
    } catch (error) {
      console.error("Error invoking CalculateTigerHash:", error);
      return null;
    }
  };

  return (
    <div>
      <div className="MessageInput">
        <input type="text" value={inputText} onChange={handleInputChange} />
        <button onClick={handleSendClick}>SEND</button>
        <input type="file" onChange={handleFileChange} className="file-input" />
        <div
          className="customCheckbox"
          onClick={() => setSelectedAlgorithm("XTEA")}
        >
          <input
            type="checkbox"
            id="useXTEACheckbox"
            checked={selectedAlgorithm === "XTEA"}
            onChange={() => {}}
          />
          <label htmlFor="useXTEACheckbox">XTEA</label>
        </div>

        <div
          className="customCheckbox"
          onClick={() => setSelectedAlgorithm("A51")}
        >
          <input
            type="checkbox"
            id="useA51Checkbox"
            checked={selectedAlgorithm === "A51"}
            onChange={() => {}}
          />
          <label htmlFor="useA51Checkbox">A5/1</label>
        </div>
      </div>
    </div>
  );
};

export default MessageInput;
