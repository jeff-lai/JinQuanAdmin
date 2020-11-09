using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
   /// <summary>
   /// 文章
   /// </summary>
    public class Article
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        public string Content { get; set; }
    }
}
