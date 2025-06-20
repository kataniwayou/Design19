using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Manager.Workflow.Services;

/// <summary>
/// Service for validating referential integrity with OrchestratedFlow entities
/// </summary>
public class OrchestratedFlowValidationService : IOrchestratedFlowValidationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrchestratedFlowValidationService> _logger;

    public OrchestratedFlowValidationService(
        HttpClient httpClient,
        ILogger<OrchestratedFlowValidationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> CheckWorkflowReferencesAsync(Guid workflowId)
    {
        _logger.LogInformation("Starting workflow reference validation. WorkflowId: {WorkflowId}", workflowId);

        try
        {
            var response = await _httpClient.GetAsync($"api/orchestratedflow/workflow/{workflowId}/exists");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var hasReferences = bool.Parse(content);

                _logger.LogInformation("Successfully validated workflow references. WorkflowId: {WorkflowId}, HasReferences: {HasReferences}",
                    workflowId, hasReferences);

                return hasReferences;
            }

            // Fail-safe: if we can't validate, assume there are references
            _logger.LogWarning("Failed to validate workflow references - service returned error. WorkflowId: {WorkflowId}, StatusCode: {StatusCode}",
                workflowId, response.StatusCode);
            return true;
        }
        catch (HttpRequestException ex)
        {
            // Fail-safe: if service is unavailable, assume there are references
            _logger.LogError(ex, "HTTP error validating workflow references - service may be unavailable. WorkflowId: {WorkflowId}",
                workflowId);
            return true;
        }
        catch (TaskCanceledException ex)
        {
            // Fail-safe: if request times out, assume there are references
            _logger.LogError(ex, "Timeout validating workflow references. WorkflowId: {WorkflowId}", workflowId);
            return true;
        }
        catch (Exception ex)
        {
            // Fail-safe: if any other error occurs, assume there are references
            _logger.LogError(ex, "Unexpected error validating workflow references. WorkflowId: {WorkflowId}", workflowId);
            return true;
        }
    }
}
