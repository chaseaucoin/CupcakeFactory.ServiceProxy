using CupcakeFactory.ServiceProxy.Dispatchers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Tests.AWS
{
    [TestFixture]
    public class LambdaLocalTests
    {
        public static IEnumerable<TestCaseData> HappyPathCaseData()
        {
            IDispatch _dispatcher = new LambdaTestDispatcher<ITestService>(new TestService());
            return new TestBuilder(_dispatcher).HappyPathCaseData();
        }

        public static IEnumerable<TestCaseData> FailedCaseData()
        {
            IDispatch _dispatcher = new LambdaTestDispatcher<ITestService>(new TestService());
            return new TestBuilder(_dispatcher).FailedCaseData();
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
