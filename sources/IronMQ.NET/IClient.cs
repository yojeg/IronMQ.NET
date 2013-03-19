namespace IronMQ
{
    public interface IClient
    {
        string Host { get; }
        int Port { get; }

        /// <summary>
        /// Returns a Queue using the given name.
        /// The network is not accessed during this call.
        /// </summary>
        /// <param name="name">param name The name of the Queue to create.</param>
        /// <returns></returns>
        IQueue Queue(string name);

        /// <summary>
        /// Returns list of queues
        /// </summary>
        /// <param name="page">Queue list page</param>
        string[] Queues(int page = 0);

        string Delete(string endpoint);
        string Get(string endpoint);
        string Post(string endpoint, string body);
    }
}