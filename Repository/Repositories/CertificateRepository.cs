using Microsoft.EntityFrameworkCore;
using Repository.Basic;
using Repository.DBContext;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class CertificateRepository : GenericRepository<Certificate>
    {
        public CertificateRepository()
        {
        }

        public CertificateRepository(CourseraStyleLMSContext context) : base(context)
        {
        }

        public async Task<List<Certificate>> GetAllAsync()
        {
            return await _context.Certificates
                .Include(c => c.User)
                .Include(c => c.Course)
                .ToListAsync();
        }

        public async Task<Certificate> GetByIdAsync(int id)
        {
            return await _context.Certificates
                .Include(c => c.User)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.CertificateId == id);
        }

        public async Task<int> CreateAsync(Certificate entity)
        {
            _context.Certificates.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Certificate entity)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Certificates.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Certificate entity)
        {
            _context.Certificates.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
