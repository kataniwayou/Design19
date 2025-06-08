using Manager.Orchestrator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Manager.Orchestrator.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Orchestration management operations")]
public class OrchestrationController : ControllerBase
{
    private readonly IOrchestrationService _orchestrationService;
    private readonly ILogger<OrchestrationController> _logger;

    public OrchestrationController(
        IOrchestrationService orchestrationService,
        ILogger<OrchestrationController> logger)
    {
        _orchestrationService = orchestrationService;
        _logger = logger;
    }

    /// <summary>
    /// Starts orchestration for the specified orchestrated flow ID
    /// </summary>
    /// <param name="orchestratedFlowId">The orchestrated flow ID to start</param>
    /// <returns>Success response</returns>
    [HttpPost("start/{orchestratedFlowId}")]
    [SwaggerOperation(Summary = "Start orchestration", Description = "Starts orchestration by gathering all required data from managers and storing in cache")]
    [SwaggerResponse(200, "Orchestration started successfully")]
    [SwaggerResponse(400, "Invalid orchestrated flow ID")]
    [SwaggerResponse(404, "Orchestrated flow not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult> Start(string orchestratedFlowId)
    {
        var userContext = User.Identity?.Name ?? "Anonymous";

        // Validate GUID format
        if (!Guid.TryParse(orchestratedFlowId, out Guid guidOrchestratedFlowId))
        {
            _logger.LogWarning("Invalid GUID format provided for Start orchestration. OrchestratedFlowId: {OrchestratedFlowId}, User: {User}, RequestId: {RequestId}",
                orchestratedFlowId, userContext, HttpContext.TraceIdentifier);
            return BadRequest($"Invalid GUID format: {orchestratedFlowId}");
        }

        _logger.LogInformation("Starting Start orchestration request. OrchestratedFlowId: {OrchestratedFlowId}, User: {User}, RequestId: {RequestId}",
            guidOrchestratedFlowId, userContext, HttpContext.TraceIdentifier);

        try
        {
            await _orchestrationService.StartOrchestrationAsync(guidOrchestratedFlowId);

            _logger.LogInformation("Successfully started orchestration. OrchestratedFlowId: {OrchestratedFlowId}, User: {User}, RequestId: {RequestId}",
                guidOrchestratedFlowId, userContext, HttpContext.TraceIdentifier);

            return Ok(new { 
                message = "Orchestration started successfully", 
                orchestratedFlowId = guidOrchestratedFlowId,
                startedAt = DateTime.UtcNow
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation during Start orchestration. OrchestratedFlowId: {OrchestratedFlowId}, User: {User}, RequestId: {RequestId}",
                guidOrchestratedFlowId, userContext, HttpContext.TraceIdentifier);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Start orchestration. OrchestratedFlowId: {OrchestratedFlowId}, User: {User}, RequestId: {RequestId}",
                guidOrchestratedFlowId, userContext, HttpContext.TraceIdentifier);
            return StatusCode(500, "An error occurred while starting orchestration");
        }
    }

    /// <summary>
    /// Stops orchestration for the specified orchestrated flow ID
    /// </summary>
    /// <param name="orchestratedFlowId">The orchestrated flow ID to stop</param>
    /// <returns>Success response</returns>
    [HttpPost("stop/{orchestratedFlowId}")]
    [SwaggerOperation(Summary = "Stop orchestration", Description = "Stops orchestration by removing data from cache and performing cleanup")]
    [SwaggerResponse(200, "Orchestration stopped successfully")]
    [SwaggerResponse(400, "Invalid orchestrated flow ID")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult> Stop(string orchestratedFlowId)
    {
        var userContext = User.Identity?.Name ?? "Anonymous";

        // Validate GUID format
        if (!Guid.TryParse(orchestratedFlowId, out Guid guidOrchestratedFlowId))
        {
            _logger.LogWarning("Invalid GUID format provided for Stop orchestration. OrchestratedFlowId: {OrchestratedFlowId}, User: {User}, RequestId: {RequestId}",
                orchestratedFlowId, userContext, HttpContext.TraceIdentifier);
            return BadRequest($"Invalid GUID format: {orchestratedFlowId}");
        }

        _logger.LogInformation("Starting Stop orchestration request. OrchestratedFlowId: {OrchestratedFlowId}, User: {User}, RequestId: {RequestId}",
            guidOrchestratedFlowId, userContext, HttpContext.TraceIdentifier);

        try
        {
            await _orchestrationService.StopOrchestrationAsync(guidOrchestratedFlowId);

            _logger.LogInformation("Successfully stopped orchestration. OrchestratedFlowId: {OrchestratedFlowId}, User: {User}, RequestId: {RequestId}",
                guidOrchestratedFlowId, userContext, HttpContext.TraceIdentifier);

            return Ok(new { 
                message = "Orchestration stopped successfully", 
                orchestratedFlowId = guidOrchestratedFlowId,
                stoppedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Stop orchestration. OrchestratedFlowId: {OrchestratedFlowId}, User: {User}, RequestId: {RequestId}",
                guidOrchestratedFlowId, userContext, HttpContext.TraceIdentifier);
            return StatusCode(500, "An error occurred while stopping orchestration");
        }
    }

    /// <summary>
    /// Gets orchestration status for the specified orchestrated flow ID
    /// </summary>
    /// <param name="orchestratedFlowId">The orchestrated flow ID to check</param>
    /// <returns>Orchestration status information</returns>
    [HttpGet("status/{orchestratedFlowId}")]
    [SwaggerOperation(Summary = "Get orchestration status", Description = "Gets the current status of orchestration including active state and metadata")]
    [SwaggerResponse(200, "Orchestration status retrieved successfully")]
    [SwaggerResponse(400, "Invalid orchestrated flow ID")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult<OrchestrationStatusModel>> GetStatus(string orchestratedFlowId)
    {
        var userContext = User.Identity?.Name ?? "Anonymous";

        // Validate GUID format
        if (!Guid.TryParse(orchestratedFlowId, out Guid guidOrchestratedFlowId))
        {
            _logger.LogWarning("Invalid GUID format provided for Get orchestration status. OrchestratedFlowId: {OrchestratedFlowId}, User: {User}, RequestId: {RequestId}",
                orchestratedFlowId, userContext, HttpContext.TraceIdentifier);
            return BadRequest($"Invalid GUID format: {orchestratedFlowId}");
        }

        _logger.LogInformation("Starting Get orchestration status request. OrchestratedFlowId: {OrchestratedFlowId}, User: {User}, RequestId: {RequestId}",
            guidOrchestratedFlowId, userContext, HttpContext.TraceIdentifier);

        try
        {
            var status = await _orchestrationService.GetOrchestrationStatusAsync(guidOrchestratedFlowId);

            _logger.LogInformation("Successfully retrieved orchestration status. OrchestratedFlowId: {OrchestratedFlowId}, IsActive: {IsActive}, StepCount: {StepCount}, AssignmentCount: {AssignmentCount}, User: {User}, RequestId: {RequestId}",
                guidOrchestratedFlowId, status.IsActive, status.StepCount, status.AssignmentCount, userContext, HttpContext.TraceIdentifier);

            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Get orchestration status. OrchestratedFlowId: {OrchestratedFlowId}, User: {User}, RequestId: {RequestId}",
                guidOrchestratedFlowId, userContext, HttpContext.TraceIdentifier);
            return StatusCode(500, "An error occurred while retrieving orchestration status");
        }
    }
}
