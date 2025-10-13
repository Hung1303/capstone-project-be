using BusinessObjects;
using BusinessObjects.DTO.Result;
using BusinessObjects.DTO.Token;
using Core.Base;
using Core.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        #region GenerateToken
        /// <summary>
        /// Which will generating token accessible for user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [NonAction]
        public TokenDTO GenerateToken(User user, String? RT)
        {
            List<Claim> claims = new List<Claim>()
    {
        new Claim("UserId", user.Id.ToString()),
        new Claim("UserName", user.UserName),
        new Claim("Email", user.Email),
        new Claim("Role", user.Role.ToString()),
        new Claim("PhoneNumber", user.PhoneNumber ?? ""),
        new Claim("FullName", user.FullName ?? ""),
        new Claim("PasswordHash", user.PasswordHash.ToString()),
    };
            //if (user.Role == UserRole.Seller || user.Role == UserRole.Shipper)
            //{
            //    claims.Add(new Claim("BankName", user.BankName ?? ""));
            //    claims.Add(new Claim("BankAccountNumber", user.BankAccountNumber ?? ""));
            //    claims.Add(new Claim("BankAccountName", user.BankAccountName ?? ""));
            //}
            //if (user.Role == UserRole.Seller)
            //{
            //    claims.Add(new Claim("ShopName", user.ShopName ?? ""));
            //    claims.Add(new Claim("ShopDescription", user.shopDescription ?? ""));
            //    claims.Add(new Claim("BusinessType", user.BusinessType ?? ""));
            //}
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes("c2VydmVwZXJmZWN0bHljaGVlc2VxdWlja2NvYWNoY29sbGVjdHNsb3Bld2lzZWNhbWU="));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "YourIssuer",
                audience: "YourAudience",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            if (RT != null)
            {
                return new TokenDTO()
                {
                    AccessToken = accessToken,
                    RefreshToken = RT,
                    ExpiredAt = _tokenService.GetRefreshTokenByUserID(user.Id).ExpiredTime
                };
            }
            return new TokenDTO()
            {
                AccessToken = accessToken,
                RefreshToken = GenerateRefreshToken(user),
                ExpiredAt = _tokenService.GetRefreshTokenByUserID(user.Id).ExpiredTime
            };
        }
        #endregion

        #region GenerateRefreshToken
        // Hàm tạo refresh token
        [NonAction]
        public string GenerateRefreshToken(User user)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string refreshtoken = Convert.ToBase64String(randomnumber);

                var refreshTokenEntity = new Token
                {
                    UserId = user.Id,
                    AccessToken = new Random().Next().ToString(),
                    RefreshToken = refreshtoken,
                    ExpiredTime = DateTime.Now.AddDays(7),
                    Status = 1
                };

                _tokenService.GenerateRefreshToken(refreshTokenEntity);
                return refreshtoken;
            }
        }

        #endregion

        #region Login

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(string email, string password)
        {
            var user = _userService.GetUserByEmailAsync(email).Result;
            if (user.IsDeleted == false && user.Status == AccountStatus.Active)
            {
                if (user != null && user.IsDeleted == false)
                {
                    // Hash the input password with SHA256
                    var hashedInputPasswordString = PasswordHasher.HashPassword(password);

                    if (hashedInputPasswordString == user.PasswordHash)
                    {
                        // Convert userId to string using .ToString()
                        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Email)
    };
                        // Compare the hashed input password with the stored hashed password
                        _tokenService.ResetRefreshToken();
                        var token = GenerateToken(user, null);
                        return Ok(new ResultDTO // Wrap the TokenDTO in a ResultDTO
                        {
                            IsSuccess = true,
                            Message = "Đăng nhập thành công.",
                            Data = token // The TokenDTO object
                        });
                    }
                }
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = "Sai email hoặc mật khẩu",
                    Data = null
                });
            }
            return BadRequest(new ResultDTO
            {
                IsSuccess = false,
                Message = "Tài khoản đã bị xóa",
                Data = null
            });
        }
        #endregion

        #region Logout
        [HttpPost]
        [Route("Logout")]
        public IActionResult Logout()
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return BadRequest(new ResultDTO
                    {
                        IsSuccess = false,
                        Message = "Invalid token format."
                    });
                }

                token = token.Split(' ')[1];
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("c2VydmVwZXJmZWN0bHljaGVlc2VxdWlja2NvYWNoY29sbGVjdHNsb3Bld2lzZWNhbWU=")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false
                };

                SecurityToken validatedToken;
                var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
                var userIdClaim = claimsPrincipal.FindFirst("UserId");

                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    // Handle the case where the UserId claim is missing or invalid
                    return BadRequest(new ResultDTO
                    {
                        IsSuccess = false,
                        Message = "Invalid UserId."
                    });
                }

                var refreshToken = _tokenService.GetRefreshTokenByUserID(userId);
                _tokenService.UpdateRefreshToken(refreshToken);
                _tokenService.ResetRefreshToken();

                if (HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    HttpContext.Request.Headers.Remove("Authorization");
                }

                return Ok(new ResultDTO
                {
                    IsSuccess = true,
                    Message = "Logout successfully!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = "Something went wrong: " + ex.Message
                });
            }
        }
        #endregion

    }
}
