using BusinessObjects;
using BusinessObjects.DTO.Result;
using BusinessObjects.DTO.Token;
using Core.Base;
using Core.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
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

            // Kiểm tra tồn tại user
            if (user == null)
            {
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = "Sai email hoặc mật khẩu",
                    Data = null
                });
            }

            // Kiểm tra trạng thái tài khoản
            if (user.IsDeleted)
            {
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = "Tài khoản đã bị xóa",
                    Data = null
                });
            }

            if (user.Status != AccountStatus.Active)
            {
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = "Tài khoản chưa được kích hoạt",
                    Data = null
                });
            }

            // ✅ Kiểm tra mật khẩu (dùng PasswordHasher.VerifyPassword)
            bool isPasswordValid = PasswordHasher.VerifyPassword(password, user.PasswordHash);

            if (isPasswordValid)
            {
                // Convert userId to string using .ToString()
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email)
        };

                // Reset refresh token (nếu có)
                _tokenService.ResetRefreshToken();

                // Sinh JWT token
                var token = GenerateToken(user, null);

                // ✅ Trả về kết quả thành công
                return Ok(new ResultDTO
                {
                    IsSuccess = true,
                    Message = "Đăng nhập thành công.",
                    Data = token
                });
            }

            // ❌ Mật khẩu sai
            return BadRequest(new ResultDTO
            {
                IsSuccess = false,
                Message = "Sai email hoặc mật khẩu",
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

        #region Who Am I
        /// <summary>
        /// Get information of the currently authenticated user
        /// </summary>
        /// <returns>Information about the logged-in user</returns>
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { Message = "User is not authenticated" });
            }

            try
            {
                // Lấy claim cơ bản
                var userIdClaim = User.FindFirst("UserId");
                var emailClaim = User.FindFirst("Email");
                var usernameClaim = User.FindFirst("UserName");
                var fullNameClaim = User.FindFirst("FullName");
                var phoneClaim = User.FindFirst("PhoneNumber");
                var roleClaim = User.FindFirst("Role");
                var statusClaim = User.FindFirst("Status");

                // Lấy claim profile
                var teacherProfileClaim = User.FindFirst("TeacherProfileId");
                var centerProfileClaim = User.FindFirst("CenterProfileId");
                var studentProfileClaim = User.FindFirst("StudentProfileId");
                var parentProfileClaim = User.FindFirst("ParentProfileId");

                // Kiểm tra thông tin bắt buộc
                if (userIdClaim == null || emailClaim == null || usernameClaim == null || roleClaim == null)
                {
                    return Unauthorized(new { Message = "Missing essential user information in claims" });
                }

                // Parse role và status
                UserRole role = Enum.TryParse(roleClaim.Value, out UserRole parsedRole)
                    ? parsedRole
                    : UserRole.Student;

                AccountStatus status = Enum.TryParse(statusClaim?.Value, out AccountStatus parsedStatus)
                    ? parsedStatus
                    : AccountStatus.Active;

                // Tạo dictionary động để dễ dàng loại bỏ field null
                var userInfo = new Dictionary<string, object?>
        {
            { "Id", userIdClaim.Value },
            { "Email", emailClaim.Value },
            { "UserName", usernameClaim.Value },
            { "FullName", fullNameClaim?.Value },
            { "PhoneNumber", phoneClaim?.Value },
            { "Role", role.ToString() },
            { "Status", status.ToString() },
            { "TeacherProfileId", teacherProfileClaim?.Value },
            { "CenterProfileId", centerProfileClaim?.Value },
            { "StudentProfileId", studentProfileClaim?.Value },
            { "ParentProfileId", parentProfileClaim?.Value }
        };

                // Loại bỏ các key có giá trị null
                var filteredInfo = userInfo
                    .Where(kv => kv.Value != null)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                return Ok(filteredInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = "Error reading user information",
                    Detail = ex.Message
                });
            }
        }
        #endregion


    }
}
