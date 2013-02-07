namespace IronMQ.Data
{
    using System;

    [Serializable]
    public class QueueMessages
    {
        public Message[] messages{ get; set; }       
    }
}
