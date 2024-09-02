using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Keyless]
public partial class Vorganizaion
{
    [Column("organizationID")]
    public int OrganizationId { get; set; }

    [Column("name")]
    [StringLength(500)]
    public string Name { get; set; }

    [Required]
    [Column("website")]
    [StringLength(500)]
    public string Website { get; set; }

    [Column("users")]
    public int? Users { get; set; }

    [Column("active")]
    public bool? Active { get; set; }
}
