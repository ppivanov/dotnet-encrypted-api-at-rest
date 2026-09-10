using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace EncryptedDbAtRest.Server;

public class User : Instance
{
    [NotMapped]
    public string Username { get; set; }

    [NotMapped]
    public string Password { get; set; }

    public User() : base(null)
    {

    }

    public User(Tenant tenant) : base(tenant)
    {
    }
}

public static class UserController
{
    public static User Register(HttpContext httpContext, DbContext dbContext, [FromRoute] string tenant, [FromBody] UserRequest request)
    {

    }
}

public class UserRequest
{
    public required string Username { get; set; }

    public required string Password { get; set; }
}