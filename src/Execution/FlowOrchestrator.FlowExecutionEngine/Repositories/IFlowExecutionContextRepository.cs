using FlowOrchestrator.FlowExecutionEngine.Models;
using System.Threading.Tasks;

namespace FlowOrchestrator.FlowExecutionEngine.Repositories
{
    /// <summary>
    /// Interface for the flow execution context repository.
    /// </summary>
    public interface IFlowExecutionContextRepository
    {
        /// <summary>
        /// Gets a flow execution context by execution identifier.
        /// </summary>
        /// <param name="executionId">The execution identifier.</param>
        /// <returns>The flow execution context.</returns>
        Task<FlowExecutionContext?> GetByExecutionIdAsync(string executionId);

        /// <summary>
        /// Gets a flow execution context by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The flow execution context.</returns>
        Task<FlowExecutionContext?> GetByIdAsync(string id);

        /// <summary>
        /// Creates a flow execution context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created flow execution context.</returns>
        Task<FlowExecutionContext> CreateAsync(FlowExecutionContext context);

        /// <summary>
        /// Updates a flow execution context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The updated flow execution context.</returns>
        Task<FlowExecutionContext> UpdateAsync(FlowExecutionContext context);

        /// <summary>
        /// Deletes a flow execution context.
        /// </summary>
        /// <param name="executionId">The execution identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(string executionId);
    }
}
