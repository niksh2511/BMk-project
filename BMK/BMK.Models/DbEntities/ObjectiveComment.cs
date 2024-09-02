using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("objectiveComments")]
public partial class ObjectiveComment
{
    [Key]
    [Column("objectiveCommentsID")]
    public int ObjectiveCommentsId { get; set; }

    [Column("commentText")]
    public string CommentText { get; set; }

    [Column("objectiveID")]
    public int? ObjectiveId { get; set; }

    [Column("commentByUserID")]
    public int? CommentByUserId { get; set; }

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

    [ForeignKey("CommentByUserId")]
    [InverseProperty("ObjectiveCommentCommentByUsers")]
    public virtual User CommentByUser { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("ObjectiveCommentCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("ObjectiveCommentModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("ObjectiveId")]
    [InverseProperty("ObjectiveComments")]
    public virtual Objective Objective { get; set; }
}
