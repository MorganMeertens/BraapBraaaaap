using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.GraphQL.Companies;
using MotorbikeSpecs.GraphQL.Reviews;
using MotorbikeSpecs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MotorbikeSpecs.GraphQL.Motorbikes
{
    public class MotorbikeType : ObjectType<Motorbike>
    {
        protected override void Configure(IObjectTypeDescriptor<Motorbike> descriptor)
        {
            descriptor.Field(p => p.Id).Type<NonNullType<IdType>>();
            descriptor.Field(p => p.Make).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.Model).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.Category).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.Year).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.ImageURL).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.YouTubeReviewLink).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.YouTubeThumbnailURL).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.EngineType).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.Power).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.Torque).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.Displacement).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.Compression).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.BoreXStroke).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.FuelConsumption).Type<NonNullType<StringType>>();

            descriptor
                .Field(p => p.Company)
                .ResolveWith<Resolvers>(r => r.GetCompanyByMotorbike(default!, default!, default))
                .UseDbContext<BraapDbContext>()
                .Type<NonNullType<CompanyType>>();

            descriptor
                .Field(p => p.Reviews)
                .ResolveWith<Resolvers>(r => r.GetReviewsByMotorbike(default!, default!, default))
                .UseDbContext<BraapDbContext>()
                .Type<NonNullType<ListType<NonNullType<ReviewType>>>>();

            descriptor.Field(p => p.Modified).Type<NonNullType<DateTimeType>>();
            descriptor.Field(p => p.Created).Type<NonNullType<DateTimeType>>();

        }


        private class Resolvers
        {
            public async Task<Company> GetCompanyByMotorbike(Motorbike motorbike, [ScopedService] BraapDbContext context,
                CancellationToken cancellationToken)
            {
                return await context.Companies.FindAsync(new object[] { motorbike.CompanyId }, cancellationToken);
            }

            public async Task<IEnumerable<Review>> GetReviewsByMotorbike(Motorbike motorbike, [ScopedService] BraapDbContext context,
                CancellationToken cancellationToken)
            {
                return await context.Reviews.Where(c => c.MotorbikeId == motorbike.Id).ToArrayAsync(cancellationToken);
            }
        }
    }
}
