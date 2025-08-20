using Repository.Models;
using Repository.Repositories;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class UserService : IUserService
    {
        private readonly UserRepository userRepository;

        public async Task<User> LoginAsync(string email, string password)
        {
            return await userRepository.LoginAsync(email, password);
        }

        public UserService()
        {
            userRepository = new UserRepository();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await userRepository.GetAllAsync();
        }

        public async Task<User> GetByIdAsync(int? id)
        {
            return await userRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                return await userRepository.RemoveAsync(user);
            }
            return false;
        }


        public async Task CreateAsync(User user)
        {
            await userRepository.CreateAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            await userRepository.UpdateAsync(user);
        }


        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            return await userRepository.GetUsersByRoleAsync(role);

        public async Task<User?> RegisterAsync(string fullName, string email, string password, string UserName)
        {
            var exists = await userRepository.GetByEmailAsync(email);
            if (exists != null) return null;

            var hashedPassword = HashPassword(password);

            var user = new User
            {
                Username = UserName,
                FullName = fullName,
                Email = email,
                PasswordHash = hashedPassword,
                RoleId = 3 // Default = Student
            };

            await userRepository.CreateAsync(user);
            return user;
        }

        public Task<User?> GetByIdAsync(int id) => userRepository.GetByIdAsync(id);

        public Task<User?> GetByEmailAsync(string email) => userRepository.GetByEmailAsync(email);

        private string HashPassword(string password)
        {
            //using var sha256 = SHA256.Create();
            //var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            //return Convert.ToBase64String(bytes);
            return password;

        }
    }
}
