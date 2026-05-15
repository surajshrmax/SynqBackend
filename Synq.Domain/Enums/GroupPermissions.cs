using System;
using System.Collections.Generic;
using System.Text;

namespace Synq.Domain.Enums;

[Flags]
public enum GroupPermissions : long
{
    None = 0,

    SendMessages = 1 << 0,
    DeleteMessages = 1 << 1,
    EditMessages = 1 << 2,
    BanMembers = 1 << 3,
    KickMembers = 1 << 4,
    AddMembers = 1 << 5,
    PinMessages = 1 << 6,
    ManageRoles = 1 << 7,
    ManageGroupInfo = 1 << 8,
    
    All = long.MaxValue
}