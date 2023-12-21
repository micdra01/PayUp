using api.filters;
using infrastructure.dataModels;
using infrastructure.models;
using Microsoft.AspNetCore.Mvc;
using service.services;

namespace api.controllers;

[ApiController]
public class NotificationController: ControllerBase
{
    private readonly NotificationService _service;

    public NotificationController(NotificationService service)
    {
        _service = service;
    }
    
    [RequireAuthentication]
    [HttpGet]
    [Route("/api/user/notifications")]
    public IEnumerable<NotificationDto> GetNotifications([FromHeader] DateTime lastUpdated)
    {
        SessionData? sessionData = HttpContext.GetSessionData();
        return _service.GetNotifications(sessionData, lastUpdated);
    }
    
    [RequireAuthentication]
    [HttpGet]
    [Route("/api/user/profileinfo/settings")]
    public NotificationSettingsDto GetNotificationSettings()
    {
        SessionData? sessionData = HttpContext.GetSessionData();
        var settings = _service.GetNotificationsSettings(sessionData!.UserId);
        return settings;
    }
    
    
    [RequireAuthentication]
    [HttpPut]
    [Route("/api/user/profileinfo/settings")]
    public IActionResult EditNotificationSettings([FromBody] NotificationSettingsDto settingsDto)
    {
        SessionData? sessionData = HttpContext.GetSessionData(); 
        settingsDto.UserId = sessionData!.UserId; // Set the user ID from the session data
        _service.EditUserNotificationSettings(settingsDto);
        return Ok(new { Message = "Notification settings updated successfully" });
    }
}