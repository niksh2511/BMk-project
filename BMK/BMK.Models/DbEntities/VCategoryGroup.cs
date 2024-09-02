using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Keyless]
public partial class VCategoryGroup
{
    [Column("groupTypesID")]
    public int? GroupTypesId { get; set; }

    [Column("groupTypeName")]
    [StringLength(100)]
    public string GroupTypeName { get; set; }

    [Column("userGroupsID")]
    public int? UserGroupsId { get; set; }

    [Column("groupName")]
    [StringLength(50)]
    [Unicode(false)]
    public string GroupName { get; set; }

    [Column("access")]
    public int? Access { get; set; }

    [Column("catgeoryId")]
    public int? CatgeoryId { get; set; }

    [Column("categoryName")]
    [StringLength(50)]
    [Unicode(false)]
    public string CategoryName { get; set; }
}
