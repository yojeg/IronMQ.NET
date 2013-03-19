namespace IronMQ
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class IronMQIntegrationTests
    {
        private const string TestQueueName = "test-queue";

        private readonly string _projectId;
        private readonly string _token;

        public IronMQIntegrationTests()
        {
            _projectId = ConfigurationManager.AppSettings["IRONIO_PROJECT_ID"];
            _token = ConfigurationManager.AppSettings["IRONIO_TOKEN"];

            Assert.IsFalse(string.IsNullOrWhiteSpace(_projectId));
            Assert.IsFalse(string.IsNullOrWhiteSpace(_token));
        }

        [Test]
        public void BasicTests()
        {
            var client = new Client(_projectId, _token);
            var queue = client.Queue(TestQueueName);

            ClearQueue(queue);
            // clear_queue
            queue.Push("hello world!");
            var message = queue.Get();
            Assert.IsNotNull(message.Id);
            Assert.IsNotNull(message.Body);

            queue.DeleteMessage(message.Id);
            message = queue.Get();
            Assert.IsNull(message);

            queue.Push("hello world 2!");

            message = queue.Get();
            Assert.IsNotNull(message);

            queue.DeleteMessage(message.Id);

            message = queue.Get();
            Assert.IsNull(message);
        }

        [Test]
        public void BulkGetTest()
        {
            var client = new Client(_projectId, _token);
            var queue = client.Queue(TestQueueName);
            ClearQueue(queue);

            var messages = Enumerable.Range(0, 10)
                .Select(i => i.ToString(CultureInfo.InvariantCulture))
                .ToArray();
            queue.Push(messages);

            var actual = queue.Get(100);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 1);
        }

        [Test]
        public void BulkPushTest()
        {
            var client = new Client(_projectId, _token);
            var queue = client.Queue(TestQueueName);

            ClearQueue(queue);
            var messages = Enumerable.Range(0, 10)
                .Select(i => i.ToString(CultureInfo.InvariantCulture))
                .ToArray();
            queue.Push(messages);

            for (var i = 0; i < 10; i++)
            {
                var message = queue.Get();
                Assert.IsNotNull(message);
            }
            // Assumption is that if we queued up 1000 and we got back 1000 then it worked fine.
            // Note: this does not verify we got back the same messages
        }

        [Test]
        public void ClearEmptyQueueTest()
        {
            var client = new Client(_projectId, _token);
            var queue = client.Queue(TestQueueName);
            ClearQueue(queue);
            // At this point the queue should be empty
            queue.Clear();
        }

        [Test]
        public void ClearQueueTest()
        {
            const string messageBody = "This is a test of the emergency broadcasting system... Please stand by...";

            var client = new Client(_projectId, _token);
            var queue = client.Queue(TestQueueName);
            ClearQueue(queue);

            queue.Push(messageBody);
            queue.Clear();

            var message = queue.Get();
            Assert.IsNull(message);
        }

        [Test]
        public void TestMethod1()
        {
            const string body = "Hello, IronMQ!";

            var client = new Client(_projectId, _token);
            var queue = client.Queue(TestQueueName);
            ClearQueue(queue);

            queue.Push(body);

            var message = queue.Get();
            Assert.IsTrue(String.CompareOrdinal(body, message.Body) == 0);
            queue.DeleteMessage(message);
        }

        private static void ClearQueue(IQueue queue)
        {
            try
            {
                queue.Clear();
            }
            catch
            {
                //TODO: This is here because of a bug in the Endpoint where clearing an empty queue results in a 500 internal server error.
            }
        }
    }
}