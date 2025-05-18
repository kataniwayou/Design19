using FlowOrchestrator.Abstractions.Messages;
using FlowOrchestrator.IntegrationBase.Messaging;

namespace FlowOrchestrator.ExecutionBase.Models;

/// <summary>
/// Represents an import command.
/// </summary>
public class ImportCommand : CommandBase
{
    /// <summary>
    /// Gets the importer identifier.
    /// </summary>
    public string ImporterId { get; }

    /// <summary>
    /// Gets the import parameters.
    /// </summary>
    public ImportParameters Parameters { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportCommand"/> class.
    /// </summary>
    /// <param name="importerId">The importer identifier.</param>
    /// <param name="parameters">The import parameters.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    public ImportCommand(
        string importerId,
        ImportParameters parameters,
        string correlationId)
        : base(correlationId)
    {
        ImporterId = importerId;
        Parameters = parameters;
    }
}
