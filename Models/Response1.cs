using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class ERSEndPoint
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string mac { get; set; }
        public string profileId { get; set; }
        public bool staticProfileAssignment { get; set; }
        public bool staticProfileAssignmentDefined { get; set; }
        public string groupId { get; set; }
        public bool staticGroupAssignment { get; set; }
        public bool staticGroupAssignmentDefined { get; set; }
        public string portalUser { get; set; }
        public string identityStore { get; set; }
        public string identityStoreId { get; set; }
    
    }

    public class Response1
    {
        public ERSEndPoint ERSEndPoint { get; set; }
    }
    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
        public string type { get; set; }
    }

    public class CustomAttributes
    {
        public object customAttributes { get; set; }
    }
}