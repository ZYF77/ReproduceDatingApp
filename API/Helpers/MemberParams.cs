using System;

namespace API.Helpers;
/// <summary>
/// member请求,包含分页,筛选条件,排序
/// </summary>
public class MemberParams : PagingParams
{
    public string? Gender { get; set; }
    public string? CurrentMemberId { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 99;
    public string? OrderBy { get; set; } = "lastActive";
}
