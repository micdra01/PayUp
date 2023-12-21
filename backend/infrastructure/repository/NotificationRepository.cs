using System.Data.SqlTypes;
using Dapper;
using infrastructure.models;
using Npgsql;

namespace infrastructure.repository;

public class NotificationRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NotificationRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
    
    public bool CreateUserNotificationSettings(NotificationSettingsDto settingsDto)
    {
        var sql = @"
        INSERT INTO users.user_notification_settings (user_id, invite_notification, invite_notification_email, expense_notification, expense_notification_email)
        VALUES (@UserId, @InviteNotification, @InviteNotificationEmail, @ExpenseNotification, @ExpenseNotificationEmail);
    ";
        try
        {
            using var conn = _dataSource.OpenConnection();
            int rowsAffected = conn.Execute(sql, settingsDto);

            // If at least one row was affected, the object was created successfully
            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Could not create user notification settings", e);
        }
    }
    
    public NotificationSettingsDto GetUserNotificationSettings(int userId)
    {
        var sql = @"
        SELECT
            user_id AS UserId,
            invite_notification AS InviteNotification,
            invite_notification_email AS InviteNotificationEmail,
            expense_notification AS ExpenseNotification,
            expense_notification_email AS ExpenseNotificationEmail
        FROM users.user_notification_settings
        WHERE user_id = @UserId;
    ";

        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.QueryFirst<NotificationSettingsDto>(sql, new { userId });
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to retrieve user notification settings", e);
        }
    }
    
      
    public void EditUserNotificationSettings(NotificationSettingsDto settingsDto)
    {
        var sql = @"
        UPDATE users.user_notification_settings
        SET
            invite_notification = @InviteNotification,
            invite_notification_email = @InviteNotificationEmail,
            expense_notification = @ExpenseNotification,
            expense_notification_email = @ExpenseNotificationEmail
        WHERE
            user_id = @UserId;
    ";
        try
        {
            using var conn = _dataSource.OpenConnection();
            conn.Execute(sql, settingsDto);
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to update user notification settings", e);
        }
    }
}