using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dfc.CourseDirectory.Core.Search;
using Dfc.CourseDirectory.Core.Search.Models;
using Dfc.CourseDirectory.Services.BulkUploadService;
using Dfc.CourseDirectory.Services.CourseService;
using Dfc.CourseDirectory.Services.Interfaces.BlobStorageService;
using Dfc.CourseDirectory.Services.Interfaces.BulkUploadService;
using Dfc.CourseDirectory.Services.Interfaces.CourseService;
using Dfc.CourseDirectory.Services.Interfaces.ProviderService;
using Dfc.CourseDirectory.Services.Interfaces.VenueService;
using Dfc.CourseDirectory.Web.BackgroundWorkers;
using Dfc.CourseDirectory.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Dfc.CourseDirectory.Web.Tests.Controllers
{
    /// <summary>
    /// Test controller+service together to check integration of the two.
    /// </summary>
    public class BulkUploadControllerIntegrationTests
    {
        private readonly IOptions<CourseServiceSettings> _settings = Options.Create(new CourseServiceSettings());

        [Fact]
        public async Task Index_Redirects()
        {
            IBulkUploadService bulkUploadService = new BulkUploadService(
                new Mock<IVenueService>().Object,
                _settings,
                new Mock<ICourseService>().Object,
                new Mock<ISearchClient<Lars>>().Object
            );
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            mockContextAccessor.Setup(m => m.HttpContext.Session).Returns(new FakeSession());
            var _controller = new BulkUploadController(
                NullLogger<BulkUploadController>.Instance,
                mockContextAccessor.Object,
                bulkUploadService,
                new Mock<IBlobStorageService>().Object,
                new Mock<ICourseService>().Object,
                new Mock<IWebHostEnvironment>().Object,
                new Mock<IProviderService>().Object,
                new Mock<IBackgroundTaskQueue>().Object
            );

            const string csv = "STANDARD_CODE,STANDARD_VERSION,FRAMEWORK_CODE,FRAMEWORK_PROG_TYPE,FRAMEWORK_PATHWAY_CODE,APPRENTICESHIP_INFORMATION,APPRENTICESHIP_WEBPAGE,CONTACT_EMAIL,CONTACT_PHONE,CONTACT_URL,DELIVERY_METHOD,VENUE,RADIUS,DELIVERY_MODE,ACROSS_ENGLAND, NATIONAL_DELIVERY,REGION,SUB_REGION\r\n" +
                "105,1,,,,\"This apprenticeship is applicable to any industry and perfect for those already in a team leading role or entering a management role for the first time.It lasts around 15 months and incorporates 14 one day modules plus on - the job learning and mentoring.It involves managing projects, leading and managing teams, change, financial management and coaching.If you choose to do this qualification with Azesta, you can expect exciting and engaging day long modules fully utilising experiential learning methods(one per month), high quality tutorial support, access to e-mail and telephone support whenever you need it and great results in your end point assessments.\",http://www.azesta.co.uk/apprenticeships/,info@azesta.co.uk,1423711904,http://www.azesta.co.uk/contact-us/,Both,Fenestra Centre Scunthorpe,100,Employer,No,,,\r\n" +
                "104,1,,,,\"This apprenticeship is great for current managers. It involves managing projects, leading and managing teams, change, financial and resource management, talent management and coaching and mentoring. It takes around 2.5 years to complete and if you choose to do this qualification with Azesta, you can expect exciting and engaging day long modules fully utilising experiential learning methods (one per month), high quality tutorial support, access to e-mail and telephone support whenever you need it and great results in your end point assessments.\",http://www.azesta.co.uk/apprenticeships/,info@azesta.co.uk,1423711904,http://www.azesta.co.uk,Both,Fenestra Centre Scunthorpe,100,Employer,Yes,,,\r\n";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            var bulkUploadFile = new FormFile(stream, 0, stream.Length, "bulkUploadFile", "Test.csv")
            {
                Headers = new HeaderDictionary
                {
                    { "Content-Disposition", "filename" }
                }
            };

            var result = await _controller.Upload(bulkUploadFile);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("PublishYourFile", redirectResult.ActionName);
            Assert.Equal("BulkUploadApprenticeships", redirectResult.ControllerName);
        }
    }
}
