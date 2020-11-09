using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
    public class LoginModel: BaseModel
    {
        public LoginModel()
        {
            __EVENTTARGET = "btsure";
        }

        public string txtsaveimg { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string txtLoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string txtLoginpas { get; set; }

        public string txtBar { get; set; }

    }
}
