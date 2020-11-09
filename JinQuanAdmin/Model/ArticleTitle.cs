using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
    public class ArticleTitle
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Id 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否包含
        /// </summary>
        public bool IsIncluded { get; set; }
    }
}
