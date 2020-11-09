using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
    public class BaseModel
    {
        public string __EVENTTARGET { get; set; }

        public string __EVENTARGUMENT { get; set; }

        /// <summary>
        /// html
        /// </summary>
        public string __VIEWSTATE { get; set; }

        /// <summary>
        /// html
        /// </summary>
        public string __VIEWSTATEGENERATOR { get; set; }

        /// <summary>
        /// html
        /// </summary>
        public string __EVENTVALIDATION { get; set; }
    }
}
