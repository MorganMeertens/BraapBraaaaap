using HotChocolate;
using HotChocolate.Types;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.Extensions;
using MotorbikeSpecs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotorbikeSpecs.GraphQL.Reviews
{
    [ExtendObjectType(name: "Query")]
    public class ReviewQueries
    {
        [UseBraapDbContext]
        [UsePaging]
        public IQueryable<Review> GetReviewsForMotorbikeId(int Id, [ScopedService] BraapDbContext context)
        {
            return context.Reviews.Where(c => c.MotorbikeId == Id);
        }


    }
}
