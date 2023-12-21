using api.TransferModels;
using infrastructure.dataModels;
using infrastructure.models;
using Microsoft.AspNetCore.Mvc;
using service.services;

namespace api.controllers;

[ApiController]
public class AccountController: ControllerBase
{
    private readonly AccountService _service;
    private readonly JwtService _jwtService;

    public AccountController(AccountService service, JwtService jwtService)
    {
        _service = service;
        _jwtService = jwtService;
    }
    
    [HttpPost]
    [Route("/api/account/register")]
    public ResponseDto Register([FromBody] RegisterModel model)
    {
        var user = _service.Register(model);
        return new ResponseDto { MessageToClient = "Successfully registered", };
    }

    [HttpPost]
    [Route("/api/account/login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        var user = _service.Authenticate(model);
        if (user == null) return Unauthorized();

        var token = _jwtService.IssueToken(SessionData.FromUser(user!));
        return Ok(new { token });
    }

    [HttpPost] //todo should take the email from body and send, and send an 200 status code if password is resend.
    [Route("/api/account/recover")]
    public IActionResult RecoverAccount([FromBody] LoginModel model)
    {
        throw new NotImplementedException("not implemented yet");
    }
}