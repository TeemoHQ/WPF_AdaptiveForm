using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WaitingRoomBigScreen.WebService
{
    public interface IRes
    {
        bool success { get; set; }
        string resultCode { get; set; }
        string msg { get; set; }
        long startTime { get; set; }
        long timeConsum { get; set; }
    }

    public class Res : IRes
    {
        public bool success { get; set; }
        public string resultCode { get; set; }
        public string msg { get; set; }
        public long startTime { get; set; }
        public long timeConsum { get; set; }
    }

    public interface IReq
    {
        string serviceUrl { get; }

        Dictionary<string, string> GetParams();
    }

    public class Req : IReq
    {
        public virtual string serviceUrl => string.Empty;

        public virtual Dictionary<string, string> GetParams()
        {
            return new Dictionary<string, string>();
        }
    }

    public class ReqQueue : Req
    {
        public override string serviceUrl { get; } = "queryHz";
        public string corpId { get; set; }
        public string token { get; set; }
    }

    public class ResQueue : Res
    {
        public List<QueueInfo> data { get; set; }
    }

    public class ReqToken : Req
    {
        public override string serviceUrl { get; } = "device/getAccessToken";
        public string deviceCode { get; set; }
        public string deviceSecret { get; set; }
        public string corp_code { get; set; }
    }

    public class ResToken : Res
    {
        public string data { get; set; }
    }
    public class ReqInitDevie : Req
    {
        public override string serviceUrl { get; } = "device/initDevice";
        public string deviceMac { get; set; }
    }

    public class ResInitDevie : Res
    {
        public bool data { get; set; }
    }
    public class ReqGetSecret : Req
    {
        public override string serviceUrl { get; } = "device/getSecret";
        public string deviceMac { get; set; }
    }

    public class ResGetSecret : Res
    {
        public string data { get; set; }
    }
    public class QueueInfo
    {
        public string num { get; set; }
        public string queueName { get; set; }
        public string closed { get; set; }
        public List<Patient> Patients { get; set; }
        public List<Patient> patientGH { get; set; }
    }

    public class Patient
    {
        public int id { get; set; }
        public string queueId { get; set; }
        public string orderNo { get; set; }
        public string orderType { get; set; }
        public decimal weight { get; set; }
        public string username { get; set; }
        public string sex { get; set; }
        public string patientNo { get; set; }
        public string doctor { get; set; }
        public string zhenshi { get; set; }
        public string keshi { get; set; }
        public int callTimes { get; set; }
        public int status { get; set; }
        public long creatTime { get; set; }
        public long modeifyTime { get; set; }
        public int age { get; set; }
        public long mobile { get; set; }
        public int isBack { get; set; }
        public string queueCode { get; set; }
        public string queueDate { get; set; }
        public string queueName { get; set; }
        public string queueType { get; set; }
        public string deptName { get; set; }
        public string deptCode { get; set; }
        public string back { get; set; }
    }
}
