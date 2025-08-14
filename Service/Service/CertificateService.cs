using Repository.Models;
using Repository.Repositories;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class CertificateService : ICertificateService
    {
        private readonly CertificateRepository certificateRepository;

        public CertificateService()
        {
            certificateRepository = new CertificateRepository();

        }

        public async Task<List<Certificate>> GetAllAsync()
        {
            return await certificateRepository.GetAllAsync();
        }

        public async Task<Certificate> GetByIdAsync(int? id)
        {
            return await certificateRepository.GetByIdAsync(id ?? 0);
        }

        public async Task<bool> DeleteAsync(int? id)
        {
            var certificate = await GetByIdAsync(id);
            if (certificate != null)
            {
                return await certificateRepository.RemoveAsync(certificate);
            }
            return false;
        }
    }
}
