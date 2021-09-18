using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.GraphQL.Reviews;
using MotorbikeSpecs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MotorbikeSpecs.GraphQL.Users
{
    public class UserType: ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor.Field(s => s.Id).Type<NonNullType<IdType>>();
            descriptor.Field(s => s.UserName).Type<NonNullType<StringType>>();
            descriptor.Field(s => s.GitHub).Type<NonNullType<StringType>>();
            descriptor.Field(s => s.ImageURI).Type<NonNullType<StringType>>();

            descriptor
                .Field(s => s.Reviews)
                .ResolveWith<Resolvers>(r => r.GetReviewsByUser(default!, default!, default))
                .UseDbContext<BraapDbContext>()
                .Type<NonNullType<ListType<NonNullType<ReviewType>>>>();
        }

        private class Resolvers //This is where I need to add dataloaders? PrettySure
        {
            public async Task<IEnumerable<Review>> GetReviewsByUser(User user, [ScopedService] BraapDbContext context,
                CancellationToken cancellationToken)
            {
                return await context.Reviews.Where(c => c.UserId == user.Id).ToArrayAsync(cancellationToken);
            }
        }
    }
}
