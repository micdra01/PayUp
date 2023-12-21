using System.ComponentModel.DataAnnotations;

namespace infrastructure.models;

public enum NotificationCategory
{
    GroupInvite = 1
}

public class NotificationDto
{
    public Enum Category { get; set; }
    
    [Required]
    public string Subject { get; set; }
    [Required]
    public string Body { get; set; }//todo shoul be max 50 chars
    [Required]
    public string Footer { get; set; }
    [Required]
    public DateTime InviteReceived { get; set; }
}


public class GroupInviteNotification
{
    [Required]
    public int GroupId { get; set; }
    [Required]
    public string GroupName { get; set; }
    [Required]
    public string GroupDescription { get; set; }//todo shoul be max 50 chars
    [Required]
    public string SenderFullName { get; set; }
    [Required]
    public DateTime InviteReceived { get; set; }
}

public class GroupInviteDto
{
    [Required]
    public bool Accepted  { get; set; }
    [Required]
    public int GroupId{ get; set; }
    
}

public class NotificationSettingsDto
{
    [Required]
    public int UserId { get; set; }
    [Required]
    public bool InviteNotification { get; set; }
    [Required]
    public bool InviteNotificationEmail { get; set; }
    [Required]
    public bool ExpenseNotification { get; set; }
    [Required]
    public bool ExpenseNotificationEmail { get; set; }
}
