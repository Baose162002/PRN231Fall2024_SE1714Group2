﻿using BusinessObject;
using BusinessObject.DTO.Response;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly FlowerShopContext flowerShopContext;
        private readonly FlowerShopContext _context;

        public CompanyRepository()
        {
            this.flowerShopContext ??= new FlowerShopContext();
            _context = new FlowerShopContext();
        }
        public async Task<bool> AddNew(Company company)
        {
            await flowerShopContext.Companies.AddAsync(company);
            var result = await flowerShopContext.SaveChangesAsync() > 0;
            return result;
        }

        public async Task<Company> GetCompanyByID(int id)
        {
            var company = await flowerShopContext.Companies.Include(x => x.Batches).FirstOrDefaultAsync(x => x.CompanyId == id);
            return company;
        }

        public async Task<List<Company>> GetCompanies()
        {
            var companies = await flowerShopContext.Companies.Include( x => x.Batches).ToListAsync();
            return companies;
        }

        public async Task Update(int id, Company updateCompany)
        {
            var company = await GetCompanyByID(id);
            if (company != null)
            {
                company.CompanyDescription = updateCompany.CompanyDescription;
                company.CompanyName = updateCompany.CompanyName;
                company.CompanyAddress = updateCompany.CompanyAddress;
                company.TaxNumber = updateCompany.TaxNumber;
                company.PostalCode = updateCompany.PostalCode;
                company.City = updateCompany.City;
                flowerShopContext.Companies.Update(company);
                await flowerShopContext.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            var company = await GetCompanyByID(id);
            if (company != null)
            {
                flowerShopContext.Companies.Remove(company);
                await flowerShopContext.SaveChangesAsync();
            }
        }
        
        public async Task<Company> GetCompanyByIdUser(int id)
        {
            var user = await flowerShopContext.Companies.Include(x => x.Batches).FirstOrDefaultAsync(x => x.UserId == id);
            return user;
        }


        public async Task<bool> UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
