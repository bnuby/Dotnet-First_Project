using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using First_Project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace First_Project.Data
{
  public class AuthRepository : IAuthRepository
  {
    private DataContext _context;

    private IConfiguration _configuration;
    public AuthRepository(DataContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    public async Task<ServiceResponse<string>> Login(string username, string password)
    {
      var response = new ServiceResponse<string>();
      var user = await _context.Users.FirstOrDefaultAsync(c => c.Username.ToLower().Equals(username.ToLower()));
      if (user == null)
      {
        response.Success = false;
        response.Message = "User not found.";
        return response;
      }
      else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
      {
        response.Success = false;
        response.Message = "Invalid username or password";
      }
      else
      {
        response.Data = CreateToken(user);
      }
      return response;
    }

    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
      ServiceResponse<int> response = new ServiceResponse<int>();

      if (await this.UserExists(user.Username))
      {
        response.Success = false;
        response.Message = "User already exists.";
        return response;
      }
      CreatePasswrodHash(password, out byte[] passwordHash, out byte[] passwordSalt);
      user.PasswordHash = passwordHash;
      user.PasswordSalt = passwordSalt;
      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      response.Data = user.Id;
      return response;
    }

    public async Task<bool> UserExists(string username)
    {
      if (await _context.Users.AnyAsync(u => u.Username.ToLower().Equals(username.ToLower())))
      {
        return true;
      }
      return false;
    }

    private void CreatePasswrodHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (var hmac = new System.Security.Cryptography.HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
      using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
      {
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < passwordHash.Length; i++)
        {
          if (passwordHash[i] != computedHash[i])
          {
            return false;
          }
        }
        return true;
      }
    }

    private string CreateToken(User user)
    {
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username)
      };

      var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      double.TryParse(_configuration.GetSection("AppSettings:TokenValidDays").Value, out double tokenValidDays);

      var tokendDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = System.DateTime.Now.AddDays(tokenValidDays),
        SigningCredentials = creds
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokendDescriptor);

      return tokenHandler.WriteToken(token);
    }
  }
}