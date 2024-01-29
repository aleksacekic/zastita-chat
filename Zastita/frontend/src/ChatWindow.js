import React, { useRef, useEffect, useState, useCallback } from "react";
import MessageList from "./MessageList";
import MessageInput from "./MessageInput";
import * as signalR from "@microsoft/signalr";

const ChatWindow = ({ messages }) => {
  const messageListRef = useRef(null);
  const [connection, setConnection] = useState(null);
  const [localMessages, setLocalMessages] = useState([]);
  const [enkr, setEnkr] = useState(null);
  const algorithmRef = useRef("XTEA"); // inic vrednost

  const onSendMessage = useCallback((message, algorithm) => {
    console.log("Poruka:", message);
    //console.log("Odabrani algoritam:", algorithm);

    algorithmRef.current = algorithm;
  }, []);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:5001/chatHub")
      .build();

    setConnection(newConnection);

    newConnection
      .start()
      .then(() => console.log("Connection started!"))
      .catch((err) =>
        console.error("Error while establishing connection:", err)
      );

    newConnection.on("ReceiveMessage", (fromConnectionId, message, hash) => {
      setEnkr(message);
      if (algorithmRef.current === undefined) {
        let novapromenljiva = "XTEA";
        try {
          newConnection.invoke(
            "DecryptAndDisplay",
            message,
            novapromenljiva,
            hash
          );
        } catch (error) {
          console.error("Error invoking DecryptAndDisplay:", error);
        }
      } else {
        try {
          newConnection.invoke(
            "DecryptAndDisplay",
            message,
            algorithmRef.current,
            hash
          );
        } catch (error) {
          console.error("Error invoking DecryptAndDisplay:", error);
        }
      }
    });

    return () => {
      newConnection.stop();
    };
  }, [onSendMessage]);

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

  function removeNullCharacters(inputString) {
    return inputString.replace(/\0/g, "");
  }

  useEffect(() => {
    if (connection) {
      connection.on("PeerConnected", (peerConnectionId) => {
        console.log(`Peer connected: ${peerConnectionId}`);
      });

      connection.on("PeerDisconnected", (peerConnectionId) => {
        console.log(`Peer disconnected: ${peerConnectionId}`);
      });

      connection.on(
        "ReceiveDecryptedMessage",
        async (fromConnectionId, decryptedMessage, tigerHash) => {
          const receivedMessage = {
            text: decryptedMessage,
            tigerHash: tigerHash,
            isMyMessage: false,
          };

          //hash dekriptovane poruke
          const decryptedWithoutNullCh = removeNullCharacters(decryptedMessage);
          const tigerHashDecrypted = await calculateTigerHash(
            decryptedWithoutNullCh
          );

          const isHashValid =
            tigerHashDecrypted &&
            tigerHash &&
            tigerHashDecrypted.toString() === tigerHash.toString();

          //console.log("Bool vrednos : " + isHashValid);

          if (isHashValid) {
            onSendMessage(receivedMessage);
            setLocalMessages((prevMessages) => [
              ...prevMessages,
              receivedMessage,
            ]);
          } else {
            //console.error("Hash se ne poklapa.");
            alert(
              "Doslo je do greske pri prenosu poruke koja je trebalo da Vam stigne."
            );
          }

          if (messageListRef.current) {
            messageListRef.current.scrollTop =
              messageListRef.current.scrollHeight;
          }
        }
      );
    }
  }, [connection, onSendMessage]);

  return (
    <div>
      <div className="ChatWindow">
        <MessageList messages={localMessages} ref={messageListRef} />
        <MessageInput
          connection={connection}
          onSendMessage={(text, algorithm, hash) => {
            if (connection) {
              algorithmRef.current = algorithm;
              const sentMessage = { text, isMyMessage: true };
              setLocalMessages((prevMessages) => [
                ...prevMessages,
                sentMessage,
              ]);
              const mojString = hash.toString();
              connection.invoke("Send", text, algorithm, mojString);
            }
          }}
        />
      </div>
      {/* <div>Poslednja pristigla poruka: {dek}</div> */}
      {/* <p className="enc">Enkripcija poslednje primljene poruke: {enkr}</p> */}
    </div>
  );
};

export default ChatWindow;
