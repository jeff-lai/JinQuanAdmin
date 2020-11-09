using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
    public class WholesaleModel : CommonModel
    {
        public string sbprovince { get; set; }
        public string sbcity { get; set; }
        public string sbcounty { get; set; }
        public string showType { get; set; }

        public string txtSourceUrl { get; set; }
        public string sbState { get; set; } = "China";

        public string txtTitle { get; set; }


    }
}
