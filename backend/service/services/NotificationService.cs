using infrastructure.dataModels;
using infrastructure.models;
using infrastructure.repository;

namespace service.services;

public class NotificationService
{
    private readonly GroupRepository _groupRepository;
    private readonly NotificationRepository _notificationRepository;
    
    public NotificationService(GroupRepository groupRepository, NotificationRepository notificationRepository)
    {
        _groupRepository = groupRepository;
        _notificationRepository = notificationRepository;
    }

    public IEnumerable<NotificationDto> GetNotifications(SessionData? sessionData, DateTime lastUpdated)
    {
        var notifications = new List<NotificationDto>();
        
        var inviteNotifications = _groupRepository.GetGroupInviteNotifications(sessionData.UserId, lastUpdated);
        foreach (var notification in inviteNotifications)
        {
            var result = NotificationFacade.CreateInviteNotification(notification);
            notifications.Add(result);
        }
        return notifications;
    }

    public NotificationSettingsDto GetNotificationsSettings(int userId)
    {
        return _notificationRepository.GetUserNotificationSettings(userId);
    }
    
    public void EditUserNotificationSettings(NotificationSettingsDto settingsDto)
    {
        _notificationRepository.EditUserNotificationSettings(settingsDto);
    }
}