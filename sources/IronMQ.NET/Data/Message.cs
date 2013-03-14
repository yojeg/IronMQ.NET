namespace IronMQ.Data
{
    using System;
    using Newtonsoft.Json;
    using System.ComponentModel;

    [Serializable]
    [JsonObject]
    public class Message
    {
// ReSharper disable InconsistentNaming
        public string Id { get; set; }

        public string Body { get; set; }

        [DefaultValue(0)]
        public long Timeout { get; set; }

        [DefaultValue(0)]
        public long Delay { get; set; }

        [DefaultValue(0)] 

        public long Expires_In { get; set; }
// ReSharper restore InconsistentNaming
    }
}
