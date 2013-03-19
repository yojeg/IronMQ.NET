namespace IronMQ
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Newtonsoft.Json;

    /// <summary>
    /// Represends a specific IronMQ Queue.
    /// </summary>
    public class Queue : IQueue
    {
        private readonly JsonSerializerSettings _settings
            = new JsonSerializerSettings
                  {
                      NullValueHandling = NullValueHandling.Ignore,
                      Formatting = Formatting.None,
                      DefaultValueHandling = DefaultValueHandling.Ignore
                  };

        private readonly Client _client;
        private readonly string _name;

        public Queue(Client client, string name)
        {
            _client = client;
            _name = name;
        }

        public void Clear()
        {
            const string emptyJsonObject = "{}";

            var endpoint = string.Format("queues/{0}/clear", _name);
            var response = _client.Post(endpoint, emptyJsonObject);
            var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(response,_settings);

            if (responseObject["msg"] != "Cleared")
                throw new Exception(string.Format("Unknown response from REST Endpoint : {0}", response));
        }

        public Message Get()
        {
            var endpoint = string.Format("queues/{0}/messages", _name);
            var json = _client.Get(endpoint);
            var queueResp = JsonConvert.DeserializeObject<QueueMessages>(json, _settings);

            return queueResp.messages.Length > 0 ? queueResp.messages[0] : null;
        }

        public IList<Message> Get(int max)
        {
            var endpoint = string.Format("queues/{0}/messages?n={1}", _name, max);
            var json = _client.Get(endpoint);
            var queueResp = JsonConvert.DeserializeObject<QueueMessages>(json,_settings);

            return queueResp.messages;
        }

        public void DeleteMessage(string id)
        {
            var endpoint = string.Format("queues/{0}/messages/{1}", _name, id);
            _client.Delete(endpoint);
        }

        public void DeleteMessage(Message msg)
        {
            DeleteMessage(msg.Id);
        }

        public void Push(string msg, long timeout = 0)
        {
            Push(new[] { msg }, timeout);
        }

        public void Push(IEnumerable<string> messages, long timeout = 0, long delay = 0, long expiresIn = 0)
        {
            var json = JsonConvert.SerializeObject(
                new QueueMessages
                    {
                        messages = messages
                            .Select(msg => new Message
                                               {
                                                   Body = msg,
                                                   Timeout = timeout,
                                                   Delay = delay,
                                                   Expires_In = expiresIn
                                               })
                            .ToArray(),
                    }, _settings);

            var endpoint = string.Format("queues/{0}/messages", _name);
            _client.Post(endpoint, json);
        }
    }
}