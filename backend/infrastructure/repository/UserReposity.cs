using System.Data.SqlTypes;
using Dapper;
using infrastructure.dataModels;
using infrastructure.models;
using Npgsql;

namespace infrastructure.repository;

public class UserRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public UserRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }


    public User Create(RegisterModel model, DateTime created)
    {
        var sql = $@"
            INSERT INTO users.user (email, full_name, phone_number, created, profile_url) " +
                  "VALUES (@Email, @FullName, @PhoneNumber, @Created, @ProfileUrl) " +
                  "RETURNING *";

        var user = new RegisterModel
        {
            Email = model.Email,
            FullName = model.FullName,
            PhoneNumber = model.PhoneNumber,
            Created = created,
            ProfileUrl = model.ProfileUrl,
            Password = null,
        };

        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.QueryFirst<User>(sql, user);
        }
        catch(Exception e)
        {
            throw new SqlTypeException("could not create user", e);
        }
    }

    public User? GetById(int userId)
    {
        var sql = @$" 
    SELECT
        id as {nameof(User.Id)},
        email as {nameof(User.Email)},
        full_name as {nameof(User.FullName)},
        phone_number as {nameof(User.PhoneNumber)},
        created as Created,
        profile_url as ProfileUrl
        FROM users.user
        WHERE id = @userId;";
        
        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.QueryFirstOrDefault<User>(sql, new { userId });
        }
        catch(Exception e)
        {
            throw new SqlTypeException("Could not retrieve user", e);
        }
    }
    
    public User? GetByEmail(string email)
    {
        var sql = @"
        SELECT * FROM users.user
        WHERE email = @email;";

        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.QueryFirstOrDefault<User>(sql, new { email });
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Could not retrieve user", e);
        }
    }

    public User? EditUserInfo(UserInfoDto user, int id)
    {
        user.Id = id;
        var updateSql = @$"
        UPDATE users.user
        SET
            full_name = @{nameof(user.FullName)},
            email = @{nameof(user.Email)},
            phone_number = @{nameof(user.PhoneNumber)} 
        WHERE id = @Id
        RETURNING id as {nameof(User.Id)},
                  email as {nameof(User.Email)},
                  full_name as {nameof(User.FullName)},
                  phone_number as {nameof(User.PhoneNumber)},
                  created as Created,
                  profile_url as ProfileUrl;";
        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.QueryFirstOrDefault<User>(updateSql, user);
        }
        catch(Exception e)
        {
            throw new SqlTypeException("Could not update user", e);
        }
    }
    
    public User? EditUserImage(string imageUrl, int id)
    {
        var sql = @$"
        UPDATE users.user
        SET
            profile_url = @imageUrl
        WHERE id = @id
        RETURNING id as {nameof(User.Id)},
                  email as {nameof(User.Email)},
                  full_name as {nameof(User.FullName)},
                  phone_number as {nameof(User.PhoneNumber)},
                  created as {nameof(User.Created)},
                  profile_url as {nameof(User.ProfileUrl)};";
        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.QueryFirst<User>(sql, new {imageUrl, id});
        }
        catch(Exception e)
        {
            throw new SqlTypeException("Could not update user", e);
        }
    }

    public IEnumerable<InvitableUser> GetInvitableUsers(InvitableUserSearch invitableUserSearch)
    {
        invitableUserSearch.SearchQuery = invitableUserSearch.SearchQuery.Insert(0, "%");
        invitableUserSearch.SearchQuery += "%";
        
        string sql = @"
                SELECT users.user.id, users.user.full_name AS FullName, users.user.profile_url AS ProfileUrl
                FROM users.user
                WHERE users.user.id NOT IN (SELECT groups.group_members.user_id FROM groups.group_members WHERE group_members.group_id = @GroupId)
                AND users.user.id NOT IN (SELECT groups.group_invitation.receiver_id FROM groups.group_invitation WHERE group_invitation.group_id = @GroupId)
                AND users.user.full_name LIKE @SearchQuery
                LIMIT @PageSize
                OFFSET @CurrentPage";
        try
        {
            using NpgsqlConnection conn = _dataSource.OpenConnection();
            return conn.Query<InvitableUser>(sql, new
            {
                invitableUserSearch.SearchQuery,
                invitableUserSearch.GroupId,
                CurrentPage = invitableUserSearch.Pagination.CurrentPage * invitableUserSearch.Pagination.PageSize,
                invitableUserSearch.Pagination.PageSize
            });
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Could not retrieve invitable users", e);
        }
    }


    public bool DeleteUser(int userId)
    {
        throw new NotImplementedException("not implemented in repo");
    }

    public IEnumerable<ShortUserDto> GetAllMembersOfGroup(int groupId)
    {
        var sql = @$" 
    SELECT
    user_id as Id,
    full_name as FullName,
    profile_url as ProfileUrl
    FROM users.user
    JOIN groups.group_members ON users.user.id = groups.group_members.user_id
    WHERE groups.group_members.group_id = @groupId;";
        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.Query<ShortUserDto>(sql, new {groupId});
        }
        catch(Exception e)
        {
            throw new SqlTypeException("Could not retrieve users", e);
        }
    }
}