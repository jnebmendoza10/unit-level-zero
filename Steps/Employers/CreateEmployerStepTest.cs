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
    public class CreateEmployerStepTest
    {
        private CreateEmployerStep target;
        private Mock<IEmployerService> employerService;

        [TestInitialize]
        public void Setup()
        {
            this.employerService = new Mock<IEmployerService>();

            this.target = new CreateEmployerStep(this.employerService.Object);
        }

        [TestCleanup]
        public void Teardown()
        {
            this.employerService = null;

            this.target = null;
        }

        [TestMethod]
        public async Task ExecuteAsync_UserIdIsNullOrEmptyGuid_ResponseHasExpectedErrorCode()
        {
            //// Arrange
            var expected = "onboarding/unknown-user";

            var request = new EmployerOnboardingRequest();
            var response = new Response<Employer>();
            var stepRequest = new EmployerOnboardingStepRequest(request, response) {
                UserId = Guid.Empty
            };

            //// Act
            var actual = await this.target.ExecuteAsync(stepRequest);

           //// Assert
            Assert.AreEqual(expected, actual.ErrorCode);
        }

        [TestMethod]
        public async Task ExecuteAsync_EmployerCreatedWithoutError_ResponseDataIsSet()
        {
            //// Arrange
            var mockId = Guid.NewGuid();
            var mockGuidResponse = new Response<Guid>() { 
                Data = mockId
            };
            var mockEmployer = new Employer(mockGuidResponse.Data, "Test");
            var request = new EmployerOnboardingRequest();
            var expected = new Response<Employer>(mockEmployer);
            this.employerService.Setup(s => s.CreateAsync(It.IsAny<CreateEmployerRequest>())).ReturnsAsync(mockGuidResponse);

            //// Act
            var actual = await this.target.ExecuteAsync(new EmployerOnboardingStepRequest(request, expected));

            //// Assert
            Assert.AreEqual(expected.Data, actual.Data);
        }

        [TestMethod]
        public async Task ExecuteAsync_EmployerCreatedWithError_ResponseHasExpectedErrorCode()
        {
            //// Arrange
            var mockId = Guid.NewGuid();
            var mockGuidResponse = new Response<Guid>()
            {
                Data = mockId,
            };
            mockGuidResponse.SetError("mock-error");
            var employer = new Employer(mockGuidResponse.Data, "Test");
            var request = new EmployerOnboardingRequest();

            var expected = new Response<Employer>(employer);
            expected.SetError(mockGuidResponse.ErrorCode);
            this.employerService.Setup(s => s.CreateAsync(It.IsAny<CreateEmployerRequest>())).ReturnsAsync(mockGuidResponse);

            //// Act
            var actual = await this.target.ExecuteAsync(new EmployerOnboardingStepRequest(request, expected));

            //// Assert
            Assert.AreEqual(expected.ErrorCode, actual.ErrorCode);
        }
    }
}
