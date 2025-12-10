using System;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task<bool> Complete()
    {
        try
        {
            return await context.SaveChangesAsync() > 0;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("An error occured while saving changes",ex);
        }
    }

    public bool HasChanged()
    {
        return context.ChangeTracker.HasChanges();
    }
}
