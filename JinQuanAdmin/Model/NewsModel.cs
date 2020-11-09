using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
    public class NewsModel : BaseModel
    {
        public string sbType { get; set; } = "0";
        public string txtTitle { get; set; }

        public string editor_trigger { get; set; }
        public string txtPATH { get; set; }
        public string showType { get; set; } = "0";
        public string save { get; set; } = "提交";
    }
}
