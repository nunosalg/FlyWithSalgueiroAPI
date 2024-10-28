using FlyWithSalgueiroAPI.Helpers;
using FlyWithSalgueiroAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
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

        [Authorize]
        [HttpPost("uploaduserimage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UploadUserImage(IFormFile image)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (image != null)
            {
                user.AvatarUrl = await _imageHelper.UploadImageAsync(image, "users");

                var response = await _userHelper.UpdateUserAsync(user);
                if(response.Succeeded)
                {
                    return Ok("Image uploaded successfully.");
                }
            }

            return BadRequest("No image uploaded.");
        }


        [Authorize]
        [HttpGet("userimage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserImage()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var userImage = await _userHelper.GetUserImageAsync(userEmail);

            return Ok(userImage);
        }

        [Authorize]
        [HttpGet("userinfo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserInfo()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var model = new ChangeUserViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber,
            };
            
            return Ok(model);
        }

        [Authorize]
        [HttpPost("updateuserinfo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUserInfo(ChangeUserViewModel model)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.BirthDate = model.BirthDate;
            user.PhoneNumber = model.PhoneNumber;
            
            return Ok(user);
        }

        [Authorize]
        [HttpPost("changepassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password changed successfully!");
            }

            return BadRequest("Couldn't change password.");
        }

        [HttpPost("tokentoresetpassword")]
        public async Task<IActionResult> GetTokenToResetPassword(RecoverPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            if (myToken == null) 
            {
                return BadRequest("Couldn't generate token.");
            }

            Response response = await _mailHelper.SendEmailAsync(model.Email, "FWS Password Reset", $"<h1>Fly With Salgueiro Password Reset</h1>" +
                    $"To reset your password use this token:</br></br> {myToken}");
            if (response.IsSuccess)
            {
                return Ok("The token to recover your password has been sent to your email.");
            }

            return BadRequest("Something went wrong...");
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password reset successfully!");
            }

            return BadRequest("Couldn't reset password.");
        }
    }
}
