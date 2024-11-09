using FlyWithSalgueiroAPI.Data.Entities;
using FlyWithSalgueiroAPI.Helpers;
using FlyWithSalgueiroAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace FlyWithSalgueiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl = "https://lzl75nls-44301.uks1.devtunnels.ms/";

        public CustomersController(
            IUserHelper userHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper,
            IConfiguration configuration)
        {
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user != null)
            {
                return BadRequest("There is already a user with this email.");
            }

            if (model.BirthDate.AddYears(18) > DateTime.Now)
            {
                return BadRequest("The user must be at least 18 years old to register.");
            }

            user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                BirthDate = model.BirthDate,
                Role = "Customer"
            };

            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result != IdentityResult.Success)
            {
                return BadRequest("Couldn't register user.");
            }

            await _userHelper.AddUserToRoleAsync(user, "Customer");

            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            if (myToken == null)
            {
                return BadRequest("Couldn't generate token.");
            }

            string tokenLink = $"{_baseUrl}api/customers/confirmemail?userid={user.Id}&token={WebUtility.UrlEncode(myToken)}";

            Response response = await _mailHelper.SendEmailAsync(model.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                        $"To finalize the register, " +
                        $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

            if (response.IsSuccess)
            {
                return Ok("Check your email to finalize the register.");
            }

            return BadRequest("Failed to send email with token.");
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Redirect($"https://flywithsalgueiro.somee.com/Account/Login");
            }

            return BadRequest("Invalid email confirmation token.");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null || !await _userHelper.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            var key = _configuration["JWT:Key"] ?? throw new ArgumentNullException("JWT:Key", "JWT:Key cannot be null.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, model.Email!)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new ObjectResult(new
            {
                AccessToken = jwt,
                TokenType = "bearer",
                UserId = user.Id,
                UserName = user.UserName
            });
        }

        [HttpPost("UploadUserImage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UploadUserImage(IFormFile image)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { ErrorMessage = "User not found" });
            }

            if (image != null)
            {
                user.AvatarUrl = await _imageHelper.UploadImageAsync(image, "users");

                var response = await _userHelper.UpdateUserAsync(user);
                if (response.Succeeded)
                {
                    return Ok("Image uploaded successfully.");
                }
            }

            return BadRequest(new { ErrorMessage = "No image uploaded." });
        }


        [HttpGet("UserImage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserImage()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { ErrorMessage = "User not found" });
            }

            var userImage = await _userHelper.GetUserImageAsync(userEmail);

            return Ok(userImage);
        }

        [HttpGet("UserInfo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserInfo()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { ErrorMessage = "User not found" });
            }

            var result = new
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
            };

            return Ok(result);
        }

        [HttpPut("UpdateUserInfo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUserInfo(UpdateUserModel model)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { ErrorMessage = "User not found" });
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.BirthDate = model.BirthDate;

            var result = await _userHelper.UpdateUserAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { Message = "User information updated successfully." });
            }

            return BadRequest(new { ErrorMessage = "Couldn't update user information." });
        }

        [HttpPost("ChangePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { ErrorMessage = "User not found" });
            }

            var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Password changed successfully!" });
            }

            return BadRequest(new { ErrorMessage = "Couldn't change password." });
        }

        [HttpPost("RecoverPassword")]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(new { ErrorMessage = "User not found" });
            }

            var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            if (myToken == null)
            {
                return BadRequest(new { ErrorMessage = "Couldn't generate token." });
            }

            Response response = await _mailHelper.SendEmailAsync(model.Email, "FWS Password Reset", $"<h1>Fly With Salgueiro Password Reset</h1>" +
                    $"To reset your password use this token:</br></br> {myToken}");
            if (response.IsSuccess)
            {
                return Ok(new { Message = "The token to recover your password has been sent to your email." });
            }

            return BadRequest(new { ErrorMessage = "Failed to send email with token." });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(new { ErrorMessage = "User not found" });
            }

            var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Password reset successfully!" });
            }

            return BadRequest(new { ErrorMessage = "Couldn't reset password." });
        }
    }
}
