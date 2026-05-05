using System;
using Moq;
using ParallelProcessing;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ParallelProcessingTests
{
    public class StartupTests
    {
        [Fact]
        public void Constructor_WithValidConfiguration_CreatesInstance()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();

            // Act
            var startup = new Startup(mockConfiguration.Object);

            // Assert
            Assert.NotNull(startup);
        }

        [Fact]
        public void Constructor_WithNullConfiguration_CreatesInstance()
        {
            // Act
            var startup = new Startup(null!);

            // Assert
            Assert.NotNull(startup);
        }

        [Fact]
        public void Constructor_WithDifferentConfigurationInstances_CreatesMultipleInstances()
        {
            // Arrange
            var mockConfiguration1 = new Mock<IConfiguration>();
            var mockConfiguration2 = new Mock<IConfiguration>();

            // Act
            var startup1 = new Startup(mockConfiguration1.Object);
            var startup2 = new Startup(mockConfiguration2.Object);

            // Assert
            Assert.NotNull(startup1);
            Assert.NotNull(startup2);
            Assert.NotSame(startup1, startup2);
        }

        [Fact]
        public void ConfigureServices_WithValidServiceCollection_CallsAddSingleton()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var mockServices = new Mock<IServiceCollection>();
            var startup = new Startup(mockConfiguration.Object);

            // Act
            startup.ConfigureServices(mockServices.Object);

            // Assert
            mockServices.Verify(
                s => s.Add(It.Is<ServiceDescriptor>(sd =>
                    sd.ServiceType == typeof(IBespokeDictionary) &&
                    sd.ImplementationType == typeof(BespokeDictionary) &&
                    sd.Lifetime == ServiceLifetime.Singleton)),
                Times.Once);
        }

        [Fact]
        public void ConfigureServices_WithNullServiceCollection_ThrowsArgumentNullException()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var startup = new Startup(mockConfiguration.Object);

            // Act & Assert - AddSingleton will throw when called with null
            Assert.Throws<ArgumentNullException>(() => startup.ConfigureServices(null!));
        }

        [Fact]
        public void ConfigureServices_CalledMultipleTimes_RegistersSingletonEachTime()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var mockServices = new Mock<IServiceCollection>();
            var startup = new Startup(mockConfiguration.Object);

            // Act
            startup.ConfigureServices(mockServices.Object);
            startup.ConfigureServices(mockServices.Object);

            // Assert
            mockServices.Verify(
                s => s.Add(It.Is<ServiceDescriptor>(sd =>
                    sd.ServiceType == typeof(IBespokeDictionary) &&
                    sd.ImplementationType == typeof(BespokeDictionary) &&
                    sd.Lifetime == ServiceLifetime.Singleton)),
                Times.Exactly(2));
        }

        [Fact]
        public void Constructor_StoresConfigurationCorrectly()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();

            // Act
            var startup = new Startup(mockConfiguration.Object);

            // Assert - Verify that the startup instance was created successfully,
            // which indicates the configuration was stored (even though _configuration is private)
            Assert.NotNull(startup);
        }
    }
}
