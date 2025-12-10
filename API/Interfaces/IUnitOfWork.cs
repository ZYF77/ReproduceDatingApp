using System;

namespace API.Interfaces;

public interface IUnitOfWork
{
    Task<bool> Complete();
    bool HasChanged();
}
