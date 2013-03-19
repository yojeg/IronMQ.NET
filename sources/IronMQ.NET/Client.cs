namespace IronMQ
{
    using System.IO;
    using System.Net;
    using System.Web.Script.Serialization;
    using Data;

    public class Client : IClient
    {
        private const string Protocol = "https";
        private const string HostDefault = "mq-aws-us-east-1.iron.io";
        private const int PortDefault = 443;
        private const string ApiVersion = "1";

        private readonly string _projectId = string.Empty;
        private readonly string _token = string.Empty;

        public string Host { get; private set; }
        public int Port { get; private set; }

        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        /// <summary>
        /// Constructs a new Client using the specified project ID and token.
        ///  The network is not accessed during construction and this call will
        /// succeed even if the credentials are invalid.
        /// </summary>
        /// <param name="projectId">projectId A 24-character project ID.</param>
        /// <param name="token">token An OAuth token.</param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public Client(string projectId, string token, string host = HostDefault, int port = PortDefault)
        {
            _projectId = projectId;
            _token = token;

            Host = host;
            Port = port;
        }

        public IQueue Queue(string name)
        {
            return new Queue (this, name);
        }

        public string[] Queues(int page = 0)
        {
            var ep = "queues";
            if (page != 0)
                ep += string.Format("?page={0}", page);

            return _serializer.Deserialize<string[]>(Get(ep));
        }

        public string Delete(string endpoint)
        {
            return Request("DELETE", endpoint, null);
        }

        public string Get(string endpoint)
        {
            return Request("GET", endpoint, null);
        }

        public string Post(string endpoint, string body)
        {
            return Request("POST", endpoint, body);
        }

        private string Request(string method, string endpoint, string body)
        {
            var path = string.Format("/{0}/projects/{1}/{2}", ApiVersion, _projectId, endpoint);
            var uri = string.Format("{0}://{1}:{2}{3}", Protocol, Host, Port, path);

            var request = (HttpWebRequest) WebRequest.Create(uri);

            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "OAuth " + _token);
            request.UserAgent = "IronMQ .Net Client";
            request.Method = method;

            if (body != null)
                using (var write = new StreamWriter(request.GetRequestStream()))
                {
                    write.Write(body);
                    write.Flush();
                }

            var response = (HttpWebResponse) request.GetResponse();
            string json;
            using (var reader = new StreamReader(response.GetResponseStream()))
                json = reader.ReadToEnd();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var error = _serializer.Deserialize<Error>(json);
                throw new System.Web.HttpException((int) response.StatusCode, error.msg);
            }

            return json;
        }
    }
}
