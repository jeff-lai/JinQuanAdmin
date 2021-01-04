using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
    public class AccountBase
    {
        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }


        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }


        internal void SetNameAndPassword(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

    }
}
