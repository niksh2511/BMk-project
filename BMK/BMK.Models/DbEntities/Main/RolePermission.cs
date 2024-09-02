using RxWeb.Core.Annotations;
using RxWeb.Core.Data.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMK.Models.DbEntities.Main
{
    [Table("RolePermissions", Schema = "dbo")]
    public partial class RolePermission
    {
        #region RolePermissionId Annotations

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [System.ComponentModel.DataAnnotations.Key]
        #endregion RolePermissionId Annotations

        public int RolePermissionId { get; set; }

        #region RoleId Annotations

        [Range(1, int.MaxValue)]
        [Required]
        [RelationshipTableAttribue("Roles", "dbo", "", "RoleId")]
        #endregion RoleId Annotations

        public int RoleId { get; set; }

        #region ApplicationModuleId Annotations

        [Range(1, int.MaxValue)]
        [Required]
        [RelationshipTableAttribue("ApplicationModules", "dbo", "", "ApplicationModuleId")]
        #endregion ApplicationModuleId Annotations

        public int ApplicationModuleId { get; set; }


        public bool? CanView { get; set; }


        public bool? CanAdd { get; set; }


        public bool? CanEdit { get; set; }


        public bool? CanDelete { get; set; }

        #region PermissionPriority Annotations

        [Range(1, int.MaxValue)]
        [Required]
        #endregion PermissionPriority Annotations

        public int PermissionPriority { get; set; }

        #region ApplicationModule Annotations

        [ForeignKey(nameof(ApplicationModuleId))]
        [InverseProperty(nameof(Main.ApplicationModule.RolePermissions))]
        #endregion ApplicationModule Annotations

        public virtual ApplicationModule ApplicationModule { get; set; }

        #region Role Annotations

        [ForeignKey(nameof(RoleId))]
        [InverseProperty(nameof(Main.Role.RolePermissions))]
        #endregion Role Annotations

        public virtual Role Role { get; set; }


        public RolePermission()
        {
        }
    }
}