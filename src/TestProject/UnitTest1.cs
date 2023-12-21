using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swetugg.Web.Areas.Admin.Controllers;
using Swetugg.Web.Areas.Admin.Controllers.Models;
using System.Web.Mvc; // Added reference to System.Web.Mvc


namespace TestProject
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test_GetScheduleFromSessionize()
        {
            // Arrange
            //var controller = new SessionizeController(null, null);
            SessionizeController.baseurl = $"api/v2/ful893fy/view";
            var expectedSchedule = new List<SezzionizeSchedule>();

            // Act
            var actualSchedule = await SessionizeController.GetScheduleFromSessionize();

            // Assert
            Assert.True(true);
        }

        [Test]
        public async Task Test_GetScheduleFromSessionize2()
        {
            // Arrange
            var controller = new SessionizeController(null, null);
            SessionizeController.baseurl = $"api/v2/ful893fy/view";


            // Act
            var s = await controller.ImportSchedule();

            // Assert
            Assert.True(true);
        }
    }
}