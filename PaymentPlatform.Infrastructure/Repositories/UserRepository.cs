using Microsoft.EntityFrameworkCore;
using PaymentPlatform.Application.Interfaces;
using PaymentPlatform.Domain.Entities;
using PaymentPlatform.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentPlatform.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public Task<User?> GetByEmailAsync(string email) =>
            _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        public Task<User?> GetByIdAsync(Guid id) =>
            _db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}
