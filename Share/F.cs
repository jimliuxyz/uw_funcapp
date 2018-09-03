
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace UwFuncapp
{
    /// <summary>
    /// Shared Functions
    /// </summary>
    public static class F
    {
        public static string Hash(string context)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] source = Encoding.Default.GetBytes(context + R.SALT_SMS);
            byte[] crypto = sha256.ComputeHash(source);
            return Convert.ToBase64String(crypto);
        }

    }
}

