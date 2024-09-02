using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.ViewModels
{
    public class Response<T>
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; }

        public T Data { get; set; } 
    }
}
