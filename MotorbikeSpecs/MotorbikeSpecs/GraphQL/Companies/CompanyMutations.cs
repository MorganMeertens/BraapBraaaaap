using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.Extensions;
using MotorbikeSpecs.Model;

namespace MotorbikeSpecs.GraphQL.Companies
{
    [ExtendObjectType(name: "Mutation")]
    public class CompanyMutations
    {

            [UseBraapDbContext]
            public async Task<Company> AddCompanyAsync(AddCompanyInput input,
            [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
            {
                var company = new Company
                {
                    CompanyName = input.CompanyName,
                    WebURL = input.WebURL,
                    CountryOfOrigin = input.CountryOfOrigin,
                };

                context.Companies.Add(company);
                await context.SaveChangesAsync(cancellationToken);

                return company;
            }

            [UseBraapDbContext]
            public async Task<Company> EditCompanyAsync(EditCompanyInput input,
                    [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
            {
                var company = await context.Companies.FindAsync(int.Parse(input.CompanyId));

                company.CompanyName = input.CompanyName ?? company.CompanyName;
                company.WebURL = input.WebURL ?? company.WebURL;
                company.CountryOfOrigin = input.CountryOfOrigin ?? company.CountryOfOrigin;

                await context.SaveChangesAsync(cancellationToken);

                return company;
            }
        }
    }
