using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Keyless]
public partial class VqbAccount
{
    [Column("organizationID")]
    public int OrganizationId { get; set; }

    [Column("name")]
    [StringLength(500)]
    public string Name { get; set; }

    [Column("Org_Active")]
    public bool? OrgActive { get; set; }

    [Column("qbAccountType")]
    [StringLength(100)]
    public string QbAccountType { get; set; }

    [Column("qbAccountNumber")]
    [StringLength(100)]
    public string QbAccountNumber { get; set; }

    [Column("qbAccountName")]
    [StringLength(100)]
    public string QbAccountName { get; set; }

    [Column("Acc_Active")]
    public bool? AccActive { get; set; }

    [Column("qbMonthYear")]
    public DateOnly? QbMonthYear { get; set; }

    [Column("qbBalanceAmount", TypeName = "decimal(15, 2)")]
    public decimal? QbBalanceAmount { get; set; }
}
