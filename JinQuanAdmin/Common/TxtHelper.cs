using JinQuanAdmin.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UtfUnknown;

namespace JinQuanAdmin.Common
{
    public class TxtHelper
    {

        private const string PLine = "<p>&nbsp&nbsp{0}</p>";
        public string UserName { get; set; }
        public string Password { get; set; }

        private bool IsReadUserName = false;
        public List<Article> GetArticles(string fileName)
        {
            List<Article> articles = new List<Article>();

            var allLines = File.ReadAllLines(fileName);
            bool isContent = false;
            StringBuilder sb = new StringBuilder();
            Article article = new Article();
            foreach (var line in allLines)
            {

                if (!IsReadUserName && line.Contains("#账号#"))
                {
                    SetUserInfo(line);
                    continue;
                }
                if (line.Contains("#公司#"))
                {
                    isContent = false;
                    if (sb.Length > 0)
                    {
                        article.Content = sb.ToString();
                        sb.Clear();
                        articles.Add(article);
                    }
                    continue;
                }
                if (line.Contains("#标题#"))
                {
                    article = new Article();
                    article.Title = line.Replace("#标题#", "");
                    continue;
                }


                if (line.Contains("#关键词标签#"))
                {
                    article.Tag = line.Replace("#关键词标签#", "");
                    continue;
                }
                if (line.Contains("#正文#"))
                {

                    isContent = true;
                    sb.AppendLine(Replace(line.Replace("#正文#", "")));

                    continue;
                }
                if (isContent)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    sb.AppendLine(Replace(line));
                }

            }
            if (sb.Length > 0)
            {
                article.Content = sb.ToString();
                articles.Add(article);
            }
            return articles;
        }
        private string Replace(string line)
        {
            string newLine = Regex.Replace(line, "@图片开始@(.*?)@图片结束@", "<p style=\"text-align: center; \"><img src=\"$1\" style =\"max-width: 100%;\"></p>");

            newLine = Regex.Replace(newLine, "[（(](.*?)[)）]", "<b>$1</b>");
            newLine = Regex.Replace(newLine, "@瞄点开始@(.*?)@瞄点结束@", "<a name=\"$1\"></a>");
            return string.Format(PLine, newLine);

        }
        private void SetUserInfo(string content)
        {
            string nameAndPwd = content.Replace("#账号#", "");
            var arrays = nameAndPwd.Split(new string[] { "--" }, StringSplitOptions.None);

            UserName = arrays[0];
            Password = arrays[1];
            IsReadUserName = true;
        }



        ///// <summary>
        ///// 读取多账号
        ///// </summary>
        ///// <param name="fileName"></param>
        ///// <returns></returns>
        //public static List<Account> GetAccountByFile(string fileName)
        //{
        //    List<Account> users = new List<Account>();
        //    var allLines = File.ReadAllLines(fileName);
        //    Account account = null;
        //    foreach (var line in allLines)
        //    {
        //        if (line.Contains("#公司#"))
        //        {
        //            account = new Account();
        //            string nameAndPwd = line.Replace("#公司#", "");
        //            account.Company = nameAndPwd;
        //        }
        //        if (line.Contains("#账号#"))
        //        {
        //            string nameAndPwd = line.Replace("#账号#", "");
        //            var arrays = nameAndPwd.Split(new string[] { "--" }, StringSplitOptions.None);
        //            account.UserName = arrays[0];
        //            account.Password = arrays[1];
        //            //users.Add(new Account(arrays[0], arrays[1]));
        //            users.Add(account);
        //        }
        //    }
        //    return users;
        //}

