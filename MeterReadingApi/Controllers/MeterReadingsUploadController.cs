using System.Net;
using MediatR;
using MeterReadingApi.CQRS;
using MeterReadingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeterReadingApi.Controllers
{
    [ApiController]
    [Route("meter-readings-upload")]
    public class MeterReadingsUploadController : Controller
    {
        private readonly IMediator resolver;
        private readonly ILogger<MeterReadingsUploadController> logger;

        public MeterReadingsUploadController(IMediator resolver, ILogger<MeterReadingsUploadController> logger)
        {
            this.resolver = resolver;
            this.logger = logger;
        }

        [HttpPost(Name = "PostMeterReadingsUpload")]
        [ProducesResponseType(200, Type = typeof(MeterReadingsUpload))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            MeterReadingsUpload response;

            try
            {
                response = await this.resolver.Send(new MeterReadingsUploadRequestHandler.Context(file), cancellationToken);
            }
            catch (ArgumentException)
            {
                return this.Problem("Invalid file format", statusCode: (int)HttpStatusCode.BadRequest);
            }
            catch(Exception ex)
            {
                this.logger.LogError("Error uploading meter readings", ex);
                return this.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
            }

            return this.Ok(response);
        }
    }
}