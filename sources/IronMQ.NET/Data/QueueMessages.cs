namespace IronMQ.Data
{
    using System;

    [Serializable]
    public class QueueMessages
    {
// ReSharper disable InconsistentNaming
        public Message[] messages{ get; set; }       
// ReSharper restore InconsistentNaming
    }
}
