using System;
using System.Collections.Generic;
using System.Text;

namespace CUnity.Shared.Models
{
    public class ListConfiguration :BaseEntity
    {

        public int UserId { get; set; }

        public string ListType { get; set; }

        public string ListFrequency { get; set; }

        public string FrequencyDetail { get; set; }

        public DateTime CreatedDate { get; set; }

       
    }
  
    }
