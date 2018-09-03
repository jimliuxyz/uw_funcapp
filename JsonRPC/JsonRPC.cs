
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
    class JsonRpcQuery
    {
        public string jsonrpc;
        public string method;
        public int id;

        [JsonProperty(PropertyName = "params")]
        public dynamic params_;
    }

    public class JsonRpcRes
    {
        [JsonProperty]
        private string jsonrpc = "2.0";

        [JsonProperty]
        private int id = 0;

        [JsonProperty]
        object result = null;

        [JsonProperty]
        JsonRpcError error = null;

        private JsonRpcRes(int id, object result = null, JsonRpcError error = null)
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
            return error == null ? new OkObjectResult(this.ToString()) : new BadRequestObjectResult(this.ToString()) as IActionResult;
        }

        public static JsonRpcRes InternalError(int id)
        {
            return new JsonRpcRes(id, error: new JsonRpcError(-32603, "Internal error"));
        }
        public static JsonRpcRes Unauthorized(int id)
        {
            return new JsonRpcRes(id, error: new JsonRpcError(-32600, "Unauthorized"));
        }
        public static JsonRpcRes InvalidRequest(int id, string message = "Invalid Request")
        {
            return new JsonRpcRes(id, error: new JsonRpcError(-32600, message));
        }

        public static JsonRpcRes Ok(int id, object result = null)
        {
            return new JsonRpcRes(id, result);
        }
    }

    public class JsonRpcError
    {
        [JsonProperty]
        private int code = 0;

        [JsonProperty]
        private string message = null;

        public JsonRpcError(int code, string message)
        {
            this.code = code;
            this.message = message;
        }
    }
}
