using CupcakeFactory.ServiceProxy.Dispatchers;
using CupcakeFactory.ServiceProxy.HTTP;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Tests.HTTP
{
    [TestFixture]
    public class HTTPTests
    {
        static TestBuilder _testBuilder;
        TestServer _testServer;
        HttpClient _client;
        public HTTPTests()
        {

        }

        public static TestBuilder GetTestBuilder()
        {
            if (_testBuilder != null)
                return _testBuilder;

            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>();

            var _testServer = new TestServer(builder);
            var _client = _testServer.CreateClient();
            IDispatch _dispatcher = new HttpDispatcher<ITestService>(_client);
            return new TestBuilder(_dispatcher);
        }

        public static IEnumerable<TestCaseData> HappyPathCaseData()
        {
            return GetTestBuilder().HappyPathCaseData();
        }

        public static IEnumerable<TestCaseData> FailedCaseData()
        {
            return GetTestBuilder().FailedCaseData();
        }

        [TestCaseSource("HappyPathCaseData")]
        public async Task TestHapyyPath(Func<Task> task)
        {
            await task();
        }

        [TestCaseSource("FailedCaseData")]
        public async Task TestFailedPath(Func<Task> task)
        {
            Exception e = null;

            try
            {
                await task();
            }
            catch (Exception ex)
            {
                e = ex;
            }

            Assert.That(e != null);
            Assert.That(e.Message == "I Failed");
        }
    }
}
