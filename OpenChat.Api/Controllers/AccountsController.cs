using Microsoft.AspNetCore.Mvc;

using OpenChat.Api.Models;
using OpenChat.Domain.Users;
using OpenChat.Domain.Users.Results;

namespace OpenChat.Api.Controllers
{
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public AccountsController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("users")]
        public IActionResult Register(RegisterRequest registerRequest)
        {
            var userDto = new UserDto(
                username: registerRequest.Username!,
                password: registerRequest.Password!,
                about: registerRequest.About!
            );

            var result = _usersService.Create(userDto);

            if (result.IsFailed && result.Reason == CreateUserError.UserAlreadyExists)
            {
                return BadRequest("Username already in use.");
            }

            var response = new UserResponse(
                id: result.Value!.Id!.Value,
                username: result.Value.Username,
                about: result.Value.About!);

            return Ok(response);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest loginRequest)
        {
            var result = _usersService.GetByCredentials(loginRequest.Username!, loginRequest.Password!);

            if (result.IsFailed && result.Reason == GetUserError.NotFound)
            {
                return BadRequest("Invalid credentials.");
            }

            var response = new UserResponse(
                id: result.Value!.Id!.Value,
                username: result.Value.Username,
                about: result.Value.About!);

            return Ok(response);
        }

        [HttpGet("users")]
        public IActionResult GetAll()
        {
            var users = _usersService.GetAll()
                .Select(user => new UserResponse(
                    id: user.Id!.Value,
                    username: user.Username,
                    about: user.About!
                ))
                .ToHashSet();

            return Ok(users);
        }
    }
}