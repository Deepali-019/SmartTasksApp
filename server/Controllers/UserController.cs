using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Dtos;
using server.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
              _configuration = configuration;
            
        }

 
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            // Check if user already exists by email
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                return BadRequest("User already exists.");
            }

            // Hash password before saving
            user.PasswordHash = HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == loginDto.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var hashedInputPassword = HashPassword(loginDto.Password);
            if (user.PasswordHash != hashedInputPassword)
            {
                return BadRequest("Incorrect password.");
            }

             // Generate JWT token
             var token = CreateToken(user);//GenerateJwtToken(user);

             return Ok(new {Message="Login successful.", Token = token });

            //return Ok("Login successful.");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

    private string CreateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            //new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    //    private string CreateToken(User user)
    //     {
    //         var tokenKey = _configuration["Jwt:Key"] ?? throw new Exception("Cannot access tokenKey from appsettings");
    //         if (tokenKey.Length < 64) throw new Exception("Your tokenKey needs to be longer");
    //         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

    //         var claims = new List<Claim>
    //         {
    //             new(ClaimTypes.NameIdentifier, user.Email)
    //         };

    //         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

    //         var tokenDescriptor = new SecurityTokenDescriptor
    //         {
    //             Subject = new ClaimsIdentity(claims),
    //             Expires = DateTime.UtcNow.AddDays(7),
    //             SigningCredentials = creds
    //         };

    //         var tokenHandler = new JwtSecurityTokenHandler();
    //         var token = tokenHandler.CreateToken(tokenDescriptor);

    //         return tokenHandler.WriteToken(token);
    //     }

        // private string GenerateJwtToken(User user)
        // {
        //     var claims = new[]
        //     {
        //         new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //         new Claim(ClaimTypes.Name, user.Username),
        //         new Claim(ClaimTypes.Email, user.Email)
        //     };

        //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //     var token = new JwtSecurityToken(
        //         issuer: _configuration["Jwt:Issuer"],
        //         audience: _configuration["Jwt:Audience"],
        //         claims: claims,
        //         expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
        //         signingCredentials: creds
        //     );

        //     return new JwtSecurityTokenHandler().WriteToken(token);
        // }

        [Authorize]
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [Authorize]
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            return Ok("You are authorized to view this secure data.");
        }
    }   
}