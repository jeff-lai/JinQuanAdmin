﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
    public class Account
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

        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 开始页数
        /// </summary>

        public int StartPaged { get; set; }
        /// <summary>
        /// 结束页数
        /// </summary>
        public int EndPaged { get; set; }

        /// <summary>
        /// 锚点
        /// </summary>
        public List<string> ReadAnchor { get; set; } = new List<string>();
        public List<string> WriteAnchor { get; set; } = new List<string>();




        /// <summary>
        /// 图片地址
        /// </summary>
        public List<string> ReadPicUrl { get; set; } = new List<string>();
        public List<string> WritePicUrl { get; set; } = new List<string>();


        public string Included { get; set; }
        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetAnchorContent(int index)
        {
            StringBuilder content = new StringBuilder();

            if (ReadPicUrl != null && ReadPicUrl.Any())
            {
                content.Append(ReadPicUrl[index % ReadPicUrl.Count]);
            }
            if (ReadAnchor != null && ReadAnchor.Any())
            {
                content.Append(ReadAnchor[index % ReadAnchor.Count]);
            }
            return content.ToString();
        }

        internal void SetNameAndPassword(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#公司#" + this.Company);
            sb.AppendLine($"#账号#{this.UserName}--{this.Password}--{this.StartPaged}--{this.EndPaged}");
            if (!string.IsNullOrEmpty(this.Phone))
            {
                sb.AppendLine($"#手机#{this.Phone}");
            }
            if (!string.IsNullOrEmpty(Included))
            {
                sb.AppendLine($"#收录情况#{this.Included}");
            }
            sb.AppendLine($"#瞄点#{string.Join("|", this.WriteAnchor)}");
            sb.AppendLine($"#图片链接#");
            this.WritePicUrl.ForEach(s => sb.AppendLine($"@图片开始@{s}@图片结束@"));
            sb.AppendLine("");
            return sb.ToString();
        }


    }


}
