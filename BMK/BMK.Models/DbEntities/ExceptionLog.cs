using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("exceptionLogs")]
public partial class ExceptionLog
{
    [Key]
    [Column("exceptionLogsID")]
    public int ExceptionLogsId { get; set; }

    [Column("usersID")]
    public int UsersId { get; set; }

    [Required]
    [Column("url")]
    [StringLength(200)]
    [Unicode(false)]
    public string Url { get; set; }

    [Column("requestMethod")]
    [StringLength(10)]
    [Unicode(false)]
    public string RequestMethod { get; set; }

    [Required]
    [Column("message")]
    public string Message { get; set; }

    [Required]
    [Column("exceptionType")]
    public string ExceptionType { get; set; }

    [Required]
    [Column("exceptionSource")]
    public string ExceptionSource { get; set; }

    [Required]
    [Column("stackTrace")]
    public string StackTrace { get; set; }

    [Required]
    [Column("innerException")]
    public string InnerException { get; set; }

    [Column("exceptionDate", TypeName = "datetime")]
    public DateTime ExceptionDate { get; set; }
}
