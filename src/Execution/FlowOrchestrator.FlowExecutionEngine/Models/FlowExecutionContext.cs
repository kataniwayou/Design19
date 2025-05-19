using FlowOrchestrator.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowOrchestrator.FlowExecutionEngine.Models
{
    /// <summary>
    /// Represents the context of a flow execution.
    /// </summary>
    public class FlowExecutionContext : AbstractEntity
    {
        /// <summary>
        /// Gets or sets the execution identifier.
        /// </summary>
        public string ExecutionId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the flow identifier.
        /// </summary>
        public string FlowId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the source assignment identifier.
        /// </summary>
        public string SourceAssignmentId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the destination assignment identifiers.
        /// </summary>
        public List<string> DestinationAssignmentIds { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        public string CorrelationId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        public new Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the serialized data.
        /// </summary>
        public Dictionary<string, string> SerializedData { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the current step identifier.
        /// </summary>
        public string CurrentStepId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current step type.
        /// </summary>
        public string CurrentStepType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current branch identifier.
        /// </summary>
        public string CurrentBranchId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the execution has completed.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the execution has failed.
        /// </summary>
        public bool IsFailed { get; set; }

        /// <summary>
        /// Gets or sets the created at timestamp.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the updated at timestamp.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Sets data in the context.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetData<T>(string key, T value)
        {
            Data[key] = value!;
            SerializedData[key] = JsonSerializer.Serialize(value);
        }

        /// <summary>
        /// Gets data from the context.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The data.</returns>
        public T GetData<T>(string key)
        {
            if (Data.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }

            if (SerializedData.TryGetValue(key, out var serializedValue))
            {
                var deserializedValue = JsonSerializer.Deserialize<T>(serializedValue);
                if (deserializedValue != null)
                {
                    Data[key] = deserializedValue;
                    return deserializedValue;
                }
            }

            throw new KeyNotFoundException($"Data with key '{key}' not found or not of type {typeof(T).Name}");
        }

        /// <summary>
        /// Tries to get data from the context.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>True if the data was found, false otherwise.</returns>
        public bool TryGetData<T>(string key, out T value)
        {
            value = default!;

            if (Data.TryGetValue(key, out var objValue) && objValue is T typedValue)
            {
                value = typedValue;
                return true;
            }

            if (SerializedData.TryGetValue(key, out var serializedValue))
            {
                var deserializedValue = JsonSerializer.Deserialize<T>(serializedValue);
                if (deserializedValue != null)
                {
                    Data[key] = deserializedValue;
                    value = deserializedValue;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes data from the context.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if the data was removed, false otherwise.</returns>
        public bool RemoveData(string key)
        {
            var dataRemoved = Data.Remove(key);
            var serializedDataRemoved = SerializedData.Remove(key);
            return dataRemoved || serializedDataRemoved;
        }

        /// <summary>
        /// Clears all data from the context.
        /// </summary>
        public void ClearData()
        {
            Data.Clear();
            SerializedData.Clear();
        }

        /// <summary>
        /// Sets metadata in the context.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetMetadata(string key, string value)
        {
            Metadata[key] = value;
        }

        /// <summary>
        /// Gets metadata from the context.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The metadata.</returns>
        public string GetMetadata(string key)
        {
            if (Metadata.TryGetValue(key, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException($"Metadata with key '{key}' not found");
        }

        /// <summary>
        /// Tries to get metadata from the context.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>True if the metadata was found, false otherwise.</returns>
        public bool TryGetMetadata(string key, out string value)
        {
            return Metadata.TryGetValue(key, out value!);
        }

        /// <summary>
        /// Gets all metadata from the context.
        /// </summary>
        /// <returns>The metadata.</returns>
        public Dictionary<string, string> GetAllMetadata()
        {
            return new Dictionary<string, string>(Metadata);
        }

        /// <summary>
        /// Removes metadata from the context.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if the metadata was removed, false otherwise.</returns>
        public bool RemoveMetadata(string key)
        {
            return Metadata.Remove(key);
        }

        /// <summary>
        /// Clears all metadata from the context.
        /// </summary>
        public void ClearMetadata()
        {
            Metadata.Clear();
        }
    }
}
