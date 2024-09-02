using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Keyless]
public partial class VUserGroupsMember
{
    [Column("userGroupsID")]
    public int UserGroupsId { get; set; }

    [Column("groupName")]
    [StringLength(200)]
    public string GroupName { get; set; }

    [Column("groupDesc")]
    [StringLength(500)]
    public string GroupDesc { get; set; }

    [Column("groupTypesID")]
    public int? GroupTypesId { get; set; }

    [Column("groupTypeName")]
    [StringLength(100)]
    public string GroupTypeName { get; set; }

    [Column("usersID")]
    public int? UsersId { get; set; }
}
