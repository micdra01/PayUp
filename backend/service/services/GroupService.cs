using System.Data.SqlTypes;
using System.Security.Authentication;
using System.Security;
using infrastructure.dataModels;
using infrastructure.models;
using infrastructure.repository;

namespace service.services;

public class GroupService
{
    private readonly GroupRepository _groupRepo;
    private readonly UserRepository _userRepository;
    private readonly NotificationFacade _notificationFacade;
    private readonly NotificationRepository _notificationRepository;



    public GroupService(GroupRepository groupRepo, UserRepository userRepository, NotificationFacade notificationFacade,
        NotificationRepository notificationRepository)
    {
        _groupRepo = groupRepo;
        _userRepository = userRepository;
        _notificationFacade = notificationFacade;
        _notificationRepository = notificationRepository;
    }

    public Group CreateGroup(CreateGroupModel group, SessionData sessionData, string? imageUrl)
    {
        //Create the group
        var createGroup = new CreateGroupModel
        {
            Name = group.Name,
            Description = group.Description,
            CreatedDate = DateTime.UtcNow
        };

        var responseGroup = _groupRepo.CreateGroup(createGroup, imageUrl);
        if (ReferenceEquals(responseGroup, null)) throw new SqlNullValueException(" create group");

        //Add the creator as member(owner) in the group
        UserInGroupDto userInGroupDto = new UserInGroupDto()
            { UserId = sessionData.UserId, GroupId = responseGroup.Id, IsOwner = true };
        var addedToGroup = _groupRepo.AddUserToGroup(userInGroupDto);
        if (!addedToGroup) throw new SqlNullValueException(" add user to the group");
        return responseGroup;
    }

    public Group GetGroupById(int groupId, SessionData sessionData)
    {
        if (!_groupRepo.IsUserInGroup(sessionData.UserId, groupId)) throw new AuthenticationException();
        return _groupRepo.GetGroupById(groupId);
    }

    public IEnumerable<GroupCardModel> GetMyGroups(int userId)
    {
        return _groupRepo.GetMyGroups(userId);
    }

    public IEnumerable<ShortUserDto> GetUsersInGroup(int groupId, SessionData sessionData)
    {
        if (!_groupRepo.IsUserInGroup(sessionData.UserId, groupId)) throw new AuthenticationException();
        return _userRepository.GetAllMembersOfGroup(groupId);
    }

    public bool InviteUserToGroup(SessionData? sessionData, GroupInvitation groupInvitation)
    {
        var ownerId = _groupRepo.IsUserGroupOwner(groupInvitation.GroupId);

        if (sessionData.UserId != ownerId)
            throw new SecurityException("You are not allowed to invite users to this group");

        if (_groupRepo.IsUserInGroup(groupInvitation.ReceiverId, groupInvitation.GroupId))
            throw new ArgumentException("User is already in group");
        
        var fullGroupInvitation = new FullGroupInvitation()
        {
            ReceiverId = groupInvitation.ReceiverId,
            GroupId = groupInvitation.GroupId,
            SenderId = ownerId
        };

        IsNotifyOn(groupInvitation);
        return _groupRepo.InviteUserToGroup(fullGroupInvitation);
    }
    
    private void IsNotifyOn( GroupInvitation groupInvitation)
    {
        var settings = _notificationRepository.GetUserNotificationSettings(groupInvitation.ReceiverId);
        
        if (!settings.InviteNotificationEmail) return;
        
        var group = _groupRepo.GetGroupById(groupInvitation.GroupId);
        var user = _userRepository.GetById(groupInvitation.ReceiverId);
        var isEmailSent = _notificationFacade.SendInviteEmail(group, user.Email);
        if (!isEmailSent)
            throw new SqlNullValueException("send invite per email");
    }
    
    public bool AcceptInvite(SessionData sessionData, bool isAccepted, int groupId)
    {
        var user = new UserInGroupDto()
        {
            GroupId = groupId,
            IsOwner = false,
            UserId = sessionData.UserId
        };
        
        if (isAccepted)
        {
            bool isCreated = _groupRepo.AddUserToGroup(user);
            if (!isCreated)
                throw new SqlTypeException();
        }
        return _groupRepo.DeleteInvite(user);
    }
    
    public Group? Update(int groupId, UpdateGroupModel model, string? imageUrl)
    {
        return _groupRepo.Update(groupId, model, imageUrl);
    }
    
    public bool IsUserGroupOwner(int groupId, SessionData sessionData)
    {
        return _groupRepo.IsUserGroupOwner(groupId) == sessionData.UserId;
    }

}