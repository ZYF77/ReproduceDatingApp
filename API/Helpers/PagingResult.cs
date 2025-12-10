using System;
using Microsoft.EntityFrameworkCore;

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
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class PaginationHelp
{
    /// <summary>
    /// 分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query">表达式树</param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static async Task<PagingResult<T>> CreateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
    {
        var count = await query.CountAsync();//获取数量要在分页前面执行，否则会出错
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var metadata = new PaginationMetadata
        {
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize),
        };

        return new PagingResult<T>
        {
            Metadata = metadata,
            Items = items
        };

    }
}

