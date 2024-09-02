using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("organizationSalary")]
public partial class OrganizationSalary
{
    [Key]
    [Column("OrganizationSalaryID")]
    public int OrganizationSalaryId { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? Department { get; set; }

    public int? Role { get; set; }

    [Column("FTE")]
    public int? Fte { get; set; }

    public int? Level { get; set; }

    public int? Base { get; set; }

    public int? Bonus { get; set; }

    public int? Benefit { get; set; }

    [Column("OrganizationID")]
    public int? OrganizationId { get; set; }

    public bool? Active { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifyBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifyDate { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("OrganizationSalaries")]
    public virtual Organization Organization { get; set; }
}
