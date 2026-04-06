using Microsoft.AspNetCore.Mvc;
using StudentReportService.Services;

namespace StudentReportService.Controllers;

[ApiController]
[Route("api/v1/students")]
public sealed class StudentsController : ControllerBase
{
    private readonly INodeApiClient _nodeApiClient;
    private readonly IPdfReportService _pdfService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(
        INodeApiClient nodeApiClient,
        IPdfReportService pdfService,
        ILogger<StudentsController> logger)
    {
        _nodeApiClient = nodeApiClient;
        _pdfService    = pdfService;
        _logger        = logger;
    }

    /// <summary>
    /// Fetches student data from the Node.js backend and returns a downloadable
    /// PDF report.
    /// </summary>
    /// <param name="id">Student ID (must exist in the Node.js backend).</param>
    [HttpGet("{id:int}/report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetStudentReport(int id, CancellationToken ct)
    {
        try
        {
            var student  = await _nodeApiClient.GetStudentAsync(id, ct);
            var pdfBytes = _pdfService.GenerateStudentReport(student);
            var filename = $"student-report-{id}.pdf";

            return File(pdfBytes, "application/pdf", filename);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Student with ID {id} not found." });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to communicate with the Node.js API.");
            return StatusCode(StatusCodes.Status502BadGateway,
                new { error = "Failed to fetch student data from the upstream API." });
        }
    }
}
