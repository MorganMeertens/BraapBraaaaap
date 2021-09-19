using System.Reflection;
using MotorbikeSpecs.Data;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace MotorbikeSpecs.Extensions
{
    public class UseBraapDbContextAttribute : ObjectFieldDescriptorAttribute
    {
        public override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
        {
            descriptor.UseDbContext<BraapDbContext>();
        }
    }
}
