using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.Extensions;
using MotorbikeSpecs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MotorbikeSpecs.GraphQL.Reviews
{
    [ExtendObjectType(name: "Mutation")]
    public class ReviewMutations
    {
        [UseBraapDbContext]
        [Authorize]
        public async Task<Review> AddReviewAsync(AddReviewInput input, ClaimsPrincipal claimsPrincipal,
        [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
        {
            var braapuserIdStr = claimsPrincipal.Claims.First(c => c.Type == "braapuserId").Value;
            var review = new Review
            {
                Content = input.Content,
                MotorbikeId = int.Parse(input.MotorbikeId),
                BraapUserId = int.Parse(braapuserIdStr),
                Modified = DateTime.Now,
                Created = DateTime.Now,
            };
            context.Reviews.Add(review);

            await context.SaveChangesAsync(cancellationToken);

            return review;
        }

        [UseBraapDbContext]
        [Authorize]
        public async Task<Review> EditReviewAsync(EditReviewInput input, ClaimsPrincipal claimsPrincipal,
                [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
        {

            var braapuserIdStr = claimsPrincipal.Claims.First(c => c.Type == "braapuserId").Value;
            var review = await context.Reviews.FindAsync(int.Parse(input.ReviewId));

            if (review.BraapUserId != int.Parse(braapuserIdStr))
            {
                throw new GraphQLRequestException(ErrorBuilder.New()
                    .SetMessage("Not owned by BraapUser")
                    .SetCode("AUTH_NOT_AUTHORIZED")
                    .Build());
            }

            review.Content = input.Content ?? review.Content;
            review.Modified = DateTime.Now;

            await context.SaveChangesAsync(cancellationToken);

            return review;
        }
    }
}
