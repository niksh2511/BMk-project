using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Keyless]
public partial class Vuser
{
    [Column("usersID")]
    public int UsersId { get; set; }

    [Column("firstname")]
    [StringLength(500)]
    public string Firstname { get; set; }

    [Column("lastName")]
    [StringLength(500)]
    public string LastName { get; set; }

    [Required]
    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [Column("mobilePhone")]
    [StringLength(20)]
    public string MobilePhone { get; set; }

    public bool IsUserOtpRequired { get; set; }

    public string ProfilePicture { get; set; }

    [Column("isO365user")]
    public bool? IsO365user { get; set; }

    [Column("isVerified")]
    public bool? IsVerified { get; set; }

    [Column("credential")]
    [StringLength(1000)]
    public string Credential { get; set; }

    [Column("salt")]
    [StringLength(128)]
    public string Salt { get; set; }

    [Required]
    [Column("countryCode")]
    [StringLength(5)]
    [Unicode(false)]
    public string CountryCode { get; set; }

    [Column("organizationID")]
    public int? OrganizationId { get; set; }

    [Column("name")]
    [StringLength(500)]
    public string Name { get; set; }

    [Column("roleMasterID")]
    public int? RoleMasterId { get; set; }

    [Column("roleName")]
    [StringLength(100)]
    public string RoleName { get; set; }

    [Column("active")]
    public bool? Active { get; set; }

    [Required]
    [Column("city")]
    [StringLength(500)]
    public string City { get; set; }

    [Required]
    [Column("stateName")]
    [StringLength(100)]
    [Unicode(false)]
    public string StateName { get; set; }
}
