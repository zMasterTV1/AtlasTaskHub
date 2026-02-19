using System.Security.Claims;

namespace Atlas.Api;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext ctx)
    {
        var id = ctx.User.FindFirstValue("uid") ?? ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null) throw new InvalidOperationException("Missing user id.");
        return Guid.Parse(id);
    }
}
