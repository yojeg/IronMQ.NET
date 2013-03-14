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
    public class Queue
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

        /// <summary>
        /// Clears a Queue regardless of message status
        /// </summary>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public void Clear()
        {
            const string emptyJsonObject = "{}";

            var endpoint = string.Format("queues/{0}/clear", _name);
            var response = _client.Post(endpoint, emptyJsonObject);
            var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(response,_settings);

            if (responseObject["msg"] != "Cleared")
                throw new Exception(string.Format("Unknown response from REST Endpoint : {0}", response));
        }


        /// <summary>
        /// Retrieves a Message from the queue. If there are no items on the queue, an HTTPException is thrown.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public Message Get()
        {
            var endpoint = string.Format("queues/{0}/messages", _name);
            var json = _client.Get(endpoint);
            var queueResp = JsonConvert.DeserializeObject<QueueMessages>(json, _settings);

            return queueResp.messages.Length > 0 ? queueResp.messages[0] : null;
        }

        /// <summary>
        /// Retrieves up to "max" messages from the queue
        /// </summary>
        /// <param name="max">the count of messages to return, default is 1</param>
        /// <returns>An IList of messages</returns>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public IList<Message> Get(int max)
        {
            var endpoint = string.Format("queues/{0}/messages?n={1}", _name, max);
            var json = _client.Get(endpoint);
            var queueResp = JsonConvert.DeserializeObject<QueueMessages>(json,_settings);

            return queueResp.messages;
        }

        /// <summary>
        /// Delete a message from the queue
        /// </summary>
        /// <param name="id">Message Identifier</param>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public void DeleteMessage(string id)
        {
            var endpoint = string.Format("queues/{0}/messages/{1}", _name, id);
            _client.Delete(endpoint);
        }

        /// <summary>
        /// Delete a message from the queue
        /// </summary>
        /// <param name="msg">Message to be deleted</param>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public void DeleteMessage(Message msg)
        {
            DeleteMessage(msg.Id);
        }

        /// <summary>
        /// Pushes a message onto the queue with a timeout
        /// </summary>
        /// <param name="msg">Message to be pushed.</param>
        /// <param name="timeout">The timeout of the message to push.</param>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public void Push(string msg, long timeout = 0)
        {
            Push(new[] { msg }, timeout);
        }

        /// <summary>
        /// Pushes messages onto the queue with an optional timeout
        /// </summary>
        /// <param name="messages">Messages to be pushed.</param>
        /// <param name="timeout">The timeout of the messages to push.</param>
        /// <param name="delay"> </param>
        /// <param name="expiresIn"> </param>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
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