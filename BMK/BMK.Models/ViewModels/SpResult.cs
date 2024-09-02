using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.ViewModels
{
    public class SpResult
    {
        [Key]
        public int Id { get; set; }
        public string Result { get; set; }
        public string Data { get; set; }
    }
}
