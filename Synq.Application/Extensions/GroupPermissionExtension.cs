using Synq.Domain.Enums;

namespace Synq.Application.Extensions;

public static class GroupPermissionExtension
{
    public static bool HasPermission(this GroupPermissions permissions, GroupPermissions required)
    {
        return (permissions & required) == required;
    }
}