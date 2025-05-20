# FlowOrchestrator Observability Components

This directory contains the observability components for the FlowOrchestrator system. These components provide monitoring, statistics, alerting, and analytics capabilities.

## Components

### StatisticsService

The StatisticsService is responsible for collecting, processing, and storing statistics from all system components. It provides:

- Collection of counter, gauge, and histogram metrics
- Storage of statistics in MongoDB
- Integration with MassTransit for message-based communication
- OpenTelemetry integration for distributed tracing

### MonitoringFramework

The MonitoringFramework provides APIs for monitoring system health and performance. It includes:

- Health check endpoints
- Resource utilization monitoring
- REST API for monitoring data
- Integration with MassTransit for message-based communication
- OpenTelemetry integration for distributed tracing

### AlertingSystem

The AlertingSystem handles alert generation, notification, and management. It provides:

- Alert rule engine
- Alert notification
- Alert history and management
- Integration with MassTransit for message-based communication
- OpenTelemetry integration for distributed tracing

### AnalyticsEngine

The AnalyticsEngine provides advanced analytics and reporting capabilities. It includes:

- Analytics data collection and processing
- Report generation
- Scheduled reports
- Integration with MassTransit for message-based communication
- OpenTelemetry integration for distributed tracing

## Architecture

All observability components follow the same architectural pattern:

1. They inherit from `AbstractObservabilityComponent` in the `FlowOrchestrator.ObservabilityBase` library
2. They use MongoDB for data storage
3. They use MassTransit for message-based communication
4. They use OpenTelemetry for distributed tracing
5. They use the FlowOrchestrator logging framework

## Configuration

Each component requires the following configuration:

- MongoDB connection string
- RabbitMQ connection string
- OpenTelemetry endpoint

Example configuration:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017",
    "RabbitMQ": "rabbitmq://guest:guest@localhost/"
  },
  "OpenTelemetry": {
    "Endpoint": "http://localhost:4317"
  }
}
```

## Usage

### StatisticsService

The StatisticsService collects statistics from all system components. Components can send statistics via MassTransit messages:

```csharp
await _messageBus.PublishAsync(new StatisticsMessage
{
    Category = "Flow",
    Name = "ExecutionTime",
    Type = StatisticsType.Histogram,
    Value = executionTime,
    Attributes = new Dictionary<string, object>
    {
        { "flow.id", flowId },
        { "execution.id", executionId }
    }
});
```

### MonitoringFramework

The MonitoringFramework provides REST APIs for monitoring:

- `GET /api/monitoring/health` - Get system health status
- `GET /api/monitoring/resources` - Get resource utilization
- `POST /api/monitoring/health` - Record health check
- `POST /api/monitoring/resources` - Record resource utilization

### AlertingSystem

The AlertingSystem evaluates alert rules and sends notifications. Alert rules can be registered via MassTransit messages:

```csharp
await _messageBus.PublishAsync(new AlertRuleMessage
{
    Name = "HighCpuUsage",
    Description = "CPU usage is above 80%",
    Severity = AlertSeverity.Warning,
    Tags = new[] { "performance", "resource" }
});
```

### AnalyticsEngine

The AnalyticsEngine generates reports based on analytics data. Reports can be requested via MassTransit messages:

```csharp
var response = await _messageBus.RequestAsync<ReportRequestMessage, ReportResponseMessage>(new ReportRequestMessage
{
    ReportType = "DailyFlowExecutionReport",
    Parameters = new Dictionary<string, object>
    {
        { "date", DateTime.UtcNow.Date }
    }
});
```
