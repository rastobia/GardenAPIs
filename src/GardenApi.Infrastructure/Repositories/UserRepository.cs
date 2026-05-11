using GardenApi.Domain.Interfaces;
using GardenApi.Domain.Models;
using GardenApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GardenApi.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(GardenDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

    public async Task<bool> EmailExistsAsync(string email) =>
        await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
}
