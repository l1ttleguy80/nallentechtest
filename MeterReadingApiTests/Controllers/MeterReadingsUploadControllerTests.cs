using System.Net;
using FluentAssertions;
using MediatR;
using MeterReadingApi.Controllers;
using MeterReadingApi.CQRS;
using MeterReadingApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace MeterReadingApiTests.Controllers;

public class MeterReadingsUploadControllerTests
{
    private readonly Mock<IMediator> resolver;
    private readonly Mock<ILogger<MeterReadingsUploadController>> logger;
    private readonly MeterReadingsUploadController controller;

    public MeterReadingsUploadControllerTests()
    {
        this.resolver = new Mock<IMediator>();
        this.logger = new Mock<ILogger<MeterReadingsUploadController>>();
        this.controller = new MeterReadingsUploadController(this.resolver.Object, this.logger.Object);
    }

    [Fact]
    public async Task ShoudReturnSuccessForValidRequest()
    {
        // Arrange
        var file = new Mock<IFormFile>();
        this.resolver.Setup(x => x.Send(It.IsAny<MeterReadingsUploadRequestHandler.Context>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MeterReadingsUpload());

        // Act
        var result = await this.controller.Post(file.Object, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.Should().BeOfType<MeterReadingsUpload>();
        this.resolver.Verify(x => x.Send(It.IsAny<MeterReadingsUploadRequestHandler.Context>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShoudReturnBadRequestWhenResolverThrowsArgumentException()
    {
        // Arrange
        var file = new Mock<IFormFile>();
        this.resolver.Setup(x => x.Send(It.IsAny<MeterReadingsUploadRequestHandler.Context>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Error"));

        // Act
        var result = await this.controller.Post(file.Object, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>();
        result.As<ObjectResult>().Value.As<ProblemDetails>().Status.Should().Be((int)HttpStatusCode.BadRequest);
        result.As<ObjectResult>().Value.As<ProblemDetails>().Detail.Should().Be("Invalid file format");
        this.resolver.Verify(x => x.Send(It.IsAny<MeterReadingsUploadRequestHandler.Context>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShoudReturnInternalServerErrorAndLogWhenResolverThrowsException()
    {
        // Arrange
        var file = new Mock<IFormFile>();
        this.resolver.Setup(x => x.Send(It.IsAny<MeterReadingsUploadRequestHandler.Context>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error"));

        // Act
        var result = await this.controller.Post(file.Object, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>();
        result.As<ObjectResult>().Value.As<ProblemDetails>().Status.Should().Be((int)HttpStatusCode.InternalServerError);
        this.resolver.Verify(x => x.Send(It.IsAny<MeterReadingsUploadRequestHandler.Context>(), It.IsAny<CancellationToken>()), Times.Once);
        this.logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<object>(),
                It.IsAny<Exception>(),
                (Func<object, Exception?, string>)It.IsAny<object>()),
             Times.Once);
    }
}