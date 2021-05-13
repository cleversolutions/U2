using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace U2.Models
{
    public class U2MFASetup
    {
        public string Email { get; set; }
        public string ApplicationName { get; set; }
        public string Secret { get; set; }
        public string QrCodeUri { get; set; }
    }
}