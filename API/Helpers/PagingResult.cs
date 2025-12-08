using System;

namespace API.Helpers;
/// <summary>
/// 返回结果分页+元数据
/// </summary>
public class PagingResult<T>
{
    public PaginationMetadata Metadata { get; set; } = default!;
    public List<T> Items { get; set; } = [];
}
public class PaginationMetadata
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set;}
    public int TotalCount { get; set;}
    public int TotalPages { get; set; }
}

