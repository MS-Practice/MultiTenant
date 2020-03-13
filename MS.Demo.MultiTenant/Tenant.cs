using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Demo.MultiTenant
{
    public class Tenant
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public Dictionary<string, object> Items { get; set; } = new Dictionary<string, object>();
    }
}
