﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.DbEntities
{
    public partial class User
    {
        [NotMapped]
        public int RoleId { get; set; }
    }
}
