using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google;
using Google.Apis.Authentication;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Newtonsoft.Json;

namespace proSEOMVC.Models
{
    public class PlotPoint
    {
        public long DateTime { get; set; }
        public int Value { get;set; }
    }
    public class AnalyticsReport : AnalyticsProfile
    {

        public AnalyticsProfile profile { get; set; }
        public IList<IList<string>> JSON { get; set; }
        public string LineGraph { get; set; }
        public string PieGraph1 { get; set; }
        public int TotalVisits { get; set; }
        public int TotalPageViews { get; set; }
        public int maxNewVisitors { get; set; }
        public int maxReturningVisitors { get; set; }
        public float BounceRate { get; set; }
        public int TotalNewVisitors { get; set; }
        public int TotalReturningVisitors { get; set; }
        public List<int> newVisitors { get; set; }
        public List<int> retVisitors { get; set; }
        public List<int> totalVisitors { get; set; } 
        public List<string> keylist { get; set; }
        public List<int> daylist { get; set; }
        public List<string> reflist { get; set; }
        public List<string> pagelist { get; set; }
        public List<string> citylist { get; set; }
        public Dictionary<string, int> topRef { get { return (from source in reflist group source by source into g orderby g.Count() descending select new {g.Key, Count = g.Count() }).Take(5).ToDictionary(o => o.Key, o => o.Count); } }
        public Dictionary<string, int> topPage
        {
            get
            {

                return (from pages in pagelist
                        group pages by pages
                        into p where p.Key != null && !p.Key.Contains("Error") orderby p.Count() descending
                        select new {Key = p.Key, Count = p.Count()}).Take(7).ToDictionary(o => o.Key, o => o.Count);
            }
        }
        public Dictionary<string, int> topCity {
            get
            {return
                (from pages in citylist group pages by pages into g where g.Key != null orderby g.Count() descending select new { Key = g.Key, Count = g.Count() }).Take(5).ToDictionary(o => o.Key, o => o.Count);
            }
        }
        public Dictionary<string, int> topKeys
        {
            get
            {
                return (from pages in keylist
                        group pages by pages
                            into g
                            where !g.Key.Contains("(not set)") && !g.Key.Contains("(not provided)")
                            orderby g.Count() descending
                            select new { g.Key, Count = g.Count() }).Take(5).ToDictionary( o => o.Key, o => o.Count);
            }
        }

       public string jsondayList {get
       {
           Dictionary<long, int> splineGraph = new Dictionary<long, int>();
           HashSet<int> splineGraphz = new HashSet<int>();
           List<PlotPoint> graphPoints = new List<PlotPoint>();
           for (int i = 0; i < totalVisitors.Count; i++)
           {
               PlotPoint p = new PlotPoint();
               p.DateTime = daylist[i];
               p.Value = totalVisitors[i];
               graphPoints.Add(p);
              // splineGraph.Add(daylist[i],totalVisitors[i]);

           }

           IEnumerable<IGrouping<long, int>> query =
                   graphPoints.GroupBy(date => date.DateTime, date => date.Value).OrderBy(m => m.Key);
           List<PlotPoint> graphPoints2 = new List<PlotPoint>();
           foreach (var plotPoint in query)
           {
            PlotPoint pe = new PlotPoint();
               var date = DateTime.ParseExact(plotPoint.Key.ToString(), "yyyyMMdd", null);

               pe.DateTime = GetJavascriptTimestamp(date);
               pe.Value = plotPoint.Sum();
               graphPoints2.Add(pe);
               

                splineGraph.Add(GetJavascriptTimestamp(date), plotPoint.Sum());
              // splineGraphz.Add(plotPoint.Key, plotPoint.Sum());
           }
           return JsonConvert.SerializeObject(graphPoints2);

       }
       }

       public static long GetJavascriptTimestamp(DateTime input)
       {
          // TimeSpan span = new TimeSpan(DateTime.Parse("1/1/1970").Ticks);
          // DateTime time = input.Subtract(span);
           return (long)(input.Ticks / 10000);
       }

        public int googleRef { get { return (from gref in topRef where gref.Key.Contains("google") select gref.Value).First(); } }
        public int directRef { get { return (from gref in topRef where gref.Key.Contains("direct") select gref.Value).First(); } }
        public int returnCount { get; set; }
    }


}