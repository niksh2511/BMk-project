using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("organization")]
public partial class Organization
{
    [Key]
    [Column("organizationID")]
    public int OrganizationId { get; set; }

    [Column("name")]
    [StringLength(500)]
    public string Name { get; set; }

    [Column("website")]
    [StringLength(500)]
    public string Website { get; set; }

    [Required]
    [Column("address")]
    [StringLength(500)]
    public string Address { get; set; }

    [Required]
    [Column("city")]
    [StringLength(500)]
    public string City { get; set; }

    [Column("statesID")]
    public int? StatesId { get; set; }

    [Required]
    [Column("phone")]
    [StringLength(100)]
    public string Phone { get; set; }

    [Column("staffSize")]
    public int StaffSize { get; set; }

    [Column("annualRevenue")]
    public long AnnualRevenue { get; set; }

    [Column("excludeFromAverages")]
    public bool? ExcludeFromAverages { get; set; }

    [Column("psa")]
    public int? Psa { get; set; }

    [Column("rmm")]
    public int? Rmm { get; set; }

    [Column("otherTools")]
    [StringLength(500)]
    public string OtherTools { get; set; }

    [Column("targetNetIncome")]
    public int? TargetNetIncome { get; set; }

    [Column("targetAGP")]
    public int? TargetAgp { get; set; }

    [Column("targetEHR")]
    public int? TargetEhr { get; set; }

    [Column("salesPayrollTracking")]
    public int? SalesPayrollTracking { get; set; }

    [Column("adminPayrollTracking")]
    public int? AdminPayrollTracking { get; set; }

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

    [Column("countryCode")]
    [StringLength(5)]
    [Unicode(false)]
    public string CountryCode { get; set; }

    [Column("zipcode")]
    public int? Zipcode { get; set; }

    [InverseProperty("Organization")]
    public virtual ICollection<BmkTargetReport> BmkTargetReports { get; set; } = new List<BmkTargetReport>();

    [ForeignKey("CreatedBy")]
    [InverseProperty("OrganizationCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("OrganizationModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [InverseProperty("Organization")]
    public virtual ICollection<OrganizationPortalSetting> OrganizationPortalSettings { get; set; } = new List<OrganizationPortalSetting>();

    [InverseProperty("Organization")]
    public virtual ICollection<OrganizationSalary> OrganizationSalaries { get; set; } = new List<OrganizationSalary>();

    [InverseProperty("Organization")]
    public virtual ICollection<PsaInput> PsaInputs { get; set; } = new List<PsaInput>();

    [InverseProperty("Organization")]
    public virtual ICollection<QbMapMonthlyFinRecord> QbMapMonthlyFinRecords { get; set; } = new List<QbMapMonthlyFinRecord>();

    [InverseProperty("Organization")]
    public virtual ICollection<QbOrgAccountList> QbOrgAccountLists { get; set; } = new List<QbOrgAccountList>();

    [InverseProperty("Organization")]
    public virtual ICollection<QbProcessLog> QbProcessLogs { get; set; } = new List<QbProcessLog>();

    [InverseProperty("Organization")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
