using CupcakeFactory.ServiceProxy.Dispatchers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Tests
{
    public class TestBuilder
    {
        List<Func<Task>> _tests;
        ITestService _service;
        IDispatch _dispatcher;


        public TestBuilder(IDispatch dispatcher)
        {
            _dispatcher = dispatcher;
            _service = ServiceProxy<ITestService>.GetProxy(dispatcher);
        }

        private IEnumerable<KeyValuePair<string, Func<Task>>> PositiveTests()
        {
            yield return KeyValuePair.Create<string, Func<Task>>("DoWorkSync", () => Task.FromResult(_service.DoWorkSync(1)));
            yield return KeyValuePair.Create<string, Func<Task>>("DoWorkWithTaskAndMultipleBuiltInParameters", async () => await _service.DoWorkWithTaskAndMultipleBuiltInParameters(2, "TestString"));
            yield return KeyValuePair.Create<string, Func<Task>>("DoWorkWithTaskAndMultipleUserTypeParameters", async () => await _service.DoWorkWithTaskAndMultipleUserTypeParameters(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject()));
            yield return KeyValuePair.Create<string, Func<Task>>("DoWorkWithVoidAndMultipleBuiltInParameters", () => Task.Run(() => _service.DoWorkWithVoidAndMultipleBuiltInParameters(2, "TestString")));
            yield return KeyValuePair.Create<string, Func<Task>>("DoWorkWithVoidAndNoParameters", () => Task.Run(() => _service.DoWorkWithVoidAndNoParameters()));
        }

        private IEnumerable<KeyValuePair<string, Func<Task>>> NegativeTests()
        {
            yield return KeyValuePair.Create<string, Func<Task>>("ThisGenericTaskAlwaysFails", async () => await _service.ThisGenericTaskAlwaysFails(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject()));
            yield return KeyValuePair.Create<string, Func<Task>>("ThisTaskAlwaysFails", async () => await _service.ThisTaskAlwaysFails(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject()));
            yield return KeyValuePair.Create<string, Func<Task>>("ThisVoidAlwaysFails", () => Task.Run(() => _service.ThisVoidAlwaysFails()));
        }

        public IEnumerable<TestCaseData> HappyPathCaseData()
        {
            var dispatcherName = _dispatcher.GetType().Name;

            foreach (var test in PositiveTests())
            {
                yield return new TestCaseData(test.Value)
                    .SetName($"{dispatcherName} ITestService.{test.Key}");
            }
        }

        public IEnumerable<TestCaseData> FailedCaseData()
        {
            var dispatcherName = _dispatcher.GetType().Name;

            foreach (var test in NegativeTests())
            {
                yield return new TestCaseData(test.Value)
                    .SetName($"{dispatcherName} ITestService.{test.Key}");
            }
        }
    }
}
