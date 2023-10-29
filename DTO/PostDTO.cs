using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tarea3.DTO
{
    public class PostDTO
    {
         public int id { get; set; }
        public string? title { get; set; }
        public string? body { get; set; }
        public int userId { get; set; }
    }
}