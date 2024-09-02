using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("psaInput")]
public partial class PsaInput
{
    [Key]
    [Column("PSAId")]
    public int Psaid { get; set; }

    public int? OrganizationId { get; set; }

    public int? Year { get; set; }

    public int? Month { get; set; }

    [Column("AGP")]
    public string Agp { get; set; }

    [Column("EHR")]
    public string Ehr { get; set; }

    public string BillableUtilization { get; set; }

    public string AgreementInvoices { get; set; }

    public string StandardInvoices { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("PsaInputs")]
    public virtual Organization Organization { get; set; }
}
