using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("emailTemplates")]
public partial class EmailTemplate
{
    [Key]
    [Column("emailTemplatesID")]
    public int EmailTemplatesId { get; set; }

    [Column("templateName")]
    [StringLength(100)]
    public string TemplateName { get; set; }

    [Column("templateType")]
    [StringLength(100)]
    public string TemplateType { get; set; }

    [Column("templateSubject")]
    [StringLength(100)]
    public string TemplateSubject { get; set; }

    [Column("templateBody")]
    public string TemplateBody { get; set; }

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
    [InverseProperty("EmailTemplateCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("EmailTemplateModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }
}
