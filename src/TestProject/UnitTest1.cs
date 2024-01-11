using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swetugg.Web.Areas.Admin.Controllers;
using Swetugg.Web.Areas.Admin.Controllers.Models;
using System.Web.Mvc;
using Swetugg.Web.Models; // Added reference to System.Web.Mvc


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
            //Conf
            var confId = 10; //8 2023, 10=2024
            var code = "a18ospqz"; //2024 a18ospqz ; ful893fy 2023
            // Arrange
            var context = new ApplicationDbContext(); // "Server=tcp:swetuggdevserver.database.windows.net,1433;Initial Catalog=swetuggdev;Persist Security Info=False;User ID=swetugg-dev;Password=aBVX*8EAz_zbbaciDXdFDPACFJFJbmk7!_T_uMwo-!pc@3ZBJBTJJ;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            var controller = new SessionizeController(null, context);
            SessionizeController.baseurl = $"api/v2/{code}/view";
            controller.ConferenceId = confId;

            // Act
            var s = await controller.ImportSchedule();

            // Assert
            Assert.True(true);
        }
    }
}