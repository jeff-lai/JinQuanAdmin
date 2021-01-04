using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
    public class LinkAccount : AccountBase
    {
        public List<LinkModel> LinkUrl { get; set; } = new List<LinkModel>();
        public bool? IsCopy { get; set; }
        public bool? IsApp { get; set; }
    }

    public class LinkModel
    {
        public LinkModel(string title,string link)
        {
            this.Title = title.Trim();
            this.Url = link.Trim();
        }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
