namespace GameApi.DTOs;
public record AccountResponse(int Id, string Username);
public record LoginResponse(string Token, string Username);
public record CharacterResponse(int Id, string Name, string Class, int Level, int Hp, int AccountId);
