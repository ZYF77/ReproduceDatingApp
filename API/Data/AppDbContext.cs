using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

public class AppDbContext (DbContextOptions options): IdentityDbContext<AppUser> (options)
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<IdentityRole>()
            .HasData(
                new IdentityRole { Id = "member-id", Name = "Member", NormalizedName = "MEMBER", ConcurrencyStamp = "member-id-stamp" },
                new IdentityRole { Id = "moderator-id", Name = "Moderator", NormalizedName = "MODERATOR", ConcurrencyStamp = "moderator-id-stamp" },
                new IdentityRole { Id = "admin-id", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "admin-id-stamp" }
            );


        #region 日期类型格式转换
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        var nullabledateTimeConverter = new ValueConverter<DateTime?, DateTime?>( //datetime 和 datetime ? 有区别，需要分别处理
            v => v.HasValue ? v.Value.ToUniversalTime() : null,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
        );



        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullabledateTimeConverter);
                }
            }
        }

        #endregion
    }
}
