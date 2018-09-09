
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
using UW.Models.Collections;
using UW;

namespace UwFuncapp
{
    public static class ReqSmsVerify
    {
        [FunctionName("reqSmsVerify")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, ILogger log)
        {
            int jsonId = 0;
            try
            {
                //todo : do this to recover DB after DB reset. remove this while release
                DB.InitDB();

                string requestBody = new StreamReader(req.Body).ReadToEnd();
                JsonRpcQuery jreq = JsonConvert.DeserializeObject<JsonRpcQuery>(requestBody);
                jsonId = jreq.id;

                string phoneno = ((string)jreq.params_.phoneno).Trim();
                string passcode = new Random().Next(1, 9999).ToString("0000");

                log.LogInformation("phoneno:" + phoneno);
                // log.LogInformation("passcode:" + passcode);

                var client = R.client;
                var dq = client.CreateDocumentQuery<SmsPasscode>(R.DB_URI_SMSPASSCODE);
                var result = from c in dq where c.phoneno == F.Hash(phoneno) select c;

                //write to database
                SmsPasscode data = null;
                if (result.Count() > 0)
                {
                    //continue from old sms record
                    data = result.ToList().First();

                    if (data.resendCount >= 3)
                        return JsonRpcRes.Bad(jsonId, RPCERR.SMS_RESEND_EXCEEDED).ToActionResult();

                    //update sms record
                    data.passcode = F.Hash(passcode);
                    data.resendCount++;
                    data.verifyCount = 0;
                    data.verifyAvailTime = DateTime.UtcNow.AddMinutes(10);

                    await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(R.DB_NAME, R.DB_COL_SMSPASSCODE, data.id), data);
                }
                else
                {
                    //create new sms record
                    data = new SmsPasscode
                    {
                        phoneno = F.Hash(phoneno),
                        passcode = F.Hash(phoneno + passcode),
                        resendCount = 1,
                        verifyCount = 0,
                        verifyAvailTime = DateTime.UtcNow.AddMinutes(10)
                    };
                    await client.CreateDocumentAsync(R.DB_URI_SMSPASSCODE, data);
                }

                //send sms message
                var smsclient = new TwilioRestClient(R.TWILIO_SID, R.TWILIO_AUTH_TOKEN);
                smsclient.SendMessage(
                    R.TWILIO_PHONENO,
                    "+" + phoneno,
                    "验证码(Passcode):" + passcode);

                return JsonRpcRes.Ok(jsonId, new
                {
                    resendCount = data.resendCount
                }).ToActionResult();
            }
            catch (System.Exception e)
            {
                log.LogInformation(e.ToString());
            }

            // return InternalError for any not expected
            return JsonRpcRes.InternalError(jsonId).ToActionResult();
        }
    }

}
