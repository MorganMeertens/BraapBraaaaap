
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.Extensions;
using MotorbikeSpecs.Model;

namespace MotorbikeSpecs.GraphQL.BraapUsers
{
    [ExtendObjectType(name: "Mutation")]
    public class BraapUserMutations
    {
        [UseBraapDbContext]
        public async Task<BraapUser> AddBraapUserAsync(AddBraapUserInput input,
        [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
        {
            var braapuser = new BraapUser
            {
                UserName = input.UserName,
                GitHub = input.GitHub,
                ImageURI = input.ImageURI,
            };

            context.BraapUsers.Add(braapuser);
            await context.SaveChangesAsync(cancellationToken);

            return braapuser;
        }

        [UseBraapDbContext]
        public async Task<BraapUser> EditBraapUserAsync(EditBraapUserInput input,
                [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
        {
            var braapuser = await context.BraapUsers.FindAsync(int.Parse(input.BraapUserId));

            braapuser.UserName = input.UserName ?? braapuser.UserName;
            braapuser.GitHub = input.GitHub ?? braapuser.GitHub;
            braapuser.ImageURI = input.ImageURI ?? braapuser.ImageURI;

            await context.SaveChangesAsync(cancellationToken);

            return braapuser;
        }
    }
}
