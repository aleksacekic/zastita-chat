using backend;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private readonly A51Algorithm a51Algorithm = new A51Algorithm();
    private readonly XTEA xteaAlgorithm = new XTEA("mojXTEAkljuc");

    TigerHash tigerHash = new TigerHash();


    public async Task Send(string message,  string selectedAlgorithm, string hash)
    {
        string encryptedMessage;

        if (selectedAlgorithm == "A51")
        {
            encryptedMessage = a51Algorithm.Crypt(message);
        }
        else if (selectedAlgorithm == "XTEA")
        {
            encryptedMessage = xteaAlgorithm.Encrypt(message);
        }
        else
        {
            return;
        }


        await Clients.Others.SendAsync("ReceiveMessage", Context.ConnectionId, encryptedMessage, hash);
    }

    public async Task ConnectToPeer(string peerConnectionId)
    {
        await Clients.Client(peerConnectionId).SendAsync("PeerConnected", Context.ConnectionId);
    }

    public async Task SendToPeer(string peerConnectionId, string message, string hash, string selectedAlgorithm)
    {
        string encryptedMessage;

        if (selectedAlgorithm == "A51")
        {
            encryptedMessage = a51Algorithm.Crypt(message);
        }
        else if (selectedAlgorithm == "XTEA")
        {
            encryptedMessage = xteaAlgorithm.Encrypt(message);
        }
        else
        {
            return;
        }

        await Clients.Client(peerConnectionId).SendAsync("ReceiveMessage", Context.ConnectionId, encryptedMessage, hash);
    }

    public async Task DisconnectFromPeer(string peerConnectionId)
    {
        await Clients.Client(peerConnectionId).SendAsync("PeerDisconnected", Context.ConnectionId);
    }

    public async Task DecryptAndDisplay(string encryptedMessage, string selectedAlgorithm, string hash)
    {
        string decryptedMessage;

        try
            {
            if (selectedAlgorithm == "A51")
            {
                decryptedMessage = a51Algorithm.DeCrypt(encryptedMessage);
            }
            else if (selectedAlgorithm == "XTEA")
            {
                decryptedMessage = xteaAlgorithm.Decrypt(encryptedMessage);
            }
            else
            {
                return;
            }

            await Clients.Caller.SendAsync("ReceiveDecryptedMessage", Context.ConnectionId, decryptedMessage, hash);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error in DecryptAndDisplay: {ex.Message}");
            
        }
    }

    public byte[] CalculateTigerHash(List<int> messageBytes)
    {
        try
        {
            byte[] byteArray = messageBytes.Select(b => (byte)b).ToArray();
            return tigerHash.Hash(byteArray, byteArray.Length);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error calculating Tiger Hash: {ex.Message}");
            return new byte[0];//prazan niz ako se desi greska
        }
    }
     
}

