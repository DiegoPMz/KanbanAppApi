namespace KanbanAppApi.Models;

public record GoogleTokenResponse(
    string access_token,
    //string expires_in,
    string refresh_token,
    string scope,
    string token_type,
    string id_token
);