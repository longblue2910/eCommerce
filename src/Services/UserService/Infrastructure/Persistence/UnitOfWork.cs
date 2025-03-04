using Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private readonly AppDbContext _context = context;


    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

}