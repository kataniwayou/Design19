using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.IntegrationBase.Messaging;

namespace FlowOrchestrator.ExecutionBase.Models;

/// <summary>
/// Represents an export command.
/// </summary>
public class ExportCommand : CommandBase
{
    /// <summary>
    /// Gets the exporter identifier.
    /// </summary>
    public string ExporterId { get; }

    /// <summary>
    /// Gets the export parameters.
    /// </summary>
    public ExportParameters Parameters { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportCommand"/> class.
    /// </summary>
    /// <param name="exporterId">The exporter identifier.</param>
    /// <param name="parameters">The export parameters.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    public ExportCommand(
        string exporterId,
        ExportParameters parameters,
        string correlationId)
        : base(correlationId)
    {
        ExporterId = exporterId;
        Parameters = parameters;
    }
}
