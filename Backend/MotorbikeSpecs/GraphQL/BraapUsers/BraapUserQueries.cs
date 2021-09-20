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
using HotChocolate.AspNetCore.Authorization;
using System.Security.Claims;

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

        [UseBraapDbContext]
        [Authorize]
        public BraapUser GetSelf(ClaimsPrincipal claimsPrincipal, [ScopedService] BraapDbContext context)
        {
            var braapuserIdStr = claimsPrincipal.Claims.First(c => c.Type == "braapuserId").Value;

            return context.BraapUsers.Find(int.Parse(braapuserIdStr));
        }

    }


}
