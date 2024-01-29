using System.Collections.Generic;

namespace backend
{
    public interface ICrypto
    {

        #region Interface Methods

        byte[] Crypt(byte[] input);

        
        byte[] Decrypt(byte[] output);

        #endregion

    }
}