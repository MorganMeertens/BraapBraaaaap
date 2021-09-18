using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotorbikeSpecs.GraphQL.Companies
{
    public record EditCompanyInput
    (
        string CompanyId,
        string CompanyName,
        string WebURL,
        string? LogoURL);
}
