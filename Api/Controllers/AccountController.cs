using Api.DTOs.Account;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtService jwtService;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public AccountController(JwtService jwtService,SignInManager<User> signInManager,UserManager<User> userManager)
        {
            this.jwtService = jwtService;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email) ?.Value);
            return  CreateApplicationUserDto(user);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if(user == null)
            {
                return Unauthorized("Invalid username or password") ;
            }
            if(user.EmailConfirmed == false)
            {
                return Unauthorized("Please confirm your email");

            }
            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Invalid Username Or Password");
            }
            return CreateApplicationUserDto(user); 
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register(RegisterDto model)
        {
            if(await CheckEmailExistAsync(model.Email))
            {
                return BadRequest($"An existing account is using {model.Email}, Emailaddress.Please try with another email address"); 
            }
            var userToAdd = new User
            {
                FirstName = model.FirstName.ToLower(),
                LastName = model.LastName.ToLower(),
                UserName = model.Email.ToLower(),
                Email = model.Email.ToLower(),
                EmailConfirmed =true
            };
            var result = await userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) { return BadRequest(result.Errors); }
            return Ok("Your account has been created , you can login");

        }


        private UserDto CreateApplicationUserDto(User user)
        {
            return new UserDto 
            {
            FirstName = user.FirstName,
            LastName = user.LastName,
            JWT = jwtService.CreateJWT(user)
            };
        }
        private async Task<bool>CheckEmailExistAsync(string email)
        {
            return await userManager.Users.AnyAsync(u => u.Email == email.ToLower()); 
        }
       

    }
}
