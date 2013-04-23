using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proSEOMVC.Models
{
    public class AnalyticsProfile
    {
        public string name { get; set; }
        public string id { get; set; }
        public string url { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

    }
}
