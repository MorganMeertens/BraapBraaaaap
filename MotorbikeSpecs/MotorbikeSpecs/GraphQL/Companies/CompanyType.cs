using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.GraphQL.Motorbikes;
using MotorbikeSpecs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MotorbikeSpecs.GraphQL.Companies
{
    public class CompanyType : ObjectType<Company>
    {
        protected override void Configure(IObjectTypeDescriptor<Company> descriptor)
        {
            descriptor.Field(s => s.Id).Type<NonNullType<IdType>>();
            descriptor.Field(s => s.CompanyName).Type<NonNullType<StringType>>();
            descriptor.Field(s => s.WebURL).Type<NonNullType<StringType>>();
            descriptor.Field(s => s.CountryOfOrigin).Type<NonNullType<StringType>>();

            descriptor
                .Field(s => s.Motorbikes)
                .ResolveWith<Resolvers>(r => r.GetMotorbikesByCompany(default!, default!, default))
                .UseDbContext<BraapDbContext>()
                .Type<NonNullType<ListType<NonNullType<MotorbikeType>>>>();

        }

        private class Resolvers
        {
            public async Task<IEnumerable<Motorbike>> GetMotorbikesByCompany(Company company, [ScopedService] BraapDbContext context,
                CancellationToken cancellationToken)
            {
                return await context.Motorbikes.Where(c => c.CompanyId == company.Id).ToArrayAsync(cancellationToken);
            }

        }
    }
}
