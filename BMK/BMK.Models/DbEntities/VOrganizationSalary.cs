using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Keyless]
public partial class VOrganizationSalary
{
    [Column("OrganizationSalaryID")]
    public int OrganizationSalaryId { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string DepartmentName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string RoleName { get; set; }

    [Column("FTEName")]
    [StringLength(100)]
    [Unicode(false)]
    public string Ftename { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string LevelName { get; set; }

    public int? Base { get; set; }

    public int? Bonus { get; set; }

    public int? Benefit { get; set; }

    [Column("OrganizationID")]
    public int? OrganizationId { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifyBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifyDate { get; set; }
}
