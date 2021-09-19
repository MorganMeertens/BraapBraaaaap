using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotorbikeSpecs.GraphQL.Users
{
    public record AddUserInput(
        string UserName,
        string GitHub,
        string? ImageURI);
}
