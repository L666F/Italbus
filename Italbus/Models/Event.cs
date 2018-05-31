using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Italbus.Models
{
    public class Event
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Text { get; set; }
        public string FileName { get; set; }

        /*public long lat { get; set; }
        public long lon { get; set; }*/
    }
}
