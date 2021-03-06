using CupcakeFactory.ServiceProxy.Dispatchers;
using CupcakeFactory.ServiceProxy.Serializers;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Tests
{
    [TestFixture]
    public class InProcTests
    {
        public static IEnumerable<TestCaseData> HappyPathCaseData()
        {
            IDispatch _dispatcher = new InProcDispatcher<ITestService>(new JsonProxySerializer(), new TestService());
            return new TestBuilder(_dispatcher).HappyPathCaseData();
        }

        public static IEnumerable<TestCaseData> FailedCaseData()
        {
            IDispatch _dispatcher = new InProcDispatcher<ITestService>(new JsonProxySerializer(), new TestService());
            return new TestBuilder(_dispatcher).FailedCaseData();
        }

        [TestCaseSource("HappyPathCaseData")]
        public void TestHapyyPath(Func<Task> task)
        {
            task();
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
