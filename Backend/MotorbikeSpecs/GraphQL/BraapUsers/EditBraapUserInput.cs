using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotorbikeSpecs.GraphQL.BraapUsers
{
    public record EditBraapUserInput(
        string BraapUserId,
        string UserName,
        string GitHub,
        string? ImageURI);
}
