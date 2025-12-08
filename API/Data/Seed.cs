using System;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager, ILogger<Seed> logger)
    {
        //如果已经有用户，不做任何操作
        if (await userManager.Users.AnyAsync()) return;
        //读取json文件，并反序列化类集合
        var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var members = System.Text.Json.JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

        if (members == null)
        {
            logger.LogWarning("No members in seed data");
            return;
        }

        //存入数据
        foreach (var member in members)
        {
            var user = new AppUser
            {
                Id = member.Id,
                Email = member.Email,
                UserName = member.Email,
                DisplayName = member.DisplayName,
                ImageUrl = member.ImageUrl,
                Member = new Member
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName,
                    Description = member.Description,
                    DateOfBirth = member.DateOfBirth,
                    ImageUrl = member.ImageUrl,
                    Gender = member.Gender,
                    City = member.City,
                    Country = member.Country,
                    Created = member.Created,
                    LastActive = member.LastActive
                }
            };

            user.Member.Photos.Add(new Photo
            {
                Url = member.ImageUrl!,
                MemberId = member.Id
            });

            var result = await userManager.CreateAsync(user, "Pa$$w0rd");
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    logger.LogWarning("Failed to create user: {ErrorDescription}", error.Description);
                }
                continue; 
            }
        }
        var admin = new AppUser
        {
            UserName = "admin@test.com",
            Email = "admin@test.com",
            DisplayName = "Admin"
        };

        var createAdminResult = await userManager.CreateAsync(admin, "Pa$$w0rd");
        if (!createAdminResult.Succeeded) 
        {
             foreach (var error in createAdminResult.Errors)
             {
                 logger.LogWarning("Failed to create admin user: {ErrorDescription}", error.Description);
             }
             return;
        }

        var resultAdmin = await userManager.AddToRolesAsync(admin,["Admin","Moderator"]);
        if (!resultAdmin.Succeeded)
        {
            logger.LogWarning("Failed to Add Admin role user: {ErrorDescription}",resultAdmin.Errors.First().Description);
        }
    }
}
