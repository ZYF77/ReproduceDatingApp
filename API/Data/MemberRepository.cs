using System;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MemberRepository(AppDbContext context) : IMemberRepository
{
    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        return await context.Members.FindAsync(id);
    }

    public async Task<Member?> GetMemberForUpdate(string id)
    {
        //级联member关联的数据
        return await context.Members
            .Include(x => x.User)
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<PagingResult<Member>> GetMembersAsync(MemberParams memberParams)
    {
        var query = context.Members.AsQueryable();
        query = query.Where(m => m.Id != memberParams.CurrentMemberId);
        if (!string.IsNullOrEmpty(memberParams.Gender))
        {
            query = query.Where(m => m.Gender == memberParams.Gender);
        }
        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge - 1)); //最早出生日期
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge)); //最晚出生日期
        query = query.Where(m => m.DateOfBirth >= minDob && m.DateOfBirth <= maxDob);

        query = memberParams.OrderBy switch
        {
            "created" => query.OrderByDescending(m =>m.Created),
            _ => query.OrderByDescending(m => m.LastActive)
        };
        return await PaginationHelp.CreateAsync(query,memberParams.PageNumber,memberParams.PageSize);
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
    {
        return await context.Members
            .Where(m => m.Id == memberId)
            .SelectMany(x => x.Photos)
            .ToListAsync();
    }

    public void Update(Member member)
    {
        context.Members.Update(member);
    }
}
