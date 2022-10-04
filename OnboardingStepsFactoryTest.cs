namespace Yousource.UnitTesting.Exercise.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Yousource.UnitTesting.Exercise.Interfaces;
    using Yousource.UnitTesting.Exercise.Steps.Employers;

    /// <summary>
    /// Instructions: Write unit tests for OnboardingStepsFactory.cs that tests for:
    /// 1. The correct order of steps (step 1 & 2).
    /// 2. The first step is the correct step (SignUpEmployerStep).
    /// </summary>
    [TestClass]
    public class OnboardingStepsFactoryTest
    {
        private OnboardingStepsFactory target;
       
        private Mock<IIdentityService> identityService;
        private Mock<IEmployerService> employerService;
       

        [TestInitialize]
        public void Setup()
        {
            this.employerService = new Mock<IEmployerService>();
            this.identityService = new Mock<IIdentityService>();

            this.target = new OnboardingStepsFactory(this.identityService.Object, this.employerService.Object);
        }

        [TestCleanup]
        public void Teardown()
        {
            this.employerService = null;
            this.identityService = null;

            this.target = null;
        }

        [TestMethod]
        public void CreateEmployerOnboardingProcess_AnyScenario_FirstStepIsSignUpEmployer()
        {
            //// Act
            var expectedType = typeof(SignUpEmployerStep);

            //// Arrange
            var result = this.target.CreateEmployerOnboardingProcess();
            
            //// Assert
            Assert.AreEqual(expectedType, result.GetType());
        }

        [TestMethod]
        public void CreateEmployerOnboardingProcess_AnyScenario_SecondStepIsCreateEmployer()
        {
            //// Act
            var expectedType = typeof(CreateEmployerStep);

            //// Arrange
            var result = this.target.CreateEmployerOnboardingProcess();

            //// Assert
            Assert.AreEqual(expectedType, result.Next.GetType());
        }
    }
}
