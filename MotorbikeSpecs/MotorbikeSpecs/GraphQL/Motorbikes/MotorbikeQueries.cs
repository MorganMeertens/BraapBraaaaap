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

namespace MotorbikeSpecs.GraphQL.Motorbikes
{
    [ExtendObjectType(name: "Query")]
    public class MotorbikeQueries
    {
        [UseBraapDbContext]
        [UsePaging]
        public IQueryable<Motorbike> GetAllMotorbikes([ScopedService] BraapDbContext context)
        {
            return context.Motorbikes.OrderBy(c => c.Created);
        }

        [UseBraapDbContext]
        public Motorbike GetMotorbikeById(int id, [ScopedService] BraapDbContext context)
        {
            return context.Motorbikes.Find(id);
        }
    }
}