        public static List<Account> GetAccounts(string fileName)
        {
            List<Account> anchors = new List<Account>();
            DetectionResult result = CharsetDetector.DetectFromFile(fileName);
            var allLines = File.ReadAllLines(fileName, result.Detected.Encoding);
            Account anchor = null;
            foreach (var line in allLines)
            {
                if (line.Contains("#公司#"))
                {
                    if (anchor != null)
                    {
                        anchors.Add(anchor);
                    }
                    anchor = new Account();
                    string nameAndPwd = line.Replace("#公司#", "");
                    anchor.Company = nameAndPwd;
                    continue;
                }
                if (line.Contains("#账号#"))
                {

                    string nameAndPwd = line.Replace("#账号#", "");
                    var arrays = nameAndPwd.Split(new string[] { "--" }, StringSplitOptions.None);

                    anchor.SetNameAndPassword(arrays[0], arrays[1]);
                    if (arrays.Length > 2)
                    {
                        try
                        {
                            anchor.StartPaged = Convert.ToInt32(arrays[2]);
                            anchor.EndPaged = Convert.ToInt32(arrays[3]);
                        }
                        catch (Exception e)
                        {
                            throw new Exception($"{anchor.UserName}账号格式不正确，请调整账号页数");
                        }

                    }
                    continue;
                }
                if (line.Contains("#手机#"))
                {
                    string nameAndPwd = line.Replace("#手机#", "");
                    anchor.Phone = nameAndPwd;
                    continue;
                }
                if (line.Contains("#瞄点#"))
                {
                    string anchorStr = line.Replace("#瞄点#", "");
                    anchor.OriginAnchor = anchorStr + "|";
                    anchor.ReadAnchor = anchorStr.Split('|')?.ToList()?.Select(s => string.IsNullOrEmpty(s) ? "" : $"<p></p><p></p><p>&nbsp&nbsp<a name=\"{s}\"></a></p>")?.ToList();
                    continue;
                }
                if (line.Contains("@图片开始@"))
                {
                    anchor.OriginPicUrl.Add(line);
                    string newLine = ReplayPic(line);

                    anchor.ReadPicUrl.Add(newLine);
                }
            }
            if (anchor != null)
            {
                anchors.Add(anchor);
            }
            return anchors;
        }


        public static List<LinkAccount> GetLinkAccounts(string fileName)
        {
            List<LinkAccount> anchors = new List<LinkAccount>();
            DetectionResult result = CharsetDetector.DetectFromFile(fileName);
            var allLines = File.ReadAllLines(fileName, result.Detected.Encoding);
            LinkAccount anchor = null;
            foreach (var line in allLines)
            {
                if (line.Contains("#公司#"))
                {
                    if (anchor != null)
                    {
                        anchors.Add(anchor);
                    }
                    anchor = new LinkAccount();
                    string nameAndPwd = line.Replace("#公司#", "");
                    anchor.Company = nameAndPwd;
                    continue;
                }
                if (line.Contains("#账号#"))
                {

                    string nameAndPwd = line.Replace("#账号#", "");
                    var arrays = nameAndPwd.Split(new string[] { "--" }, StringSplitOptions.None);

                    anchor.SetNameAndPassword(arrays[0], arrays[1]);

                    continue;
                }
                if (line.Contains("#是否禁止复制#"))
                {
                    string isCopyStr = line.Replace("#是否禁止复制#", "");
                    anchor.IsCopy = isCopyStr.Trim().Equals("是");
                    continue;
                }
                if (line.Contains("#是否手机网站#"))
                {
                    string isAppStr = line.Replace("#是否手机网站#", "");
                    anchor.IsApp = isAppStr.Trim().Equals("是");
                    continue;
                }
                if (line.Contains("#链接#"))
                {
                    string titleAndLink = line.Replace("#链接#", "");
                    var arrays = titleAndLink.Split(new string[] { "--" }, StringSplitOptions.None);
                    anchor.LinkUrl.Add(new LinkModel(arrays[0], arrays[1]));
                }
            }
            if (anchor != null)
            {
                anchors.Add(anchor);
            }
            return anchors;
        }



        private static string ReplayPic(string line)
        {
            return Regex.Replace(line, "@图片开始@(.*?)@图片结束@", "<p></p><p></p><p style=\"text-align: center; \"><img src=\"$1\" style =\"max-width: 100%;\"></p>");
        }
    }
}

