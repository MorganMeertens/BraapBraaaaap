using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using MotorbikeSpecs.Extensions;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotorbikeSpecs.GraphQL.Companies
{
    [ExtendObjectType(name: "Query")]
    public class CompanyQueries
    {
        [UseBraapDbContext]
        [UsePaging]
        public IQueryable<Company> GetAllCompanies([ScopedService] BraapDbContext context)
        {
            return context.Companies;
        }

        [UseBraapDbContext]
        public Company GetCompanyById(int id, [ScopedService] BraapDbContext context)
        {
            return context.Companies.Find(id);
        }
    }
}
