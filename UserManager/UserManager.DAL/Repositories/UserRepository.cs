﻿using Microsoft.EntityFrameworkCore;
using UserManager.DAL.Context;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManagerDbContext _context;

    public UserRepository(UserManagerDbContext context)
    {
        _context = context;
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckEmailIsExistAsync(string email)
    {
        return await _context.Users.AnyAsync(e => e.Email == email);
    }

    public ValueTask<User?> GetUserByIdAsync(Guid id)
    {
        return _context.Users.FindAsync(id);
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        return _context.Users.FirstOrDefaultAsync(e => e.Email == email);
    }
}