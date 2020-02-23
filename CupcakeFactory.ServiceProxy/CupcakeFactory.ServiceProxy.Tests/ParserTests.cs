using CupcakeFactory.ServiceProxy.Dispatchers;
using CupcakeFactory.ServiceProxy.Serializers;
using NUnit.Framework;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Tests
{
    [TestFixture]
    public class ParserTests
    {
        IDispatch _dispatcher = new InProcDispatcher<ITestService>(new JsonProxySerializer<ITestService>(), new TestService());


        [TestCase]
        public async Task CanParseArguments()
        {
            var proxy = ServiceProxy<ITestService>.GetProxy(_dispatcher);
            
            var result = await proxy.DoWorkWithTaskAndMultipleUserTypeParameters(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject());
        }

        [TestCase]
        public void CanInvokeSyncReturn()
        {
            var proxy = ServiceProxy<ITestService>.GetProxy(_dispatcher);

            var result = proxy.DoWorkSync(596);
        }

        [TestCase]
        public async Task CanThrowExceptionWithGenericTask()
        {
            var proxy = ServiceProxy<ITestService>.GetProxy(_dispatcher);

            try
            {
                var result = await proxy.ThisGenericTaskAlwaysFails(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject());
            }
            catch (Exception ex)
            {
                if (ex.Message != "I Failed")
                    Assert.Fail("Wrong Exception Thrown");

                return;
            }

            Assert.Fail("Exception did not throw");
        }
    }
}
