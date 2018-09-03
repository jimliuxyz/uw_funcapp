
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
using Microsoft.Azure.Documents.Client;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace UwFuncapp
{
    public static class UploadAvatar
    {
        static string blobRoot = "avatar";
        static int THUMB_SIZE = 200;
        static int MAX_SIZE = 1 * 1000 * 1000;

        static DocumentClient client = R.client;

        [FunctionName("uploadAvatar")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            int jsonId = 0;

            string userId = null;
            User user = null;
            try
            {
                //validate token
                var token = req.Headers?.Authorization?.ToString()?.Replace("Bearer ", "");
                // log.Info("token : " + token);
                ClaimsPrincipal principle = JwtValidator.GetPrincipal(token, log);

                var identity = principle.Identity as ClaimsIdentity;
                if (!identity.IsAuthenticated)
                    return JsonRpcRes.Unauthorized(jsonId).ToActionResult();

                userId = identity.FindFirst("userId")?.Value;
                log.LogInformation("userId : " + userId);

                user = getUser(userId, log);
            }
            catch (Exception e)
            {
                log.LogInformation(e.ToString());
                return JsonRpcRes.Unauthorized(jsonId).ToActionResult();
            }


            var randomId = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/]", "-");
            var fileName_thumb = randomId + ".jpg";
            var imgUrl = "https://" + R.BLOB_NAME + ".blob.core.windows.net/" + blobRoot + "/200/" + fileName_thumb;
            // log.Info("fileName_thumb : " + fileName_thumb);
            try
            {
                var provider = new MultipartMemoryStreamProvider();
                await req.Content.ReadAsMultipartAsync(provider);

                //get file information
                var file = provider.Contents.First();
                var fileInfo = file.Headers.ContentDisposition;
                var fileData = await file.ReadAsByteArrayAsync();
                var fileName = fileInfo.FileName.Trim('"');
                var fileLength = fileData.Length;
                log.LogInformation("fileName:" + fileName);
                log.LogInformation("fileLength:" + fileLength);

                if (fileLength <= 0 || fileLength > MAX_SIZE)
                    return JsonRpcRes.InvalidRequest(jsonId, "file is too large").ToActionResult();

                using (var fileStream = new MemoryStream(fileData))
                {
                    //upload source image to blob
                    // await CreateBlobImage("avatar", fileName, fileStream, log); 
                    // fileStream.Position = 0;

                    //create thumb
                    using (var thumbStream = new MemoryStream())
                    {
                        using (var image = Image.Load(fileStream))
                        {
                            image.Mutate(ctx => ctx.Resize(THUMB_SIZE, THUMB_SIZE));
                            image.Save(thumbStream, new JpegEncoder());
                        }
                        thumbStream.Position = 0;

                        //upload thumb to blob
                        await CreateBlobImage(THUMB_SIZE + "/", fileName_thumb, thumbStream, log);
                    }

                    //todo : remove old avatar before replacement 
                    user.avatar = imgUrl;
                    await updateUser(user, log);
                }
                return JsonRpcRes.Ok(jsonId, new
                {
                    url = imgUrl
                }).ToActionResult();
            }
            catch (Exception e)
            {
                log.LogInformation(e.ToString());
            }
            // return InternalError for any not expected
            return JsonRpcRes.InternalError(jsonId).ToActionResult();
        }

        private async static Task CreateBlobImage(string folder, string name, MemoryStream data, ILogger log)
        {
            string connectionString;
            CloudStorageAccount storageAccount;
            CloudBlobClient client;
            CloudBlobContainer container;
            CloudBlockBlob blob;

            connectionString = "DefaultEndpointsProtocol=https;AccountName=" + R.BLOB_NAME + ";AccountKey=" + R.BLOB_KEY + ";EndpointSuffix=core.windows.net";
            storageAccount = CloudStorageAccount.Parse(connectionString);

            //get or create blob container
            client = storageAccount.CreateCloudBlobClient();
            container = client.GetContainerReference(blobRoot);
            await container.CreateIfNotExistsAsync();

            //change container permission.
            //todo : call this only once at creation
            BlobContainerPermissions permissions = await container.GetPermissionsAsync();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            await container.SetPermissionsAsync(permissions);

            //upload data 
            // blob = container.GetBlockBlobReference(name);
            var dir = container.GetDirectoryReference(folder);
            blob = dir.GetBlockBlobReference(name);
            blob.Properties.ContentType = "image/jpg";
            await blob.UploadFromStreamAsync(data);
        }

        public static User getUser(string userId, ILogger log)
        {
            try
            {
                var dq = client.CreateDocumentQuery<User>(R.DB_URI_USER);
                var result = from c in dq where c.userId == userId select c;
                return result.ToList().First();
            }
            catch (Exception)
            {
                throw new Exception($"User({userId}) not found");
            }
        }

        public static async Task updateUser(User user, ILogger log)
        {
            try
            {
                await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(R.DB_NAME, R.DB_COL_USER, user.userId), user);
            }
            catch (Exception e)
            {
                log.LogInformation(e.ToString());
                throw e;
            }
        }

    }

}
