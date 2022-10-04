namespace Yousource.UnitTesting.Exercise.Tests.Steps.Employers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Yousource.UnitTesting.Exercise.Interfaces;
    using Yousource.UnitTesting.Exercise.Messages;
    using Yousource.UnitTesting.Exercise.Models;
    using Yousource.UnitTesting.Exercise.Steps.Employers;

    /// <summary>
    /// Instructions: Write unit tests for the class CreateEmployerStep.cs. Ensure that all scenarios are covered (lines of code)
    /// </summary>
    [TestClass]
    public class SignUpEmployerStepTest
    {
        private SignUpEmployerStep target;
        private Mock<IIdentityService> identityService;

        [TestInitialize]
        public void Setup()
        {
            this.identityService = new Mock<IIdentityService>();

            this.target = new SignUpEmployerStep(this.identityService.Object);
        }

        [TestCleanup]
        public void Teardown()
        {
            this.identityService = null;

            this.target = null;
        }

        [TestMethod]
        public async Task ExecuteAsync_SignUpHasError_ResponseHasExpectedErrorCode()
        {
            //// Arrange
            var request = new EmployerOnboardingRequest();
            var response = new Response<Employer>();
            var stepRequest = new EmployerOnboardingStepRequest(request, response);
            var expected = new Response<Guid>();
            expected.SetError("mock-error");

            this.identityService.Setup(s => s.SignUpAsync(It.IsAny<SignUpRequest>())).ReturnsAsync(expected);

            //// Act
            var actual = await this.target.ExecuteAsync(stepRequest);

            //// Assert
            Assert.AreEqual(expected.ErrorCode, actual.ErrorCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_SignUpSuccessful_RequestUserIdIsSet()
        {
            //// Arrange
            var mockId = Guid.NewGuid();
            var employer = new Employer(mockId, "test-name");
            var request = new EmployerOnboardingRequest();
            var response = new Response<Employer>(employer);
            var stepRequest = new EmployerOnboardingStepRequest(request, response) {
                UserId = Guid.Empty
            };
            var expected = new Response<Guid>() { 
                Data = mockId
            };

            this.identityService.Setup(s => s.SignUpAsync(It.IsAny<SignUpRequest>())).ReturnsAsync(expected);

            //// Act
            var actual = await this.target.ExecuteAsync(stepRequest);

            //// Assert
            Assert.AreEqual(expected.Data, actual.Data.Id);
        }
    }
}
