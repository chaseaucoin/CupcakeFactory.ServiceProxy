using CupcakeFactory.ServiceProxy.Dispatchers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CupcakeFactory.ServiceProxy.Tests
{
    public class TestBuilder
    {
        List<Action> _tests;
        ITestService _service;
        IDispatch _dispatcher;


        public TestBuilder(IDispatch dispatcher)
        {
            _dispatcher = dispatcher;
            _service = ServiceProxy<ITestService>.GetProxy(dispatcher);
        }

        private IEnumerable<KeyValuePair<string, Action>> PositiveTests()
        {
            yield return KeyValuePair.Create<string, Action>("DoWorkSync", () => _service.DoWorkSync(1));
            yield return KeyValuePair.Create<string, Action>("DoWorkWithTaskAndMultipleBuiltInParameters", () => _service.DoWorkWithTaskAndMultipleBuiltInParameters(2, "TestString"));
            yield return KeyValuePair.Create<string, Action>("DoWorkWithTaskAndMultipleUserTypeParameters", () => _service.DoWorkWithTaskAndMultipleUserTypeParameters(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject()));
            yield return KeyValuePair.Create<string, Action>("DoWorkWithVoidAndMultipleBuiltInParameters", () => _service.DoWorkWithVoidAndMultipleBuiltInParameters(2, "TestString"));
            yield return KeyValuePair.Create<string, Action>("DoWorkWithVoidAndNoParameters", () => _service.DoWorkWithVoidAndNoParameters());
        }

        private IEnumerable<KeyValuePair<string, Action>> NegativeTests()
        {
            yield return KeyValuePair.Create<string, Action>("ThisGenericTaskAlwaysFails", () => _service.ThisGenericTaskAlwaysFails(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject()));
            yield return KeyValuePair.Create<string, Action>("ThisTaskAlwaysFails", () => _service.ThisTaskAlwaysFails(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject()));
            yield return KeyValuePair.Create<string, Action>("ThisVoidAlwaysFails", () => _service.ThisVoidAlwaysFails());
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
