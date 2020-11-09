using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
    public class CommonModel : BaseModel
    {
        public string sbType { get; set; }
        public string sbBigCat { get; set; }
        public string sbMidCat { get; set; }
        public string sbSmallCat { get; set; }
        public string sbactioncat { get; set; }

        public string editor_trigger { get; set; }

        public string raqixian { get; set; } = "365";

        public string save { get; set; } = "提交";
    }
}
