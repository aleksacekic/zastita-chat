using System;
using System.Text;

public class XTEA
{
    private readonly uint[] key;

    private readonly uint[] fixedKey = { 0x01234567, 0x89ABCDEF, 0xFEDCBA98, 0x76543210 }; //random bukvalno
    public XTEA(string keyString) //keystring ako bismo hteli od stringa da napr
    {
        key =  fixedKey;
    }

    public string Encrypt(string plainText)
    {
        //pretvaranje stringa u niz bajtova
        byte[] data = Encoding.UTF8.GetBytes(plainText);
        //padding ako zatreba (8byt)
        int padding = 8 - (data.Length % 8);
        byte[] paddedData = new byte[data.Length + padding];
        Array.Copy(data, paddedData, data.Length);

        for (int i = 0; i < paddedData.Length; i += 8)
        {
            uint[] block = ToUInt32Array(paddedData, i, 8);
            EncryptBlock(block);
            Buffer.BlockCopy(ToByteArray(block), 0, paddedData, i, 8);
        }

        return Convert.ToBase64String(paddedData);
    }

    public string Decrypt(string cipherText)
    {
        byte[] data = Convert.FromBase64String(cipherText);

        for (int i = 0; i < data.Length; i += 8)
        {
            uint[] block = ToUInt32Array(data, i, 8);
            DecryptBlock(block);
            Buffer.BlockCopy(ToByteArray(block), 0, data, i, 8);
        }

        //sklanjanje padding
        int padding = data[data.Length - 1];
        byte[] unpaddedData = new byte[data.Length - padding];
        Array.Copy(data, unpaddedData, unpaddedData.Length);

        return Encoding.UTF8.GetString(unpaddedData);
    }

   private void EncryptBlock(uint[] block)
{
    const uint delta = 0x9e3779b9;
    uint sum = 0;

    for (int i = 0; i < 32; i++)
    {
        sum += delta;
        block[0] += ((block[1] << 4) + key[0]) ^ (block[1] + sum) ^ ((block[1] >> 5) + key[1]);
        block[1] += ((block[0] << 4) + key[2]) ^ (block[0] + sum) ^ ((block[0] >> 5) + key[3]);
    }
}


   private void DecryptBlock(uint[] block)
{
    const ulong delta = 0x9e3779b9;
    ulong sum = delta * 32;

    for (int i = 0; i < 32; i++)
    {
        block[1] -= (uint)unchecked(((block[0] << 4) + key[2]) ^ (block[0] + sum) ^ ((block[0] >> 5) + key[3]));
        block[0] -= (uint)unchecked(((block[1] << 4) + key[0]) ^ (block[1] + sum) ^ ((block[1] >> 5) + key[1]));
        sum -= delta;
    }
}

    private uint[] ToUInt32Array(byte[] data, int startIndex, int length)
    {
        uint[] result = new uint[length / 4];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = BitConverter.ToUInt32(data, startIndex + i * 4);
        }
        return result;
    }

    private byte[] ToByteArray(uint[] data)
    {
        byte[] result = new byte[data.Length * 4];
        for (int i = 0; i < data.Length; i++)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(data[i]), 0, result, i * 4, 4);
        }
        return result;
    }

   
  

}
