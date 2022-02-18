using Moq;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Microsoft.Extensions.Configuration;

namespace MyAWSLambdaFunc.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestIPInfoFunction()
        {
            var configuration = new Mock<IConfiguration>();
            var ipService = new Mock<IMyIPService>();

            // Setup
            var testOutput = "xyz";
            ipService.Setup(s => s.GetIPInfo(It.IsAny<string>())).Returns(testOutput);

            // Action: Invoke the lambda function 
            var context = new TestLambdaContext();
            var function = new MyFunction(configuration.Object, ipService.Object);
            var ipInfo = function.FunctionHandler("161.185.160.93", context);

            // Verify
            Assert.NotNull(ipInfo);
            Assert.Equal(ipInfo, testOutput);
        }
    }
}
