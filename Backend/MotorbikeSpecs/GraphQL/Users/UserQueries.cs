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

namespace MotorbikeSpecs.GraphQL.Users
{
    [ExtendObjectType(name: "Query")]
    public class UserQueries
    {
        [UseBraapDbContext]
        [UsePaging]
        public IQueryable<User> GetAllUsers([ScopedService] BraapDbContext context)
        {
            return context.Users;
        }

        [UseBraapDbContext]
        public User GetUserById(int id, [ScopedService] BraapDbContext context)
        {
            return context.Users.Find(id);
        }
    }
}
