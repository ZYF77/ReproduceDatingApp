using System;
using API.DTOs;
using API.Entities;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Authorize]
public class MembersController : BaseAPIController
{
    [HttpGet]
    public async Task<ActionResult<Member>> GetMembers(MemberParams memberParams)
    {
        return Ok(new List<Member>());
    }
}
