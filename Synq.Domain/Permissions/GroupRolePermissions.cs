using Synq.Domain.Enums;

namespace Synq.Domain.Permissions;

public static class GroupRolePermissions
{
    public static GroupPermissions GetPermissions(GroupRole role) {
        return role switch
        {
            GroupRole.Owner => GroupPermissions.All,

            GroupRole.Admin => GroupPermissions.SendMessages | GroupPermissions.DeleteMessages | GroupPermissions.BanMembers | GroupPermissions.ManageRoles,

            GroupRole.Mod => GroupPermissions.SendMessages | GroupPermissions.DeleteMessages,

            GroupRole.Member => GroupPermissions.SendMessages,

            _ => GroupPermissions.None
        };
    }
}