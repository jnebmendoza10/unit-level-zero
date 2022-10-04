namespace Yousource.UnitTesting.Exercise.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Yousource.UnitTesting.Exercise.Exceptions;
    using Yousource.UnitTesting.Exercise.Interfaces;
    using Yousource.UnitTesting.Exercise.Messages;
    using Yousource.UnitTesting.Exercise.Models;
    using Yousource.UnitTesting.Exercise.Steps;
    using Yousource.UnitTesting.Exercise.Steps.Employers;

    /// <summary>
    /// Instructions: Write unit tests for OnboardingService.cs that tests for:
    /// 1. Happy Path (No Exceptions)
    /// 2. When Exception occurs
    /// </summary>
    [TestClass]
    public class OnboardingServiceTest
    {
        private OnboardingService target;

        private Mock<OnboardingStepsFactory> stepsFactory;
        private Mock<ILogger> logger;

        //// step (AsyncStep) should be stubbed to produce custom behavior/scenarios
        private Mock<AsyncStep<EmployerOnboardingStepRequest, Response<Employer>>> step;

        [TestInitialize]
        public void Setup()
        {
            this.step = new Mock<AsyncStep<EmployerOnboardingStepRequest, Response<Employer>>>();
            this.logger = new Mock<ILogger>();
            this.logger.Setup(l => l.LogException(It.IsAny<Exception>())).Verifiable();

            this.stepsFactory = new Mock<OnboardingStepsFactory>();

            //// Setup to return a stubbed step (AsyncStep)
            this.stepsFactory.Setup(s => s.CreateEmployerOnboardingProcess()).Returns(this.step.Object);

            this.target = new OnboardingService(this.stepsFactory.Object, this.logger.Object);
        }

        [TestCleanup]
        public void Teardown()
        {
            this.step = null;
            this.logger = null;
            this.stepsFactory = null;

            this.target = null;
        }

        [TestMethod]
        public async Task OnboardEmployerAsync_ProcessingSucceeded_ExpectedResponseEqualToActual()
        {
            //// Arrange
            var id = Guid.NewGuid();
            var employer = new Employer(id, "Test Employer");
            var expected = new Response<Employer>(employer);
            this.step.Setup(s => s.ExecuteAsync(It.IsAny<EmployerOnboardingStepRequest>())).ReturnsAsync(expected);

            //// Act
            var actual = await this.target.OnboardEmployerAsync(new EmployerOnboardingRequest());

            //// Assert
            Assert.AreEqual(expected.Data.Id, actual.Data.Id);
        }
        
        [TestMethod]
        [ExpectedException(typeof(OnboardingServiceException))]
        public async Task OnboardEmployerAsync_ExceptionThrown_LoggerCalled()
        {
            //// Arrange
            var id = Guid.NewGuid();
        
            this.step.Setup(s => s.ExecuteAsync(It.IsAny<EmployerOnboardingStepRequest>())).Throws(new Exception());

            //// Act
            await this.target.OnboardEmployerAsync(new EmployerOnboardingRequest());

            //// Assert
            this.logger.Verify(l => l.LogException(It.IsAny<Exception>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(OnboardingServiceException))]
        public async Task OnboardEmployerAsync_ExceptionThrown_ExceptionRethrownAsOnboardingServiceException()
        {
            //// Arrange
            var id = Guid.NewGuid();

            this.step.Setup(s => s.ExecuteAsync(It.IsAny<EmployerOnboardingStepRequest>())).Throws(new Exception());

            //// Act
            await this.target.OnboardEmployerAsync(new EmployerOnboardingRequest());

            //// Assert
            Assert.ThrowsException<OnboardingServiceException>(() => this.target.OnboardEmployerAsync(new EmployerOnboardingRequest()));
        }
    }
}
