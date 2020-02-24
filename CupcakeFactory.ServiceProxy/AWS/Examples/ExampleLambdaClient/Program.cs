using Amazon.Runtime.CredentialManagement;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CupcakeFactory.ServiceProxy;
using CupcakeFactory.ServiceProxy.Lambda;
using ExampleContracts;
using System.Threading.Tasks;

namespace ExampleLambdaClient
{
    public class Program
    {
        [RPlotExporter]
        public class LambdaBenchmarks
        {
            IAdditionService _service;

            public LambdaBenchmarks()
            {
                var credentialsFile = new SharedCredentialsFile();
                CredentialProfile profile;
                credentialsFile.TryGetProfile("CupcakeFactory", out profile);

                var credentials = profile.GetAWSCredentials(credentialsFile).GetCredentials();

                var dispatcher = new LambdaDispatcher<IAdditionService>("cupcake-example-service", "us-east-1", credentials);
                _service = ServiceProxy<IAdditionService>.GetProxy(dispatcher);
            }

            [Benchmark]
            public int Add2Numbers() => _service.Add(1, 3);
        }

        public static async Task Main(string[] args)
        {
            var credentialsFile = new SharedCredentialsFile();
            CredentialProfile profile;
            credentialsFile.TryGetProfile("CupcakeFactory", out profile);

            var credentials = await profile.GetAWSCredentials(credentialsFile).GetCredentialsAsync();

            var dispatcher = new LambdaDispatcher<IAdditionService>("cupcake-example-service", "us-east-1", credentials);
            IAdditionService service = ServiceProxy<IAdditionService>.GetProxy(dispatcher);

            var result = service.Add(1, 3);

            //Uncomment to benchmark your results
            BenchmarkRunner.Run<LambdaBenchmarks>();
        }
    }
}
