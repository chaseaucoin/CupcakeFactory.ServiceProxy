using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeFactory.ServiceProxy.Tests
{
    public interface ITestService
    {
        void DoWorkWithVoidAndNoParameters();

        void DoWorkWithVoidAndMultipleBuiltInParameters(long someLong, string someString);

        Task DoWorkWithTaskAndMultipleBuiltInParameters(long someLong, string someString);

        Task<SimpleObject> DoWorkWithTaskAndMultipleUserTypeParameters(SimpleObject simpleObject, ComplexObject complexObject, SelfReferenceingObject selfRefObject);

        Task<SimpleObject> ThisGenericTaskAlwaysFails(SimpleObject simpleObject, ComplexObject complexObject, SelfReferenceingObject selfRefObject);

        Task ThisTaskAlwaysFails(SimpleObject simpleObject, ComplexObject complexObject, SelfReferenceingObject selfRefObject);

        void ThisVoidAlwaysFails();
    }

    public class TestService : ITestService
    {
        public Task DoWorkWithTaskAndMultipleBuiltInParameters(long someLong, string someString)
        {
            return Task.FromResult(true);
        }

        public Task<SimpleObject> DoWorkWithTaskAndMultipleUserTypeParameters(SimpleObject simpleObject, ComplexObject complexObject, SelfReferenceingObject selfRefObject)
        {
            return Task.FromResult(simpleObject);
        }

        public void DoWorkWithVoidAndMultipleBuiltInParameters(long someLong, string someString)
        {
            
        }

        public void DoWorkWithVoidAndNoParameters()
        {

        }

        public int Add(int x, int y)
        {
            return x + y;
        }

        public Task<SimpleObject> ThisGenericTaskAlwaysFails(SimpleObject simpleObject, ComplexObject complexObject, SelfReferenceingObject selfRefObject)
        {
            throw new Exception("I Failed");
        }

        public Task ThisTaskAlwaysFails(SimpleObject simpleObject, ComplexObject complexObject, SelfReferenceingObject selfRefObject)
        {
            throw new Exception("I Failed");
        }

        public void ThisVoidAlwaysFails()
        {
            throw new Exception("I Failed");
        }
    }

    public interface IOtherService
    {
        public int Add(int x, int y);
    }
}
