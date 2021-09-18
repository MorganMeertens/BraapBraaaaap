using HotChocolate;
using HotChocolate.Types;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.GraphQL.Motorbikes;
using MotorbikeSpecs.GraphQL.Users;
using MotorbikeSpecs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MotorbikeSpecs.GraphQL.Reviews
{
    public class ReviewType : ObjectType<Review>
    {

        protected override void Configure(IObjectTypeDescriptor<Review> descriptor)
        {
            descriptor.Field(s => s.Id).Type<NonNullType<IdType>>();
            descriptor.Field(s => s.Content).Type<NonNullType<StringType>>();

            descriptor
                .Field(s => s.Motorbike)
                .ResolveWith<Resolvers>(r => r.GetMotorbikeByReview(default!, default!, default))
                .UseDbContext<BraapDbContext>()
                .Type<NonNullType<MotorbikeType>>();

            descriptor
                .Field(s => s.User)
                .ResolveWith<Resolvers>(r => r.GetUserByReview(default!, default!, default))
                .UseDbContext<BraapDbContext>()
                .Type<NonNullType<UserType>>();

            descriptor.Field(p => p.Modified).Type<NonNullType<DateTimeType>>();
            descriptor.Field(p => p.Created).Type<NonNullType<DateTimeType>>();

        }

        private class Resolvers
        {
            public async Task<Motorbike> GetMotorbikeByReview(Review review, [ScopedService] BraapDbContext context,
                CancellationToken cancellationToken)
            {
                return await context.Motorbikes.FindAsync(new object[] { review.MotorbikeId }, cancellationToken);
            }

            public async Task<User> GetUserByReview(Review review, [ScopedService] BraapDbContext context,
                CancellationToken cancellationToken)
            {
                return await context.Users.FindAsync(new object[] { review.UserId }, cancellationToken);
            }
        }

    }
}
