using System.Reflection;

namespace OpenChat.Infrastructure.Extensions
{
    public static class MemberInfoExtensions
    {
        public static object GetValue(this MemberInfo memberInfo, object forObject)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(forObject)!;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(forObject)!;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}