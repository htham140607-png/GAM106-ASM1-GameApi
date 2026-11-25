using GameApi.Data;
using GameApi.DTOs;
using GameApi.Models;
using Microsoft.EntityFrameworkCore;
namespace GameApi.Services;
public class CharacterService(AppDbContext db)
{
    private static CharacterResponse ToResponse(Character c) => new(c.Id,c.Name,c.Class,c.Level,c.Hp,c.AccountId);
    public async Task<CharacterResponse?> Create(int accountId, CreateCharacterRequest request)
    {
        if (!await db.Accounts.AnyAsync(a => a.Id == accountId)) return null;
        var c = new Character { AccountId = accountId, Name = request.Name, Class = request.Class };
        db.Characters.Add(c);
        await db.SaveChangesAsync();
        return ToResponse(c);
    }
    public async Task<CharacterResponse?> Update(int accountId, int id, UpdateCharacterRequest request)
    {
        var c = await db.Characters.SingleOrDefaultAsync(x => x.Id == id && x.AccountId == accountId);
        if (c is null) return null;
        c.Name = request.Name; c.Class = request.Class; c.Level = request.Level!.Value; c.Hp = request.Hp!.Value;
        await db.SaveChangesAsync();
        return ToResponse(c);
    }
}
