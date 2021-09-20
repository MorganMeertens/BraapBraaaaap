using HotChocolate;
using HotChocolate.Types;
using MotorbikeSpecs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MotorbikeSpecs.Model;
using MotorbikeSpecs.Extensions;
using HotChocolate.Data;

namespace MotorbikeSpecs.GraphQL.BraapUsers
{
    [ExtendObjectType(name: "Query")]
    public class BraapUserQueries
    {
        [UseBraapDbContext]
        [UsePaging]
        public IQueryable<BraapUser> GetAllBraapUsers([ScopedService] BraapDbContext context)
        {
            return context.BraapUsers;
        }

        [UseBraapDbContext]
        public BraapUser GetBraapUserById(int id, [ScopedService] BraapDbContext context)
        {
            return context.BraapUsers.Find(id);
        }
    }
}
