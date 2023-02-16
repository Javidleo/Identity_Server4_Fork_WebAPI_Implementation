using Newtonsoft.Json;

namespace IdentityServer4.API.DTO;

public record LoginDTO(string UserName,string Password,bool RememberLogin,string ReturnUrl);