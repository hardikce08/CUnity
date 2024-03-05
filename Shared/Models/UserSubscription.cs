using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CUnity.Shared.Models
{
    public class ClientSubscription : BaseEntity
    {
        public int ClientId { get; set; }
        [Required(ErrorMessage ="Please enter valid subscription key")]
        public string SubscriptionKey { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ProductName { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionEndDate { get; set; }
    }

    public class AllSubscriptionViewModel
    {
      
        public string SubscriptionKey { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ProductName { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionEndDate { get; set; }

        public string ClientName { get; set; }
    }
}
