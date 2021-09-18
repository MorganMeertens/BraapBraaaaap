using System.Reflection;
using MarvelCinematicUniverse_DB.Data;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using MotorbikeSpecs.Data;

namespace MarvelCinematicUniverse_DB.Extensions
{
    public class UseBraapDbContextAttribute : ObjectFieldDescriptorAttribute
    {
        public override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
        {
            descriptor.UseDbContext<BraapDbContext>();
        }
    }
}
