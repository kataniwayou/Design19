using FlowOrchestrator.Abstractions.Messages;

namespace FlowOrchestrator.ObservabilityBase.Monitoring;

/// <summary>
/// Alert rule.
/// </summary>
public class AlertRule
{
    /// <summary>
    /// Gets or sets the alert rule name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alert rule description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alert condition function.
    /// </summary>
    public Func<Task<bool>> Condition { get; set; } = () => Task.FromResult(false);

    /// <summary>
    /// Gets or sets the alert severity.
    /// </summary>
    public AlertSeverity Severity { get; set; } = AlertSeverity.Info;

    /// <summary>
    /// Gets or sets the alert tags.
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();
}

/// <summary>
/// Alert.
/// </summary>
public class Alert
{
    /// <summary>
    /// Gets or sets the alert name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alert description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alert severity.
    /// </summary>
    public AlertSeverity Severity { get; set; } = AlertSeverity.Info;

    /// <summary>
    /// Gets or sets the alert start timestamp.
    /// </summary>
    public DateTime StartTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the alert end timestamp.
    /// </summary>
    public DateTime? EndTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the alert last update timestamp.
    /// </summary>
    public DateTime LastUpdateTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the alert tags.
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the alert data.
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets the alert duration.
    /// </summary>
    public TimeSpan Duration => EndTimestamp.HasValue ? EndTimestamp.Value - StartTimestamp : DateTime.UtcNow - StartTimestamp;
}

/// <summary>
/// Alert severity.
/// </summary>
public enum AlertSeverity
{
    /// <summary>
    /// Informational alert.
    /// </summary>
    Info = 0,

    /// <summary>
    /// Warning alert.
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error alert.
    /// </summary>
    Error = 2,

    /// <summary>
    /// Critical alert.
    /// </summary>
    Critical = 3
}

/// <summary>
/// Alert notification type.
/// </summary>
public enum AlertNotificationType
{
    /// <summary>
    /// New alert.
    /// </summary>
    New = 0,

    /// <summary>
    /// Updated alert.
    /// </summary>
    Update = 1,

    /// <summary>
    /// Resolved alert.
    /// </summary>
    Resolved = 2
}

/// <summary>
/// Alert notification.
/// </summary>
public class AlertNotification
{
    /// <summary>
    /// Gets or sets the alert.
    /// </summary>
    public Alert Alert { get; set; } = new Alert();

    /// <summary>
    /// Gets or sets the notification type.
    /// </summary>
    public AlertNotificationType NotificationType { get; set; } = AlertNotificationType.New;

    /// <summary>
    /// Gets or sets the notification timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Alert notification message.
/// </summary>
public class AlertNotificationMessage : EventBase
{
    /// <summary>
    /// Gets the alert notification.
    /// </summary>
    public AlertNotification Notification { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlertNotificationMessage"/> class.
    /// </summary>
    /// <param name="notification">The alert notification.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    public AlertNotificationMessage(
        AlertNotification notification,
        string correlationId)
        : base(correlationId)
    {
        Notification = notification;
    }
}
