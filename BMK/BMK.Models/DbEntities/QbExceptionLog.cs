using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("qbExceptionLogs")]
public partial class QbExceptionLog
{
    [Key]
    [Column("qbExceptionLogsID")]
    public int QbExceptionLogsId { get; set; }

    [Column("organizationID")]
    public int? OrganizationId { get; set; }

    [Column("usersID")]
    public int? UsersId { get; set; }

    [Column("errorProcedure")]
    [StringLength(100)]
    [Unicode(false)]
    public string ErrorProcedure { get; set; }

    [Column("errorLine")]
    public int? ErrorLine { get; set; }

    [Column("errorMessage")]
    [Unicode(false)]
    public string ErrorMessage { get; set; }

    [Column("errorDate", TypeName = "datetime")]
    public DateTime? ErrorDate { get; set; }

    [Column("jsonString")]
    public string JsonString { get; set; }
}
