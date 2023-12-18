using System.Data.SqlTypes;
using System.Security;
using api.models;
using infrastructure.dataModels;
using infrastructure.repository;

namespace service.services;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly GroupRepository _groupRepository;

    public UserService(UserRepository userRepo, GroupRepository groupRepository)
    {
        _userRepository = userRepo;
        _groupRepository = groupRepository;
    }

    public User GetLoggedInUser(SessionData data)
    {
        var user = _userRepository.GetById(data.UserId);
        if (ReferenceEquals(user, null)) throw new SqlNullValueException();
        return user;
    }

    public User EditProfileInfo(SessionData? data, UserInfoDto user)
    {
        var responseUser = _userRepository.EditUserInfo(user, data.UserId);
        if (ReferenceEquals(responseUser, null)) throw new SqlNullValueException("Edit User");//checks if response user is null before returning it.
        return responseUser; 
    }

    public User EditProfileImage(SessionData? data, string? imageUrl)
    {
        var responseUser = _userRepository.EditUserImage(imageUrl, data.UserId);
        //var responseUser = _userRepository.EditUserInfo(user, data.UserId);
        if (ReferenceEquals(responseUser, null)) throw new SqlNullValueException("Edit User");//checks if response user is null before returning it.
        return responseUser; 
    }

    public IEnumerable<InvitableUser> GetInvitableUsers(SessionData? data, InvitableUserSearch invitableUserSearch)
    {
        int groupOwnerId = _groupRepository.IsUserGroupOwner(invitableUserSearch.GroupId);
        if (groupOwnerId != data.UserId) throw new SecurityException("You are not allowed to invite users to this group");
        return _userRepository.GetInvitableUsers(invitableUserSearch);
    }
    
    
    public bool DeleteAccount(SessionData? data)
    {
        var wasDeleted = _userRepository.DeleteUser(data.UserId);
        if (!wasDeleted) throw new SqlNullValueException(" delete user");//checks if response is true before returning it.
        return wasDeleted; 
    }
}