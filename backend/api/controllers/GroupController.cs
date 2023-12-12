using api.filters;
using api.models;
using infrastructure.dataModels;
using Microsoft.AspNetCore.Mvc;
using service.services;

namespace api.controllers;

[ApiController]
public class GroupController : ControllerBase
{
    private readonly GroupService _service;
    private readonly BlobService _blobService;

    public GroupController(
        GroupService service, 
        BlobService blobService
    )
    {
        _service = service;
        _blobService = blobService;
    }

    [RequireAuthentication]
    [HttpPost]
    [Route("/api/group/create")]
    public Group CreateGroup([FromForm] CreateGroupModel group, IFormFile? image)
    {
        var sessionData = HttpContext.GetSessionData();
        var imageUrl = "https://micdrastorageaccount.blob.core.windows.net/payup/65721ca9-0517-47b2-91f1-05ad8b176132";
        if (image != null)
        {
            // We need a stream of bytes (image data)
            using var imageStream = image.OpenReadStream();
            // "avatar" is the container name
            imageUrl = _blobService.Save("payup", imageStream, null);
        }
        
        return _service.CreateGroup(group, sessionData!, imageUrl);
    }

    [RequireAuthentication]
    [HttpGet]
    [Route("/api/group/{groupId}")]
    public Group GetGroupById([FromRoute] int groupId)
    {
        return _service.GetGroupById(groupId, HttpContext.GetSessionData()!);
    }

    [RequireAuthentication]
    [HttpGet]
    [Route("/api/mygroups")]
    public IEnumerable<GroupCardModel> GetMyGroups()
    {
        var sessionData = HttpContext.GetSessionData();
        var userId = sessionData.UserId;
        return _service.GetMyGroups(userId);
    }

    [RequireAuthentication]
    [HttpPost]
    [Route("/api/group/invite")]
    public bool InviteUserToGroup([FromBody] GroupInvitation groupInvitation)
    {
        bool success = _service.InviteUserToGroup(HttpContext.GetSessionData(), groupInvitation);
        if (success) HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return success;
    }

    [RequireAuthentication]
    [HttpGet]
    [Route("/api/group/{groupId}/users")]
    public IEnumerable<ShortUserDto> GetUsersInGroup([FromRoute] int groupId)
    {
        return _service.GetUsersInGroup(groupId, HttpContext.GetSessionData()!);
    }

    [RequireAuthentication]
    [HttpPost]
    [Route("/api/user/accept-invite")]
    public bool AcceptInvite([FromBody] GroupInviteDto inviteAnswer)
    {
        bool success = _service.AcceptInvite(HttpContext.GetSessionData(), inviteAnswer.Accepted, inviteAnswer.GroupId);
        if (success) HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return success;
    }
    
    [RequireAuthentication]
    [HttpPut]
    [Route("/api/group/{groupId}/update")]
    public Group Update([FromForm] UpdateGroupModel model, [FromRoute] int groupId, IFormFile? image)
    {
        var session = HttpContext.GetSessionData()!;
        
        var imageUrl = _service.GetGroupById(groupId, session).ImageUrl;;
        if (image != null)
        {
            // We need a stream of bytes (image data)
            using var imageStream = image.OpenReadStream();
            imageUrl = _blobService.Save("payup", imageStream, null);
        }
        
        return _service.Update(groupId, model, imageUrl);
    }
    
    [RequireAuthentication]
    [HttpGet]
    [Route("/api/group/{groupId}/owner")]
    public bool IsUserGroupOwner([FromRoute] int groupId)
    {
        return _service.IsUserGroupOwner(groupId, HttpContext.GetSessionData());
    }
}