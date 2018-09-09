
namespace UW
{
    public partial class RPCERR
    {
        public static readonly RPCERR SMS_RESEND_EXCEEDED = new RPCERR(-1001, "SMS resend limit Exceeded");
        public static readonly RPCERR ACTION_FAILED = new RPCERR(-1002, "Action failed");

        public static readonly RPCERR PASSCODE_EXPIRED = new RPCERR(-1003, "Passcode expired");
        public static readonly RPCERR PASSCODE_VERIFY_EXCEEDED = new RPCERR(-1004, "Passcode verify limit Exceeded");
    }


    public partial class RPCERR
    {
        public readonly int code;
        public readonly string msg;
        private RPCERR(int code, string msg)
        {
            this.code = code;
            this.msg = msg;
        }
    }
}