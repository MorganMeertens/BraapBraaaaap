
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using MotorbikeSpecs.Data;
using MotorbikeSpecs.Extensions;
using MotorbikeSpecs.Model;

namespace MotorbikeSpecs.GraphQL.Users
{
    [ExtendObjectType(name: "Mutation")]
    public class UserMutations
    {
        [UseBraapDbContext]
        public async Task<User> AddUserAsync(AddUserInput input,
        [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
        {
            var user = new User
            {
                UserName = input.UserName,
                GitHub = input.GitHub,
                ImageURI = input.ImageURI,
            };

            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);

            return user;
        }

        [UseBraapDbContext]
        public async Task<User> EditUserAsync(EditUserInput input,
                [ScopedService] BraapDbContext context, CancellationToken cancellationToken)
        {
            var student = await context.Users.FindAsync(int.Parse(input.UserId));

            student.UserName = input.UserName ?? student.UserName;
            student.GitHub = input.GitHub ?? student.GitHub;
            student.ImageURI = input.ImageURI ?? student.ImageURI;

            await context.SaveChangesAsync(cancellationToken);

            return student;
        }
    }
}
