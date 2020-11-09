using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
    public class ProductModel : CommonModel
    {

        public string Menu1Stylecss { get; set; }
        public string Menu1sfvip { get; set; }
        public string Menu1comid { get; set; }
        public string sbTypes { get; set; }

        /// 标签
        /// </summary>
        public string txttag { get; set; }

        public string txtPatch { get; set; }
        public string txtSourceUrl { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string txtPicName { get; set; }
        public string sbpricType { get; set; } = "元/副";

        public string txtColor { get; set; }
        public string txtSum { get; set; }
        public string txtMin { get; set; }
        public string txtQiXian { get; set; }
        public string txtprice { get; set; } = "0";
        public string txthyzhekou { get; set; } = "0";
        public string txtvipzhekou { get; set; } = "0";
        public string image1 { get; set; } = "0";
        public string image2 { get; set; } = "0";
        public string image3 { get; set; } = "0";
        public string txtimgPatch { get; set; }
        public string txtimgPatch1 { get; set; }
        public string txtimgPatch1b { get; set; }
        public string txtimgPatch2 { get; set; }
        public string txtimgPatch2b { get; set; }
        public string sbWatermarkType { get; set; } = "0";
        public string sbArea { get; set; } = "2";
        public string sbFamilyName { get; set; } = "Courier New";

        public string txtPATH { get; set; }
        public string sbintromore { get; set; }
        public string txtGuige { get; set; }
        public string txtpicweb { get; set; } = "http://www.smxkf.jqw.com";
        public string status { get; set; } = "1";
    }
}
