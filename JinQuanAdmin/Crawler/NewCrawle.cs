﻿using JinQuanAdmin.Common;
using JinQuanAdmin.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace JinQuanAdmin.Crawler
{
    public class NewCrawle : IDisposable
    {
        private IWebDriver _webDriver;
        public NewCrawle() : this("")
        {
        }

        public ChromeDriverService ChromeDriverService { get; set; }
        public ChromeOptions ChromeOptions { get; set; }
        public NewCrawle(string proxy)
        {
            ChromeDriverService = ChromeDriverService.CreateDefaultService(Environment.CurrentDirectory + "/Package");
            try
            {
                ChromeOptions = new ChromeOptions();
                ChromeDriverService.HideCommandPromptWindow = true;
                ChromeOptions.AddArguments("--headless");
                ChromeOptions.AddArgument("no-sandbox");
                ChromeOptions.AddArgument("--disable-gpu");

                ChromeOptions.AddExcludedArgument("enable-automation");
                ChromeOptions.AddAdditionalCapability("useAutomationExtension", false);
                //options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);//禁止加载图片
                string exePath = Properties.Settings.Default.ExePath;

                if (!string.IsNullOrEmpty(exePath))
                {
                    if (!File.Exists(exePath))
                    {
                        throw new Exception("未找到浏览器路径");
                    }
                    ChromeOptions.BinaryLocation = exePath;
                }
                if (!string.IsNullOrEmpty(proxy))
                {
                    //var proxyIpAddress = "tps111.kdlapi.com:15818"; //_proxyProvider.GetProxyIpAddress();
                    ChromeOptions.AddArgument("--proxy-server=http://" + proxy);

                }
                _webDriver = new ChromeDriver(ChromeDriverService, ChromeOptions, TimeSpan.FromMinutes(5));

            }
            catch (Exception e)
            {
                ChromeDriverService.Dispose();
                LogHelper.LogAction.Invoke($"浏览器启动失败");
                throw e;
            }
        }
        private string login_url = "https://my.jqw.com/2017/web/login2017.aspx";
        private string loing_name_id = "txtLoginName";
        private string loing_pwd_id = "txtLoginpas";
        private string loing_submit_id = "btsure";
        private string manager_list_xpath = "//ul[@id='htmlstr']/li[2]";
        private string back_url = @"https://member.jqw.com/member2015/index.aspx";
        private void CleanCookie()
        {
            _webDriver.Manage().Cookies.DeleteAllCookies();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool Login(string username, string pwd)
        {
            try
            {
                LogHelper.LogAction.Invoke($"账号：{username}，开始登陆");
                closeAllALert();
                CleanCookie();
                _webDriver.Navigate().GoToUrl(login_url);
                Thread.Sleep(2_000);
                _webDriver.FindElement(By.Id(loing_name_id), 10).SendKeys(username);
                _webDriver.FindElement(By.Id(loing_pwd_id), 10).SendKeys(pwd);
                _webDriver.FindElement(By.Id(loing_submit_id), 10).Click();
                Thread.Sleep(3_000);
                closeAllALert();
                if (_webDriver.IsElementExist(By.Id("hide")))
                {
                    return false;
                }
                if (_webDriver.Url.Contains("error"))
                {
                    return false;
                }
                _webDriver.Navigate().GoToUrl(back_url);
                LogHelper.LogAction.Invoke("登录成功");
                //_webDriver.FindElement(By.XPath(manager_list_xpath), 10).Click();
                return true;
            }
            catch (Exception e)
            {
                LogHelper.LogAction.Invoke("登录失败，" + e.Message);

                return false;
            }
        }

        public void closeAllALert()
        {
            while (isAlertPersent())
            {
                _webDriver.SwitchTo().Alert().Accept();
                try
                {
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {

                }
            }
        }

        public bool isAlertPersent()
        {
            try
            {
                _webDriver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException e) { return false; }
        }



        private string pro_url = "/produceList.aspx";
        private string news_url = "/news_list.aspx";
        private string sup_url = "/WholesaleList.aspx";
        private string company_url = "/postCompany.aspx";
        private string company_contact_url = "/CompanyContact.aspx";
        private string function_link_url = "/friendsLink.aspx";
        private string function_settion_url = "/closeCopy.aspx";
        private string MeunUrl = "";

        #region 获取文章列表        
        private bool NavigateList(MenuType menuType)
        {

            string cuurentUrl = _webDriver.Url;

            MeunUrl = cuurentUrl.Substring(0, cuurentUrl.LastIndexOf("/"));

            switch (menuType)
            {
                case MenuType.produceList:
                    MeunUrl = MeunUrl + pro_url;
                    break;
                case MenuType.news_list:
                    MeunUrl = MeunUrl + news_url;
                    break;
                case MenuType.WholesaleList:
                    MeunUrl = MeunUrl + sup_url;
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(MeunUrl))
            {
                LogHelper.LogAction.Invoke("获取栏目文章列表失败");
                return false;
            }
            else
            {
                _webDriver.Navigate().GoToUrl(MeunUrl);
                return true;
            }
        }

        private string article_id_xpath = "//table//td[@class='tdPosi']/span/a[1]";
        private int pageSize = 16;
        private string pagecount_xpath = "//form//font[@color='red']";
        /// <summary>
        /// 获取所有文章列表
        /// </summary>
        /// <param name="menuType"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<string> GetArticleUrls(MenuType menuType, int count, int start = -1, int end = -1)
        {

            LogHelper.LogAction.Invoke($"正在采集{menuType.GetDescription()}文章列表");
            List<string> idList = new List<string>();
            try
            {
                if (NavigateList(menuType))
                {
                    var firstList = _webDriver.FindElements(By.XPath(article_id_xpath), 10);
                    if (count < 0)
                    {
                        count = Convert.ToInt32(_webDriver.FindElement(By.XPath(pagecount_xpath), 10).Text);
                    }
                    int needPage = (count + pageSize - 1) / pageSize;
                    if (end > 0 && needPage > end) { needPage = end; }
                    LogHelper.LogAction.Invoke($"{menuType.GetDescription()}将要采集文章数量：{count},总页数:{needPage}");

                    if (firstList != null && start < 2)
                    {
                        LogHelper.LogAction.Invoke($"{menuType.GetDescription()}开始采集第 1 页文章");

                        foreach (var item in firstList)
                        {
                            idList.Add(item.GetAttribute("href"));
                        }
                    }
                    if (needPage > 1)
                    {
                        for (int i = start < 2 ? 2 : start; i <= needPage; i++)
                        {

                            Thread.Sleep(2_000);
                            _webDriver.Navigate().GoToUrl(string.Concat(MeunUrl, "?p=", i));
                            LogHelper.LogAction.Invoke($"{menuType.GetDescription()}开始采集第 {i} 页文章");
                            var tempList = _webDriver.FindElements(By.XPath(article_id_xpath), 10);
                            if (tempList != null)
                            {
                                foreach (var item in tempList)
                                {
                                    idList.Add(item.GetAttribute("href"));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.LogAction.Invoke("文章列表失败，" + e.Message);
                idList = new List<string>();
            }
            LogHelper.LogAction.Invoke($"{menuType.GetDescription()}采集结束,获取文章数{idList.Count}");
            Thread.Sleep(2 * 1000);
            return idList;
        }


        /// <summary>
        /// 列表行
        /// </summary>
        private string article_rows = "//table//td//tr[position()>1]";
        private string article_row_ckeckbox = "//input[@type='checkbox']";
        private string article_row_title = "//td[3]/span";
        private string article_row_old_title = "//td[2]/span";
        /// <summary>
        /// ArticleTitle
        /// </summary>
        /// <param name="menuType"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<ArticleTitle> GetArticlesTitles(MenuType menuType, int count, out int total, out int pageTotal)
        {

            LogHelper.LogAction.Invoke($"正在采集{menuType.GetDescription()}文章列表");
            List<ArticleTitle> list = new List<ArticleTitle>();
            pageTotal = 0;
            string title_position = menuType == MenuType.produceList ? article_row_title : article_row_old_title;
            try
            {
                if (NavigateList(menuType))
                {
                    var firstValueList = _webDriver.FindElements(By.XPath(article_rows + article_row_ckeckbox), 10);


                    var firstTitleList = _webDriver.FindElements(By.XPath(article_rows + title_position), 10);


                    if (count < 0)
                    {
                        count = Convert.ToInt32(_webDriver.FindElement(By.XPath(pagecount_xpath), 10).Text);
                    }
                    int needPage = (count + pageSize - 1) / pageSize;
                    pageTotal = needPage;
                    LogHelper.LogAction.Invoke($"{menuType.GetDescription()}将要采集文章数量：{count},总页数:{needPage}");

                    if (firstValueList != null)
                    {
                        LogHelper.LogAction.Invoke($"{menuType.GetDescription()}开始采集第 1 页文章");

                        for (int i = 0; i < firstValueList.Count; i++)
                        {
                            ArticleTitle articleTitle = new ArticleTitle();
                            articleTitle.Value = firstValueList[i].GetAttribute("value");
                            string title = firstTitleList[i].Text;
                            articleTitle.Title = title.Substring(0, title.IndexOf("\r\n"));
                            list.Add(articleTitle);
                        }
                    }
                    if (needPage > 1)
                    {
                        for (int i = 2; i <= needPage; i++)
                        {

                            Thread.Sleep(2_000);
                            _webDriver.Navigate().GoToUrl(string.Concat(MeunUrl, "?p=", i));
                            LogHelper.LogAction.Invoke($"{menuType.GetDescription()}开始采集第 {i} 页文章");
                            var tempValueList = _webDriver.FindElements(By.XPath(article_rows + article_row_ckeckbox), 10);
                            var tempTitleList = _webDriver.FindElements(By.XPath(article_rows + title_position), 10);
                            if (tempValueList != null)
                            {
                                for (int j = 0; j < tempValueList.Count; j++)
                                {
                                    ArticleTitle articleTitle = new ArticleTitle();
                                    articleTitle.Value = tempValueList[j].GetAttribute("value");
                                    articleTitle.Title = tempTitleList[j].Text;
                                    list.Add(articleTitle);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.LogAction.Invoke("文章列表失败，" + e.Message);
                list = new List<ArticleTitle>();
            }
            LogHelper.LogAction.Invoke($"{menuType.GetDescription()}采集结束");
            Thread.Sleep(2 * 1000);
            total = count;
            return list;
        }
        #endregion

        #region 自动发布功能        
        private string title_id = "txtTitle";
        private string context_id = "editor-trigger";
        private string sumbit_id = "save";

        Random random = new Random();
        private string pro_title_id = "txtPicName";
        public string tag_id = "txttag";
        /// <summary>
        /// 文章提交
        /// </summary>
        public bool SubmitArticle(MenuType menuType, string url, Article article)
        {
            try
            {
                Thread.Sleep(3 * 1000);
                _webDriver.Navigate().GoToUrl(url);
                LogHelper.LogAction.Invoke("发布文章:" + article.Title);
                var jsDriver = (IJavaScriptExecutor)_webDriver;

                string title = article.Title;
                bool isIgnoreTitle = Form1.IsIgnoreTitle;


                if (menuType == MenuType.produceList)
                {
                    //_webDriver.FindElement(By.Id(pro_title_id), 10).SendKeys(article.Title);
                    //_webDriver.FindElement(By.Id(pro_title_id), 10).SendKeys(article.Title);
                    if (isIgnoreTitle)
                    {
                        title = jsDriver.ExecuteScript($"return document.getElementById('{pro_title_id}').value;").ToString();
                    }
                    else
                    {
                        jsDriver.ExecuteScript($"document.getElementById('{pro_title_id}').value = '{title}';");
                        string tag = string.IsNullOrEmpty(article.Tag) ? article.Title : article.Tag;
                        var tagEl = _webDriver.FindElement(By.Name(tag_id), 3);
                        tagEl.Clear();
                        tagEl.SendKeys(tag);
                    }

                    //jsDriver.ExecuteScript($"document.getElementsByName('{tag_id}').value = '{tag}';");
                }
                else
                {
                    if (isIgnoreTitle)
                    {
                        title = jsDriver.ExecuteScript($"return document.getElementById('{title_id}').value;").ToString();
                    }
                    else
                    {
                        jsDriver.ExecuteScript($"document.getElementById('{title_id}').value = '{article.Title}';");
                    }

                }
                var context = article.Content;
                if (isIgnoreTitle)
                {
                    int firstPeriod = context.IndexOf('。') + 1;
                    context = context.Insert(firstPeriod, title + "。");
                }
                var contextet = System.Text.RegularExpressions.Regex.Escape(context);
                jsDriver.ExecuteScript($"document.getElementById('{context_id}').innerText = '{contextet}';");
                jsDriver.ExecuteScript($"document.getElementsByClassName('wangEditor-txt')[0].innerText = '{contextet}';");
                Thread.Sleep(1000 * random.Next(10, 21));
                _webDriver.FindElement(By.Id(sumbit_id), 10).Click();
                Thread.Sleep(3 * 1000);
                return true;
            }
            catch (Exception e)
            {
                LogHelper.LogAction.Invoke(url + " ,发布失败：" + e.Message);
                return false;
            }

        }

        #endregion

        #region 替换发布

        Regex SpecialCharacters = new Regex("[^\\u4E00-\\u9FFF\\da-zA-Z]");
        public bool ReplaceArticle(string url, Dictionary<string, string> replaceDic, bool isCheckTag)
        {
            try
            {
                Thread.Sleep(1 * 1000);
                _webDriver.Navigate().GoToUrl(url);
                Thread.Sleep(2 * 1000);

                var jsDriver = (IJavaScriptExecutor)_webDriver;
                string title = "";
                if (isCheckTag)
                {
                    string tag = jsDriver.ExecuteScript($"return document.getElementById('{tag_id}').value;").ToString();
                    title = jsDriver.ExecuteScript($"return document.getElementById('{pro_title_id}').value;").ToString();
                    if (SpecialCharacters.IsMatch(title))
                    {
                        title = SpecialCharacters.Replace(title, string.Empty);
                    }

                    if (string.IsNullOrEmpty(tag))
                    {
                        jsDriver.ExecuteScript($"document.getElementById('{tag_id}').value = '{title.Substring(0, 10)}';");
                    }
                }
                else
                {
                    title = jsDriver.ExecuteScript($"return document.getElementById('{title_id}').value;").ToString();
                }
                string content = jsDriver.ExecuteScript($"return document.getElementsByClassName('wangEditor-txt')[0].innerHTML;").ToString();
                foreach (var item in replaceDic)
                {
                    content = content.Replace(item.Key, item.Value);
                    title = title.Replace(item.Key, item.Value);
                }

                if (isCheckTag)
                {
                    jsDriver.ExecuteScript($"document.getElementById('{pro_title_id}').value = '{title}';");
                }
                else
                {
                    jsDriver.ExecuteScript($"document.getElementById('{title_id}').value = '{title}';");
                }

                var contextet = System.Text.RegularExpressions.Regex.Escape(content);
                jsDriver.ExecuteScript($"document.getElementById('{context_id}').innerHTML = '{contextet}';");
                jsDriver.ExecuteScript($"document.getElementsByClassName('wangEditor-txt')[0].innerHTML = '{contextet}';");
                Thread.Sleep(1000 * random.Next(3, 11));
                _webDriver.FindElement(By.Id(sumbit_id), 10).Click();
                Thread.Sleep(3 * 1000);
                return true;
            }
            catch (Exception e)
            {
                LogHelper.LogAction.Invoke(url + " ,发布失败：" + e.Message);
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }



        private string baidu_rows = "//body//div[contains(@class,'result') and contains(@class,'c-container') and contains(@class,'new-pmd')]//h3[@class='t']";

        private string baidu_first_match = "//div[@class='c-line-clamp1']";

        private bool first_enter = true;
        /// <summary>
        /// 百度是否收录
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public BaiduResponseResult IsBaiduRecord(string title)
        {
            try
            {


                var newTitle = ToDBC(title);
                var kw = System.Web.HttpUtility.UrlEncode(newTitle, System.Text.Encoding.UTF8);
                string baiduUrl = $"https://www.baidu.com/s?wd={kw}";
                IJavaScriptExecutor js = (IJavaScriptExecutor)_webDriver;
                string returnjs = (string)js.ExecuteScript("Object.defineProperties(navigator, {webdriver:{get:()=>undefined}});");
                _webDriver.Navigate().GoToUrl(baiduUrl);

                if (first_enter)
                {
                    Thread.Sleep(3000);
                    first_enter = false;
                }
                else
                {
                    Thread.Sleep(1000);
                }
                if (_webDriver.Url.Contains("wappass"))
                {
                    return BaiduResponseResult.IpBlackIntercept;
                }
                if (_webDriver.PageSource.Length < 100)
                {
                    return BaiduResponseResult.ProxyException;
                }
                if (!_webDriver.IsElementExist(By.Id("container")))
                {
                    return BaiduResponseResult.IpBlackIntercept;
                }
                if (newTitle.Length > 27)
                {
                    newTitle = newTitle.Substring(0, 27);
                }
                if (_webDriver.IsElementExist(By.XPath(baidu_first_match)))
                {
                    var fisrtTxt = _webDriver.FindElement(By.XPath(baidu_first_match), 10).Text;
                    if (fisrtTxt.StartsWith(newTitle))
                    {

                        return BaiduResponseResult.Included;
                    }
                }
                var titles = _webDriver.FindElements(By.XPath(baidu_rows));
                foreach (var item in titles)
                {
                    if (item.Text.StartsWith(newTitle))
                    {
                        return BaiduResponseResult.Included;
                    }
                }
                return BaiduResponseResult.None;
            }
            catch (Exception exception)
            {
                LogHelper.LogAction.Invoke($"标题：{title},搜索异常{exception.Message},{exception.StackTrace}");
                _webDriver = new ChromeDriver(ChromeDriverService, ChromeOptions, TimeSpan.FromMinutes(5));
                return BaiduResponseResult.Exception;
            }
        }

        private string row_ck_name = "cbproduce";
        private string bt_top_btreftch = "btreftch";
        public void RefreshSetTop(List<ArticleTitle> list, MenuType type)
        {
            int needPage = (list.Count + pageSize - 1) / pageSize;
            _webDriver.Navigate().GoToUrl(string.Concat(MeunUrl, "?p=", 1));
            Thread.Sleep(2_000);


            var jsDriver = (IJavaScriptExecutor)_webDriver;
            for (int i = 1; i <= needPage; i++)
            {
                IWebElement button;
                if (type == MenuType.produceList)
                {
                    button = _webDriver.FindElement(By.Id("btReftch"), 10);
                }
                else
                {
                    button = _webDriver.FindElement(By.Id(bt_top_btreftch), 10);
                }
                var setList = list.Skip((i - 1) * 16).Take(16).ToList();
                for (int j = 0; j < setList.Count; j++)
                {
                    jsDriver.ExecuteScript($"document.getElementsByName('{row_ck_name}')[{j}].value = '{setList[j].Value}';");
                    jsDriver.ExecuteScript($"document.getElementsByName('{row_ck_name}')[{j}].checked = true;");
                }
                button.Click();
                Thread.Sleep(2_000);
            }

        }


        /// <summary>
        /// 插入锚点
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        public void InsertAnchor(string url, string content)
        {
            Thread.Sleep(1 * 1000);
            _webDriver.Navigate().GoToUrl(url);
            var jsDriver = (IJavaScriptExecutor)_webDriver;
            string original = jsDriver.ExecuteScript($"return document.getElementsByClassName('wangEditor-txt')[0].innerHTML;").ToString();
            string newContent = original + content;
            var contextet = System.Text.RegularExpressions.Regex.Escape(newContent);
            jsDriver.ExecuteScript($"document.getElementById('{context_id}').innerHTML = '{contextet}';");
            jsDriver.ExecuteScript($"document.getElementsByClassName('wangEditor-txt')[0].innerHTML = '{contextet}';"); Thread.Sleep(1000 * random.Next(3, 11));
            Thread.Sleep(2 * 1000);
            _webDriver.FindElement(By.Id(sumbit_id), 10).Click();
            Thread.Sleep(2 * 1000);

        }

        string xpath_product_pic = "//img[@name]";
        static string regex_domain = @"^((http://)|(https://))?([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,6}";

        Regex ImgRegex = new Regex(regex_domain);
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<string> GetProductPic(string url)
        {
            List<string> list = new List<string>();
            Thread.Sleep(1 * 1000);
            _webDriver.Navigate().GoToUrl(url);
            string domain = ImgRegex.Match(url).ToString();
            var picElements = _webDriver.FindElements(By.XPath(xpath_product_pic), 10);
            foreach (var item in picElements)
            {
                var pic = item.GetAttribute("src");
                if (!pic.EndsWith("image_Vip.jpg"))
                {

                    int index = pic.LastIndexOf("/") + 1;
                    pic = pic.Insert(index, "b");
                    list.Add(pic);

                }

            }
            return list;
        }

        private string xpath_compay_contact_Phone_id = "txtMobile";
        private string xpath_compay_contact_submit_id = "Submit";

        /// <summary>
        /// 设置手机号码
        /// </summary>
        /// <param name="phone"></param>
        public void UpdatePhone(string phone)
        {
            Thread.Sleep(1_000);
            string cuurentUrl = _webDriver.Url;
            string url = cuurentUrl.Substring(0, cuurentUrl.LastIndexOf("/")) + company_contact_url;
            _webDriver.Navigate().GoToUrl(url);
            Thread.Sleep(2_000);
            var jsDriver = (IJavaScriptExecutor)_webDriver;
            jsDriver.ExecuteScript($"document.getElementById('{xpath_compay_contact_Phone_id}').value = '{phone}';");

            _webDriver.FindElement(By.Id(xpath_compay_contact_submit_id), 10).Click();
            Thread.Sleep(1_000);
            closeAllALert();
        }

        private string xpath_company_keyword_name = "//input[@name='txtkeyword']";
        /// <summary>
        /// 获取锚点
        /// </summary>
        /// <returns></returns>
        public List<string> GetAnchorList()
        {
            Thread.Sleep(2_000);
            List<string> list = new List<string>();
            string cuurentUrl = _webDriver.Url;

            string url = cuurentUrl.Substring(0, cuurentUrl.LastIndexOf("/")) + company_url;
            _webDriver.Navigate().GoToUrl(url);
            Thread.Sleep(2_000);
            var elements = _webDriver.FindElements(By.XPath(xpath_company_keyword_name), 10);

            foreach (var item in elements)
            {
                var value = item.GetAttribute("value");
                list.Add(value);
            }

            return list;
        }


        private string link_title = "//form[@id='Form1']//input[@name='txtTitle']";
        private string link_url = "//form[@id='Form1']//input[@name='txtUrl']";
        private string link_submit = "//form[@id='Form1']//input[@type='submit']";
        private string link_max_num = "//td[@class='list_right_box']//tbody//tr[last()]/td[1]";

        public bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        /// <summary>
        /// 添加友情链接
        /// </summary>
        /// <param name="linkModels"></param>
        public void AddLinkList(List<LinkModel> linkModels)
        {
            if (!linkModels.Any())
            {
                return;
            }
            Thread.Sleep(1_000);
            string cuurentUrl = _webDriver.Url;
            string url = cuurentUrl.Substring(0, cuurentUrl.LastIndexOf("/")) + function_link_url;
            _webDriver.Navigate().GoToUrl(url);
            Thread.Sleep(2_000);
            string numStr = _webDriver.FindElement(By.XPath(link_max_num), 10).Text;
            if (IsNumeric(numStr))
            {
                var num = Convert.ToInt32(numStr);
                for (int i = 0; i < num; i++)
                {
                    _webDriver.FindElement(By.Id($"del0")).Click();
                }
            }
            var jsDriver = (IJavaScriptExecutor)_webDriver;
            foreach (var item in linkModels.Take(10))
            {

                LogHelper.LogAction.Invoke($"添加链接{item.Url}");
                jsDriver.ExecuteScript($"showDiv();");
                _webDriver.FindElement(By.XPath(link_title)).SendKeys(item.Title);
                _webDriver.FindElement(By.XPath(link_url)).SendKeys(item.Url);
                _webDriver.FindElement(By.XPath(link_submit), 10).Click();
                Thread.Sleep(1_500);
            }
        }

        private string function_is_copy = "//input[@name='radiowIsCopy' and  @value='#value#']";
        private string function_is_app = "//input[@name='radioTiaoZhuanPc' and  @value='#value#']";
        private string function_is_copy_submit = "//body[1]/form[1]/div[5]/div[1]/div[2]/div[3]/table[1]/tbody[1]/tr[3]/td[1]/input[1]";
        private string function_is_app_submit = "//body[1]/form[1]/div[5]/div[1]/div[2]/div[4]/table[1]/tbody[1]/tr[3]/td[1]/input[1]";

        public void ChangeFunctionSetting(bool? isCopy, bool? isApp)
        {

            Thread.Sleep(1_000);
            string cuurentUrl = _webDriver.Url;
            string url = cuurentUrl.Substring(0, cuurentUrl.LastIndexOf("/")) + function_settion_url;
            _webDriver.Navigate().GoToUrl(url);
            Thread.Sleep(2_000);
            if (isCopy.HasValue)
            {
                LogHelper.LogAction.Invoke($"设置是否允许复制");
                var elem = "";
                if (isCopy.Value)
                {
                    elem = function_is_copy.Replace("#value#", "1");
                }
                else
                {
                    elem = function_is_copy.Replace("#value#", "0");
                }
                _webDriver.FindElement(By.XPath(elem)).Click();
                _webDriver.FindElement(By.XPath(function_is_copy_submit)).Click();
                Thread.Sleep(3000);
            }
            if (isApp.HasValue)
            {
                LogHelper.LogAction.Invoke($"设置移动端访问二级网址显示");
                var elem = "";
                if (isApp.Value)
                {
                    elem = function_is_app.Replace("#value#", "0");
                }
                else
                {
                    elem = function_is_app.Replace("#value#", "1");
                }
                _webDriver.FindElement(By.XPath(elem)).Click();
                _webDriver.FindElement(By.XPath(function_is_app_submit)).Click();
                Thread.Sleep(2000);
            }
        }

        public void Dispose()
        {
            if (_webDriver != null)
            {
                _webDriver.Close();
            }
        }
    }
}
