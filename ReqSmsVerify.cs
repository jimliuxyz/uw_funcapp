
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using Twilio;
using Microsoft.Azure.Documents.Client;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace UwFuncapp
{
    public static class ReqSmsVerify
    {
        static string SMS_SALT = "e^26rYS`}:~%E4`";
        static string ACCOUNT_SID = "AC869aecada546a05c62ba362000a62f2f";
        static string AUTH_TOKEN = "88cfe51f0483a59d173589667e12648e";

        static string DB_URI = "https://uwcosmos.documents.azure.com:443/";
        static string DB_KEY = "ImS3ZxaO7N7hFBFdSJHzVkOVC22tZlibbxRcgOQAlpKKWLvN2PDfwrovIatjFW4v0Iiw9Tv5jtED6reWSUYgLw==";

        static string DB_NAME = "UWallet";
        static string DB_COL_SMSPASSCODE = "SmsPasscode";

        static DocumentClient client;
        static Uri DB_URI_SMSPASSCODE;
        static ReqSmsVerify()
        {
            client = new DocumentClient(new Uri(DB_URI), DB_KEY);

            DB_URI_SMSPASSCODE = UriFactory.CreateDocumentCollectionUri(DB_NAME, DB_COL_SMSPASSCODE);
        }

        public static string Hash(string context)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] source = Encoding.Default.GetBytes(context + SMS_SALT);
            byte[] crypto = sha256.ComputeHash(source);
            return Convert.ToBase64String(crypto);
        }

        [FunctionName("reqSmsVerify")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            int jsonId = 0;
            try
            {
                string phoneno = ((string)req.Query["phoneno"]).ToString().Trim();
                string passcode = new Random().Next(1, 9999).ToString("0000");

                log.LogInformation("phoneno:" + phoneno);
                // log.LogInformation("passcode:" + passcode);

                var dq = client.CreateDocumentQuery<SmsPasscode>(DB_URI_SMSPASSCODE);
                var result = from c in dq where c.phoneno == Hash(phoneno) select c;

                //write to database
                SmsPasscode data = null;
                if (result.Count() > 0)
                {
                    //continue from old record
                    data = result.ToList().First();
                    data.passcode = Hash(passcode);

                    //todo : 每次紀錄保留60分鐘(ttl)
                    //todo : 每60秒允許發一次簡訊 最多3次
                    //todo : 每次簡訊 只允許驗證3次
                    //todo : 位數調至6位
                    await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DB_NAME, DB_COL_SMSPASSCODE, data.id), data);
                }
                else
                {
                    data = new SmsPasscode
                    {
                        phoneno = Hash(phoneno),
                        passcode = Hash(phoneno+passcode)
                    };
                    await client.CreateDocumentAsync(DB_URI_SMSPASSCODE, data);
                }


                //send sms message
                var smsclient = new TwilioRestClient(ACCOUNT_SID, AUTH_TOKEN);
                // smsclient.SendMessage(
                //     "+18065157359",
                //     "+" + phoneno,
                //     "验证码(Passcode):" + passcode);
                log.LogInformation("可能沒發簡訊喔~~");


                return JsonRPC.Ok(jsonId).ToActionResult();
            }
            catch (System.Exception e)
            {
                log.LogInformation(e.ToString());
            }

            // return InternalError for any not expected
            return JsonRPC.InternalError(jsonId).ToActionResult();
        }
    }

}
