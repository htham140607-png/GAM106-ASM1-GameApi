using GameApi.DTOs;
using GameApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace GameApi.Controllers;
[ApiController, Authorize, Route("api/characters")]
public class CharactersController(CharacterService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCharacterRequest request)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var accountId)) return Unauthorized();
        var result = await service.Create(accountId, request);
        return result is null ? Unauthorized() : StatusCode(201,result);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateCharacterRequest request)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var accountId)) return Unauthorized();
        var result = await service.Update(accountId,id,request);
        return result is null ? NotFound(new { message = "Không tìm thấy nhân vật của bạn" }) : Ok(result);
    }
}
