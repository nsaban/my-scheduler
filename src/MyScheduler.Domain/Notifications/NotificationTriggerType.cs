namespace MyScheduler.Domain.Notifications;

public enum NotificationTriggerType
{
    EventCreated,
    EventUpdated,
    EventCancelled,
    ResponseRecorded,
    EventReminder,
}
