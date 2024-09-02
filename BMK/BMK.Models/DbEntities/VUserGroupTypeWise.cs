using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Keyless]
public partial class VUserGroupTypeWise
{
    [Column("groupTypesID")]
    public int? GroupTypesId { get; set; }

    [Column("groupName")]
    [StringLength(200)]
    public string GroupName { get; set; }

    [Column("userGroupsID")]
    public int UserGroupsId { get; set; }
}
