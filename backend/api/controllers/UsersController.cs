using api.filters;
using api.models;
using api.TransferModels;
using infrastructure.dataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using service.services;

namespace api.controllers;

[ApiController]
public class UsersController : ControllerBase
{
    
    private readonly UserService _service;
    private readonly BlobService _blobService;

    public UsersController(
        UserService service, 
        BlobService blobService
    )
    {
        _service = service;
        _blobService = blobService;
    }
    
    [RequireAuthentication]
    [HttpGet]
    [Route("/api/user/currentuser")]
    public User GetLoggedInUser()   
    {
        var data = HttpContext.GetSessionData();
        var user = _service.GetLoggedInUser(data!);
        return user;
    }

    [RequireAuthentication]
    [HttpPut]
    [Route("/api/user/profileinfo")]
    public User EditProfileInfo([FromBody] UserInfoDto model)
    {
        var data = HttpContext.GetSessionData();
        var user = _service.EditProfileInfo(data, model);
        return user;
    }
    
    [RequireAuthentication]
    [HttpPut]
    [Route("/api/user/profileimage")]
    public User EditProfileImage([FromForm] IFormFile? image)
    {
        var session = HttpContext.GetSessionData()!;

        var imageUrl = _service.GetLoggedInUser(session).ProfileUrl;
        if (image != null)
        {
            // We need a stream of bytes (image data)
            using var imageStream = image.OpenReadStream();
            imageUrl = _blobService.Save("payup", imageStream, null);
        }
        
        return _service.EditProfileImage(session, imageUrl);
    }
    
    [RequireAuthentication]
    [HttpDelete]
    [Route("/api/account/profile")]
    public ResponseDto DeleteProfile()
    {
        var data = HttpContext.GetSessionData();
        var userDeleted = _service.DeleteAccount(data);
        return new ResponseDto
        {
            ResponseData = userDeleted
        };
    }

    [RequireAuthentication]
    [HttpGet]
    [Route("/api/user")]
    public IEnumerable<InvitableUser> GetInvitableUsers(
        [FromQuery] string searchQuery, 
        [FromQuery] int currentPage,
        [FromQuery] int pageSize,
        [FromQuery] int groupId)
    {
        SessionData? data = HttpContext.GetSessionData();

        Pagination pagination = new Pagination()
        {
            CurrentPage = currentPage,
            PageSize = pageSize
        };
        
        InvitableUserSearch invitableUserSearch = new InvitableUserSearch()
        {
            SearchQuery = searchQuery.IsNullOrEmpty() ? "" : searchQuery,
            Pagination = pagination,
            GroupId = groupId
        };
        
        return _service.GetInvitableUsers(data, invitableUserSearch);
    }
    
}