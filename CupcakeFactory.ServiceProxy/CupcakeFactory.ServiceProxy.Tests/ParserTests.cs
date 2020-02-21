using CupcakeFactory.ServiceProxy.Dispatchers;
using CupcakeFactory.ServiceProxy.Serializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Tests
{
    [TestClass]
    public class ParserTests
    {
        IDispatch _dispatcher = new InProcDispatcher<ITestService>(new JsonProxySerializer<ITestService>(), new TestService());


        [TestMethod]
        public async Task CanParseArguments()
        {
            var proxy = ServiceProxy<ITestService>.GetProxy(_dispatcher);
            
            var result = await proxy.DoWorkWithTaskAndMultipleUserTypeParameters(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject());
        }

        [TestMethod]
        public void CanInvokeSyncReturn()
        {
            var proxy = ServiceProxy<ITestService>.GetProxy(_dispatcher);

            var result = proxy.DoWorkSync(596);
        }

        [TestMethod]
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
