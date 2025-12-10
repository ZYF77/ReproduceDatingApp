using System;
using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// 根据token，获取userID自定义扩展方法
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string GetMemberId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new Exception("Cannot find NameIdentifier claim");
    }
}
