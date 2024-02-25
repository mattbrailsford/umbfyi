using NPoco;
using System.Globalization;
using System.Reflection;

namespace Umb.Fyi.Hub.Mappers
{
    internal class SqliteDataTypeMapper : DefaultMapper
    {
        public override Func<object, object> GetToDbConverter(Type destType, MemberInfo sourceMemberInfo)
        {
            Type memberInfoType = sourceMemberInfo.GetMemberInfoType();
            if (!(sourceMemberInfo != null))
            {
                return null;
            }

            return GetToDbConverter(destType, memberInfoType);
        }

        public Func<object, object> GetToDbConverter(Type destType, Type sourceType)
        {
            if (sourceType == typeof(Guid))
            {
                return (value) =>
                {
                    return value.ToString().ToLowerInvariant();
                };
            }

            if (sourceType == typeof(Guid?))
            {
                return (value) =>
                {
                    return value?.ToString().ToLowerInvariant();
                };
            }

            return null;
        }

        public override Func<object, object> GetFromDbConverter(Type destType, Type sourceType)
        {
            if (destType == typeof(Guid))
            {
                return (value) =>
                {
                    var result = Guid.Parse($"{value}");
                    return result;
                };
            }

            if (destType == typeof(Guid?))
            {
                return (value) =>
                {
                    if (Guid.TryParse($"{value}", out Guid result))
                    {
                        return result;
                    }

                    return default(Guid?);
                };
            }

            if (destType == typeof(decimal))
            {
                return value =>
                {
                    var result = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                    return result;
                };
            }

            if (destType == typeof(decimal?))
            {
                return value =>
                {
                    var result = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                    return result;
                };
            }

            return base.GetFromDbConverter(destType, sourceType);
        }
    }
}
