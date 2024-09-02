using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

public partial class RolePermission
{
    [NotMapped]
    public string moduleName { get; set; }
    [NotMapped]

    public bool isAdmin { get; set; }

}
public partial class RolePermissionModel
{
    public int RoleId { get; set; }

    public string Name { get; set; }
    public string Desc { get; set; }
    public List<RolePermission> rolePermissions { get; set; }

}