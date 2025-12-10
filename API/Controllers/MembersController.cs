using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Authorize]
public class MembersController(IMemberRepository memberRepository,IUnitOfWork uow) : BaseAPIController
{
    /// <summary>
    /// 获取除自己外的所有member
    /// </summary>
    /// <param name="memberParams"></param>
    /// <returns>分页后的member</returns>
    [HttpGet]
    public async Task<ActionResult<PagingResult<Member>>> GetMembers(MemberParams memberParams)
    {
        memberParams.CurrentMemberId = User.GetMemberId();

        return Ok(await memberRepository.GetMembersAsync(memberParams));
    }
    /// <summary>
    /// 获取指定member
    /// </summary>
    /// <param name="id">memberId</param>
    /// <returns>member</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Member?>> GetMember(string id)
    {
        var user = await memberRepository.GetMemberByIdAsync(id);
        if(user == null) return NotFound();
        return Ok(user);
    }
    /// <summary>
    /// 获取指定用户的photo
    /// </summary>
    /// <param name="id">memberId</param>
    /// <returns>Photo</returns>
    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
    {
        return Ok(await memberRepository.GetPhotosForMemberAsync(id));
    }
    /// <summary>
    /// 更新member
    /// </summary>
    /// <param name="memberUpdateDto">member更新dto</param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
    {
        var memberId = User.GetMemberId();
        var member = await memberRepository.GetMemberForUpdate(memberId);
        if(member == null) return BadRequest("Could not find member to update");
        member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
        member.Description = memberUpdateDto.Description ?? member.Description;
        member.City = memberUpdateDto.City ?? member.City;
        member.Country = memberUpdateDto.Country ?? member.Country;
        member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;

        memberRepository.Update(member);

        if(await uow.Complete()) return NoContent();
        return BadRequest("Failed to update member");
    }
}
