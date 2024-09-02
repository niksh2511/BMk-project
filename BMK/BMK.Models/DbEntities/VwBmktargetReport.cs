using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Keyless]
public partial class VwBmktargetReport
{
    [Column("organizationID")]
    public int OrganizationId { get; set; }

    public int Year { get; set; }

    [Required]
    [Column("month")]
    [StringLength(10)]
    [Unicode(false)]
    public string Month { get; set; }

    [Required]
    [Column("rptHead")]
    [StringLength(50)]
    public string RptHead { get; set; }

    [Required]
    [Column("rptLine")]
    [StringLength(50)]
    public string RptLine { get; set; }

    [Column("rptSeqs")]
    public int RptSeqs { get; set; }

    [Column("amount", TypeName = "decimal(20, 2)")]
    public decimal? Amount { get; set; }
}
