using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("objectives")]
public partial class Objective
{
    [Key]
    [Column("objectiveID")]
    public int ObjectiveId { get; set; }

    [Column("usersID")]
    public int? UsersId { get; set; }

    [Column("description")]
    [StringLength(250)]
    public string Description { get; set; }

    [Column("notes")]
    public string Notes { get; set; }

    [Column("deadline")]
    public DateOnly? Deadline { get; set; }

    [Column("completion")]
    public int? Completion { get; set; }

    [Column("closedDate")]
    public DateOnly? ClosedDate { get; set; }

    [Column("status")]
    public int? Status { get; set; }

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

    [ForeignKey("CreatedBy")]
    [InverseProperty("ObjectiveCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("ObjectiveModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [InverseProperty("Objective")]
    public virtual ICollection<ObjectiveComment> ObjectiveComments { get; set; } = new List<ObjectiveComment>();

    [ForeignKey("Status")]
    [InverseProperty("Objectives")]
    public virtual AppObject StatusNavigation { get; set; }

    [ForeignKey("UsersId")]
    [InverseProperty("ObjectiveUsers")]
    public virtual User Users { get; set; }
}
