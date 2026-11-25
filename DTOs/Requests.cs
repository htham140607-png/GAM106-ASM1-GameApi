using System.ComponentModel.DataAnnotations;
namespace GameApi.DTOs;
public class RegisterRequest
{
    [Required, StringLength(50, MinimumLength=3)] public string Username { get; set; } = "";
    [Required, StringLength(100, MinimumLength=6)] public string Password { get; set; } = "";
}
public class LoginRequest
{
    [Required] public string Username { get; set; } = "";
    [Required] public string Password { get; set; } = "";
}
public class CreateCharacterRequest
{
    [Required, StringLength(50, MinimumLength=2)] public string Name { get; set; } = "";
    [Required, StringLength(30, MinimumLength=1)] public string Class { get; set; } = "";
}
public class UpdateCharacterRequest : CreateCharacterRequest
{
    [Required, Range(1,100)] public int? Level { get; set; }
    [Required, Range(0,100000)] public int? Hp { get; set; }
}
