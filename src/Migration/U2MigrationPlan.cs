using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Migrations;

namespace U2.Migration
{
    public class U2MigrationPlan : MigrationPlan
    {
        public U2MigrationPlan() : base("U2Migration")
        {
            From(string.Empty).To<U2Migration>("first-migration");
        }
    }
}