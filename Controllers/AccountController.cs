using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineLearningPlatform.BLL.Dtos.Auth;
using OnlineLearningPlatform.DAL.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineLearningPlatform.API.Controllers;




[Route("api/[controller]")]
[ApiController]

public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;


    public AccountController(UserManager<AppUser> userManager, IConfiguration configuration,RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;
    }


    [HttpPost("register-Student")]
    public async Task<IActionResult> RegisterStudent(RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var emailExists = await _userManager.FindByEmailAsync(registerDto.Email);
        if (emailExists != null)
            return BadRequest(new { Message = "email is already used" });

        var usernameExists = await _userManager.FindByNameAsync(registerDto.Username);
        if (usernameExists != null)
            return BadRequest(new { Message = "username is already used" });

        var user = new AppUser
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            UserName = registerDto.Username
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Student");
            return Ok(new { Message = "Account created successfully" });
        }

        return BadRequest(result.Errors);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LogginDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);


        var user = await _userManager.FindByEmailAsync(loginDto.UsernameOrEmail)
                   ?? await _userManager.FindByNameAsync(loginDto.UsernameOrEmail);


        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return Unauthorized(new { Message = "username or email in not correct" });
        }


        var token = await GenerateJwtToken(user);

        return Ok(new
        {
            Message = "logged in successfully",
            Token = token,
            Expires = DateTime.UtcNow.AddDays(30)
        });
    }

    private async Task<string> GenerateJwtToken(AppUser user)
    {
       
        var userRoles = await _userManager.GetRolesAsync(user);

       
        var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName??""),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

       
        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var issuer =  "https://localhost:7159";
        var audience = "https://localhost:7159";

       
        var secretKey =  "ThisIsASecretKeyLongEnoughToSecureTheToken123456!";
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            expires: DateTime.UtcNow.AddDays(30),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

     [Authorize(Roles = "Admin")]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(string username,string roleName)
    {
        
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return NotFound("user is not exist");
        }

       
        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            return BadRequest("Role is not founed");
        }

       
        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded)
        {
            return Ok(new { message = "Role assigned successfully" });
        }

       
        return BadRequest(result.Errors);
    }
}