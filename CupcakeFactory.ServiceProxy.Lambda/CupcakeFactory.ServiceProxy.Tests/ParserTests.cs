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
        [TestMethod]
        public async Task CanParseArguments()
        {
            var proxy = ServiceProxy<ITestService>.GetProxy(new TestService(), new JsonSerializer<ITestService>());
            
            var result = await proxy.DoWorkWithTaskAndMultipleUserTypeParameters(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject());
        }

        [TestMethod]
        public async Task CanParseArguments2()
        {
            var proxy = ServiceProxy<ITestService>.GetProxy(new TestService(), new JsonSerializer<ITestService>());

            var result = await proxy.DoWorkWithTaskAndMultipleUserTypeParameters(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject());
        }

        [TestMethod]
        public async Task CanThrowExceptionWithGenericTask()
        {
            var proxy = ServiceProxy<ITestService>.GetProxy(new TestService(), new JsonSerializer<ITestService>());

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
