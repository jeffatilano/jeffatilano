using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace JIL_Attendance.Helpers
{
    public class UrlEncryptionHelper
    {

        private static byte[] _optionalEntropy = { 9, 8, 7, 6, 5 };

        public static string EncryptId(int id)
        {
            return Encrypt(id.ToString());
        }

        public static int DecryptId(string id)
        {
            return Int32.Parse(Decrypt(id));
        }

        public static string Encrypt(string plainText)
        {
            return HttpServerUtility.UrlTokenEncode(ProtectedData.Protect(Encoding.UTF8.GetBytes(plainText), _optionalEntropy, DataProtectionScope.LocalMachine));
        }

        public static string Decrypt(string text)
        {
            return Encoding.UTF8.GetString(ProtectedData.Unprotect(HttpServerUtility.UrlTokenDecode(text), _optionalEntropy, DataProtectionScope.LocalMachine));
        }

        public static byte[] CreateRandomEntropy()
        {
            // Create a byte array to hold the random value. 
            byte[] entropy = new byte[16];

            // Create a new instance of the RNGCryptoServiceProvider. 
            // Fill the array with a random value. 
            new RNGCryptoServiceProvider().GetBytes(entropy);

            // Return the array. 
            return entropy;


        }
    }
}