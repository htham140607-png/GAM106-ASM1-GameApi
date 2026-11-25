using GameApi.Data;
using GameApi.DTOs;
using GameApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace GameApi.Services;
public class AuthService(AppDbContext db, IConfiguration config)
{
    private readonly PasswordHasher<Account> hasher = new();
    public async Task<AccountResponse?> Register(RegisterRequest request)
    {
        if (await db.Accounts.AnyAsync(a => a.Username == request.Username)) return null;
        var account = new Account { Username = request.Username };
        account.PasswordHash = hasher.HashPassword(account, request.Password);
        db.Accounts.Add(account);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlConnector.MySqlException { Number: 1062 }) { return null; }
        return new(account.Id, account.Username);
    }
    public async Task<LoginResponse?> Login(LoginRequest request)
    {
        var account = await db.Accounts.SingleOrDefaultAsync(a => a.Username == request.Username);
        if (account is null || hasher.VerifyHashedPassword(account, account.PasswordHash, request.Password) == PasswordVerificationResult.Failed) return null;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var token = new JwtSecurityToken(claims: new[] { new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()), new Claim(ClaimTypes.Name, account.Username) }, expires: DateTime.UtcNow.AddDays(1), signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new(new JwtSecurityTokenHandler().WriteToken(token), account.Username);
    }
}
