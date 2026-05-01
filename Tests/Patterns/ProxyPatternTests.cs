using Moq;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Proxies;
using OnlineJobs.Domain.Entities;
using Xunit;

namespace OnlineJobs.Tests.Patterns
{

    public class ProxyPatternTests
    {
        [Fact]
        public async Task ProtectionProxy_ShouldHideCompanyForUnauthenticated()
        {
            var mockJobService = new Mock<IJobService>();
            var job = new JobPosting
            {
                Id = Guid.NewGuid(),
                Title = "Test Job",
                Description = "Test Description",
                IsCompanyRevealed = true
            };
            job.Company = new Company { Name = "Secret Company" };

            mockJobService.Setup(s => s.GetJobByIdAsync(It.IsAny<Guid>())).ReturnsAsync(job);

            var realAccess = new RealJobPostingAccess(mockJobService.Object);
            var proxy = new JobPostingProtectionProxy(realAccess, isAuthenticated: false);

            // Act
            var companyName = proxy.GetCompanyName(job);

            // Assert
            Assert.Contains("Hidden", companyName);
        }

        [Fact]
        public async Task ProtectionProxy_ShouldRevealCompanyForAuthenticated()
        {
            // Arrange
            var mockJobService = new Mock<IJobService>();
            var job = new JobPosting
            {
                Id = Guid.NewGuid(),
                Title = "Test Job",
                Description = "Test Description",
                IsCompanyRevealed = true
            };
            job.Company = new Company { Name = "Public Company" };

            mockJobService.Setup(s => s.GetJobByIdAsync(It.IsAny<Guid>())).ReturnsAsync(job);

            var realAccess = new RealJobPostingAccess(mockJobService.Object);
            var proxy = new JobPostingProtectionProxy(realAccess, isAuthenticated: true, Guid.NewGuid());

            // Act
            var companyName = proxy.GetCompanyName(job);

            // Assert
            Assert.Equal("Public Company", companyName);
        }

        [Fact]
        public async Task ProtectionProxy_ShouldHideSalaryForUnauthenticated()
        {
            // Arrange
            var mockJobService = new Mock<IJobService>();
            var job = new JobPosting
            {
                Id = Guid.NewGuid(),
                Title = "Test Job",
                Description = "Test Description",
                SalaryMax = 150000
            };

            mockJobService.Setup(s => s.GetJobByIdAsync(It.IsAny<Guid>())).ReturnsAsync(job);

            var realAccess = new RealJobPostingAccess(mockJobService.Object);
            var proxy = new JobPostingProtectionProxy(realAccess, isAuthenticated: false);

            // Act
            var salary = proxy.GetSalaryRange(job);

            // Assert
            Assert.Null(salary);
        }

        [Fact]
        public async Task VirtualProxy_ShouldNotLoadUntilAccessed()
        {
            // Arrange
            var mockAppService = new Mock<IApplicationService>();
            var applications = new List<JobApplication>
            {
                new JobApplication(Guid.NewGuid(), Guid.NewGuid(), "Cover letter 1"),
                new JobApplication(Guid.NewGuid(), Guid.NewGuid(), "Cover letter 2")
            };

            mockAppService.Setup(s => s.GetApplicationsByJobPostingAsync(It.IsAny<Guid>()))
                .ReturnsAsync(applications);

            var realAccess = new RealApplicationListAccess(mockAppService.Object, Guid.NewGuid());
            var virtualProxy = new ApplicationListVirtualProxy(realAccess);

            // Assert - Should not be loaded yet
            Assert.False(virtualProxy.IsLoaded());

            // Act - First access triggers loading
            var result = await virtualProxy.GetApplicationsAsync();

            // Assert
            Assert.True(virtualProxy.IsLoaded());
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task VirtualProxy_ShouldUseCacheOnSecondAccess()
        {
            // Arrange
            var mockAppService = new Mock<IApplicationService>();
            var applications = new List<JobApplication>
            {
                new JobApplication(Guid.NewGuid(), Guid.NewGuid(), "Cover letter")
            };

            mockAppService.Setup(s => s.GetApplicationsByJobPostingAsync(It.IsAny<Guid>()))
                .ReturnsAsync(applications);

            var realAccess = new RealApplicationListAccess(mockAppService.Object, Guid.NewGuid());
            var virtualProxy = new ApplicationListVirtualProxy(realAccess);

            // Act
            await virtualProxy.GetApplicationsAsync(); // First call - loads data
            await virtualProxy.GetApplicationsAsync(); // Second call - should use cache

            // Assert - Service should be called only once
            mockAppService.Verify(s => s.GetApplicationsByJobPostingAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task VirtualProxy_ShouldReloadAfterInvalidation()
        {
            // Arrange
            var mockAppService = new Mock<IApplicationService>();
            var applications = new List<JobApplication>
            {
                new JobApplication(Guid.NewGuid(), Guid.NewGuid(), "Cover letter")
            };

            mockAppService.Setup(s => s.GetApplicationsByJobPostingAsync(It.IsAny<Guid>()))
                .ReturnsAsync(applications);

            var realAccess = new RealApplicationListAccess(mockAppService.Object, Guid.NewGuid());
            var virtualProxy = new ApplicationListVirtualProxy(realAccess);

            // Act
            await virtualProxy.GetApplicationsAsync(); // First load
            virtualProxy.Invalidate(); // Clear cache
            await virtualProxy.GetApplicationsAsync(); // Second load

            // Assert - Service should be called twice
            mockAppService.Verify(s => s.GetApplicationsByJobPostingAsync(It.IsAny<Guid>()), Times.Exactly(2));
        }
    }
}
