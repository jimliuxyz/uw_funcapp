
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace UwFuncapp
{
    public class JsonRPC
    {
        [JsonProperty]
        private string jsonrpc = "2.0";

        [JsonProperty]
        private int id = 0;

        [JsonProperty]
        object result = null;

        [JsonProperty]
        JsonRPCError error = null;

        private JsonRPC(int id, object result = null, JsonRPCError error = null)
        {
            this.id = id;
            this.result = result;
            this.error = error;
        }

        public string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public IActionResult ToActionResult()
        {
            return error==null ? new OkObjectResult(this.ToString()) : new BadRequestObjectResult(this.ToString()) as IActionResult;
        }

        public static JsonRPC InternalError(int id)
        {
            return new JsonRPC(id, error: new JsonRPCError(-32603, "Internal error"));
        }
        public static JsonRPC Unauthorized(int id)
        {
            return new JsonRPC(id, error: new JsonRPCError(-32600, "Unauthorized"));
        }
        public static JsonRPC InvalidRequest(int id, string message = "Invalid Request")
        {
            return new JsonRPC(id, error: new JsonRPCError(-32600, message));
        }

        public static JsonRPC Ok(int id, object result = null)
        {
            return new JsonRPC(id, result);
        }
    }

    public class JsonRPCError
    {
        [JsonProperty]
        private int code = 0;

        [JsonProperty]
        private string message = null;

        public JsonRPCError(int code, string message)
        {
            this.code = code;
            this.message = message;
        }
    }
}
