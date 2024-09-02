using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Keyless]
public partial class VOrganizationForPeerTeamProfile
{
    [Column("organizationID")]
    public int OrganizationId { get; set; }

    [Column("name")]
    [StringLength(500)]
    public string Name { get; set; }

    [Column("staffSize")]
    public int? StaffSize { get; set; }

    [Column("psa")]
    [StringLength(100)]
    [Unicode(false)]
    public string Psa { get; set; }

    [Column("rmm")]
    [StringLength(100)]
    [Unicode(false)]
    public string Rmm { get; set; }

    [Column("otherTools")]
    [StringLength(500)]
    public string OtherTools { get; set; }
}
