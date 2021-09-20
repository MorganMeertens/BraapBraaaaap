using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotorbikeSpecs.GraphQL.BraapUsers
{
    public record AddBraapUserInput(
        string UserName,
        string GitHub,
        string? ImageURI);
}
