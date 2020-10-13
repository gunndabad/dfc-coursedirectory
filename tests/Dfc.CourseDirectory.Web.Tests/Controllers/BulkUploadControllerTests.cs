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
            var x = await _controller.Upload(null);
        }
    }
}
