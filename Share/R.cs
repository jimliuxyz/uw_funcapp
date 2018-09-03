
using System;
using Microsoft.Azure.Documents.Client;

namespace UwFuncapp
{
    /// <summary>
    /// Shared Resources
    /// </summary>
    public static class R
    {
        //Environment Variables
        public static readonly string JWT_SECRET;
        public static readonly string SALT_SMS;
        public static readonly string TWILIO_SID, TWILIO_AUTH_TOKEN, TWILIO_PHONENO;
        public static readonly string DB_URI, DB_KEY, DB_NAME;
        public static readonly string BLOB_NAME, BLOB_KEY;

        public static readonly string DB_COL_SMSPASSCODE = "SmsPasscode";
        public static readonly string DB_COL_USER = "User";

        public static readonly DocumentClient client;
        public static readonly Uri DB_URI_SMSPASSCODE, DB_URI_USER;
        static R()
        {
            Console.WriteLine("GetEnvironmentVariable....");
            JWT_SECRET = Environment.GetEnvironmentVariable("JWT_SECRET");
            SALT_SMS = Environment.GetEnvironmentVariable("SALT_SMS");
            TWILIO_SID = Environment.GetEnvironmentVariable("TWILIO_SID");
            TWILIO_AUTH_TOKEN = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            TWILIO_PHONENO = Environment.GetEnvironmentVariable("TWILIO_PHONENO");
            DB_URI = Environment.GetEnvironmentVariable("DB_URI");
            DB_KEY = Environment.GetEnvironmentVariable("DB_KEY");
            DB_NAME = Environment.GetEnvironmentVariable("DB_NAME");

            BLOB_NAME = Environment.GetEnvironmentVariable("BLOB_NAME");
            BLOB_KEY = Environment.GetEnvironmentVariable("BLOB_KEY");

            client = new DocumentClient(new Uri(DB_URI), DB_KEY);

            DB_URI_SMSPASSCODE = UriFactory.CreateDocumentCollectionUri(DB_NAME, DB_COL_SMSPASSCODE);
            DB_URI_USER = UriFactory.CreateDocumentCollectionUri(DB_NAME, DB_COL_USER);

        }
    }
}