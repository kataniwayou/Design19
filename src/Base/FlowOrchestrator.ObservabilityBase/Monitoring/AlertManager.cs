using FlowOrchestrator.Common.Logging;
using FlowOrchestrator.Infrastructure.Common.Messaging;
using FlowOrchestrator.Infrastructure.Common.Telemetry;

namespace FlowOrchestrator.ObservabilityBase.Monitoring;

/// <summary>
/// Alert manager.
/// </summary>
public class AlertManager
{
    private readonly ILogger _logger;
    private readonly ITelemetryProvider _telemetryProvider;
    private readonly IMessageBus _messageBus;
    private readonly Dictionary<string, AlertRule> _alertRules = new Dictionary<string, AlertRule>();
    private readonly Dictionary<string, Alert> _activeAlerts = new Dictionary<string, Alert>();

    /// <summary>
    /// Initializes a new instance of the <see cref="AlertManager"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="telemetryProvider">The telemetry provider.</param>
    /// <param name="messageBus">The message bus.</param>
    public AlertManager(
        ILogger logger,
        ITelemetryProvider telemetryProvider,
        IMessageBus messageBus)
    {
        _logger = logger;
        _telemetryProvider = telemetryProvider;
        _messageBus = messageBus;
    }

    /// <summary>
    /// Registers an alert rule.
    /// </summary>
    /// <param name="name">The alert rule name.</param>
    /// <param name="condition">The alert condition function.</param>
    /// <param name="severity">The alert severity.</param>
    /// <param name="description">The alert description.</param>
    /// <param name="tags">The alert tags.</param>
    public void RegisterAlertRule(
        string name,
        Func<Task<bool>> condition,
        AlertSeverity severity,
        string description = "",
        IEnumerable<string>? tags = null)
    {
        try
        {
            var alertRule = new AlertRule
            {
                Name = name,
                Description = description,
                Condition = condition,
                Severity = severity,
                Tags = tags?.ToList() ?? new List<string>()
            };

            _alertRules[name] = alertRule;
            _logger.Info($"Registered alert rule: {name}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to register alert rule: {name}", ex);
        }
    }

    /// <summary>
    /// Evaluates all alert rules.
    /// </summary>
    /// <returns>The active alerts.</returns>
    public async Task<IReadOnlyDictionary<string, Alert>> EvaluateAlertRulesAsync()
    {
        using var span = _telemetryProvider.CreateSpan("AlertManager.EvaluateAlertRules");

        try
        {
            _logger.Info("Evaluating alert rules");

            foreach (var alertRule in _alertRules.Values)
            {
                try
                {
                    await EvaluateAlertRuleAsync(alertRule);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to evaluate alert rule: {alertRule.Name}", ex);
                }
            }

            _logger.Info($"Alert rules evaluated. Active alerts: {_activeAlerts.Count}");
            span.SetStatus(SpanStatus.Ok);
            return _activeAlerts;
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to evaluate alert rules", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return _activeAlerts;
        }
    }

    /// <summary>
    /// Evaluates a specific alert rule.
    /// </summary>
    /// <param name="name">The alert rule name.</param>
    /// <returns>True if the alert is active, false otherwise.</returns>
    public async Task<bool> EvaluateAlertRuleAsync(string name)
    {
        using var span = _telemetryProvider.CreateSpan("AlertManager.EvaluateAlertRule");
        span.SetAttribute("alert.rule.name", name);

        try
        {
            _logger.Info($"Evaluating alert rule: {name}");

            if (!_alertRules.TryGetValue(name, out var alertRule))
            {
                _logger.Warn($"Alert rule not found: {name}");
                span.SetStatus(SpanStatus.Error, $"Alert rule not found: {name}");
                return false;
            }

            var isActive = await EvaluateAlertRuleAsync(alertRule);
            _logger.Info($"Alert rule evaluated: {name}. Active: {isActive}");

            span.SetStatus(SpanStatus.Ok);
            return isActive;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to evaluate alert rule: {name}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return false;
        }
    }

    private async Task<bool> EvaluateAlertRuleAsync(AlertRule alertRule)
    {
        using var span = _telemetryProvider.CreateSpan("AlertManager.EvaluateAlertRule");
        span.SetAttribute("alert.rule.name", alertRule.Name);

        try
        {
            var isActive = await alertRule.Condition();

            if (isActive)
            {
                if (!_activeAlerts.ContainsKey(alertRule.Name))
                {
                    // New alert
                    var alert = new Alert
                    {
                        Name = alertRule.Name,
                        Description = alertRule.Description,
                        Severity = alertRule.Severity,
                        StartTimestamp = DateTime.UtcNow,
                        LastUpdateTimestamp = DateTime.UtcNow,
                        Tags = alertRule.Tags
                    };

                    _activeAlerts[alertRule.Name] = alert;
                    await SendAlertNotificationAsync(alert, AlertNotificationType.New);
                }
                else
                {
                    // Update existing alert
                    var alert = _activeAlerts[alertRule.Name];
                    alert.LastUpdateTimestamp = DateTime.UtcNow;
                    await SendAlertNotificationAsync(alert, AlertNotificationType.Update);
                }
            }
            else
            {
                if (_activeAlerts.ContainsKey(alertRule.Name))
                {
                    // Resolve alert
                    var alert = _activeAlerts[alertRule.Name];
                    alert.EndTimestamp = DateTime.UtcNow;
                    await SendAlertNotificationAsync(alert, AlertNotificationType.Resolved);
                    _activeAlerts.Remove(alertRule.Name);
                }
            }

            // Record telemetry
            _telemetryProvider.RecordMetric(
                "alert.active",
                isActive ? 1 : 0,
                new Dictionary<string, object>
                {
                    { "alert.rule.name", alertRule.Name },
                    { "alert.severity", alertRule.Severity.ToString() }
                });

            span.SetStatus(SpanStatus.Ok);
            return isActive;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to evaluate alert rule: {alertRule.Name}", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
            return false;
        }
    }

    private async Task SendAlertNotificationAsync(Alert alert, AlertNotificationType notificationType)
    {
        using var span = _telemetryProvider.CreateSpan("AlertManager.SendAlertNotification");
        span.SetAttribute("alert.name", alert.Name);
        span.SetAttribute("alert.notification.type", notificationType.ToString());

        try
        {
            _logger.Info($"Sending alert notification: {alert.Name} ({notificationType})");

            var notification = new AlertNotification
            {
                Alert = alert,
                NotificationType = notificationType,
                Timestamp = DateTime.UtcNow
            };

            await _messageBus.PublishAsync(new AlertNotificationMessage(notification, Guid.NewGuid().ToString()));
            _logger.Info($"Alert notification sent: {alert.Name} ({notificationType})");

            span.SetStatus(SpanStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to send alert notification: {alert.Name} ({notificationType})", ex);
            span.SetStatus(SpanStatus.Error, ex.Message);
            span.RecordException(ex);
        }
    }

    /// <summary>
    /// Gets all active alerts.
    /// </summary>
    /// <returns>The active alerts.</returns>
    public IReadOnlyDictionary<string, Alert> GetActiveAlerts()
    {
        return _activeAlerts;
    }

    /// <summary>
    /// Gets an active alert.
    /// </summary>
    /// <param name="name">The alert name.</param>
    /// <returns>The alert, or null if not found.</returns>
    public Alert? GetActiveAlert(string name)
    {
        return _activeAlerts.TryGetValue(name, out var alert) ? alert : null;
    }
}
