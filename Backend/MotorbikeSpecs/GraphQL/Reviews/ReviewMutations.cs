using HotChocolate;
using HotChocolate.Types;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.Extensions;
using MotorbikeSpecs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MotorbikeSpecs.GraphQL.Reviews
{
    [ExtendObjectType(name: "Mutation")]
    public class ReviewMutations
    {
    [UseBraapDbContext]
        public async Task<Review> AddReviewAsync(AddReviewInput input,
        [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
        {
            var review = new Review
            {
                Content = input.Content,
                MotorbikeId = int.Parse(input.MotorbikeId),
                BraapUserId = int.Parse(input.BraapUserId),
                Modified = DateTime.Now,
                Created = DateTime.Now,
            };
            context.Reviews.Add(review);

            await context.SaveChangesAsync(cancellationToken);

            return review;
        }

        [UseBraapDbContext]
        public async Task<Review> EditReviewAsync(EditReviewInput input,
                [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
        {
            var review = await context.Reviews.FindAsync(int.Parse(input.ReviewId));
            review.Content = input.Content ?? review.Content;

            await context.SaveChangesAsync(cancellationToken);

            return review;
        }
    }
}
