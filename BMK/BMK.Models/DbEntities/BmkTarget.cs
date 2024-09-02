using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("bmkTargets")]
public partial class BmkTarget
{
    [Key]
    [Column("bmkTargetID")]
    public int BmkTargetId { get; set; }

    [Required]
    [Column("target")]
    [StringLength(500)]
    [Unicode(false)]
    public string Target { get; set; }

    [Column("value")]
    public int? Value { get; set; }

    [Column("active")]
    public bool? Active { get; set; }

    [Column("createdBy")]
    public int? CreatedBy { get; set; }

    [Column("createdDate", TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column("modifiedBy")]
    public int? ModifiedBy { get; set; }

    [Column("modifiedDate", TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }
}
