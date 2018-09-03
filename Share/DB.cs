
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using UW.Models.Collections;

namespace UwFuncapp
{
    /// <summary>
    /// Shared Functions
    /// </summary>
    public static class DB
    {
        private static Uri URI_DB = UriFactory.CreateDatabaseUri(R.DB_NAME);

        //user
        private static string COL_USER = typeof(UW.Models.Collections.User).Name;
        private static Uri URI_USER = UriFactory.CreateDocumentCollectionUri(R.DB_NAME, COL_USER);

        //sms passcode
        private static string COL_SMSPCODE = typeof(SmsPasscode).Name;
        private static Uri URI_SMSPCODE = UriFactory.CreateDocumentCollectionUri(R.DB_NAME, COL_SMSPCODE);

        //contact
        private static string COL_CONTACT = typeof(Contacts).Name;
        private static Uri URI_CONTACT = UriFactory.CreateDocumentCollectionUri(R.DB_NAME, COL_CONTACT);

        //balance
        private static string COL_BALANCE = typeof(Balance).Name;
        private static Uri URI_BALANCE = UriFactory.CreateDocumentCollectionUri(R.DB_NAME, COL_BALANCE);
        public static void InitDB()
        {
            var client = R.client;

            //create database
            client.CreateDatabaseIfNotExistsAsync(new Database { Id = R.DB_NAME }).Wait();

            //create collections
            var defReqOpts = new RequestOptions { OfferThroughput = 400 }; //todo:實際運作400RU可能太小
            client.CreateDocumentCollectionIfNotExistsAsync(URI_DB,
                                new DocumentCollection { Id = COL_USER }, defReqOpts).Wait();
            client.CreateDocumentCollectionIfNotExistsAsync(URI_DB,
                                new DocumentCollection { Id = COL_SMSPCODE, DefaultTimeToLive = 60 * 60 }, defReqOpts).Wait(); //60min for a round
            client.CreateDocumentCollectionIfNotExistsAsync(URI_DB,
                                new DocumentCollection { Id = COL_CONTACT }, defReqOpts).Wait();
            client.CreateDocumentCollectionIfNotExistsAsync(URI_DB,
                                new DocumentCollection { Id = COL_BALANCE }, defReqOpts).Wait();
        }
    }
}

