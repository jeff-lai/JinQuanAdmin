using JinQuanAdmin.Common;
using JinQuanAdmin.Crawler;
using JinQuanAdmin.Model;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinQuanAdmin
{
    public partial class Form1 : Form
    {

        private Dictionary<string, string> ReplaceDic = new Dictionary<string, string>();

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref System.UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);


        #region 公共属性

        /// <summary>
        /// 菜单集合
        /// </summary>
        HashSet<MenuType> _MenuTypesSets = new HashSet<MenuType>();
        /// <summary>
        /// 用户名
        /// </summary>
        private string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        private string Password { get; set; }
        /// <summary>
        /// 文章列表
        /// </summary>
        private List<Article> Articles { get; set; }

        /// <summary>
        /// 是否忽略标题
        /// </summary>
        public static bool IsIgnoreTitle;


        /// <summary>
        /// 文件路径
        /// </summary>
        private string FilePath = "";
        #endregion

        public Form1()
        {
            InitializeComponent();

        }

        #region Load Events


        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            this.txt_log.Enabled = false;
            LogHelper.LogAction = (log) => WriteLogger(log);
            string path = Properties.Settings.Default.ExePath;
            if (!string.IsNullOrEmpty(path))
            {
                WriteLogger($"浏览器执行路径：{path}");
            }
            foreach (CheckBox ck in gb_ck_menu.Controls)
            {
                ck.CheckedChanged += Ck_CheckedChanged;
            }
        }

        /// <summary>
        /// 菜单选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ck_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ck = sender as CheckBox;
            MenuType type = (MenuType)Convert.ToInt32(ck.Tag);
            if (ck.Checked)
            {
                WriteLogger("选择栏目：" + ck.Text);
                _MenuTypesSets.Add(type);

            }
            else
            {
                _MenuTypesSets.Remove(type);
                WriteLogger("取消选择栏目：" + ck.Text);
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        /// <summary>
        /// 浏览器设置事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_setting_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择谷歌浏览器地址";
            dialog.Filter = "文本文件(*.exe)|*.exe";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                Properties.Settings.Default.ExePath = file;
                Properties.Settings.Default.Save();
                WriteLogger($"执行路径已保存：{file}");
            }
            else
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.ExePath))
                {
                    if (MessageBox.Show("是否要清除浏览器执行路径？", "确认删除", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Properties.Settings.Default.ExePath = "";
                        Properties.Settings.Default.Save();
                    }
                }
            }
        }


        /// <summary>
        /// 日志导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_log_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "请选择保存文件路径";
            saveFile.Filter = "文本文件|*.txt";
            saveFile.OverwritePrompt = true;  //是否覆盖当前文件
            saveFile.RestoreDirectory = true;  //还原目录
            //saveFile.FileName = "替换日志" + DateTime.Now.ToString("yyyyMMddHH:mm:ss");
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                string filepath = saveFile.FileName;
                WriteTxt(filepath);
                MessageBox.Show("导出成功", "消息");
            }
            else
            {
                return;
            }
        }

        #endregion

        #region 公共方法


        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择文件";
            dialog.Filter = "文本文件(*.txt)|*.txt";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = dialog.FileName;
                WriteLogger($"加载文件：{FilePath}");
            }
        }

        /// <summary>
        /// 设置文本变更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_log_TextChanged(object sender, EventArgs e)
        {
            this.txt_log.SelectionStart = this.txt_log.Text.Length;
            this.txt_log.SelectionLength = 0;
            this.txt_log.ScrollToCaret();
        }


        /// <summary>
        /// 检查参数
        /// </summary>
        /// <returns></returns>
        private bool CheckParamter()
        {

            if (Articles == null || !Articles.Any())
            {
                MessageBox.Show("请选择文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (_MenuTypesSets.Count != 1)
            {
                MessageBox.Show("请选择一个栏目,不支持多选", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log"></param>
        private void WriteLogger(string log)
        {
            if (txt_log.InvokeRequired)
            {
                Action<string> action = new Action<string>(WriteLogger);
                Invoke(action, new object[] { log });

            }
            else
            {
                txt_log.AppendText($"{DateTime.Now.ToString("HH:mm:ss")}:");
                txt_log.AppendText(log);
                txt_log.AppendText("...");
                txt_log.AppendText(Environment.NewLine);
            }

        }

        /// <summary>
        /// 设置控制器是否隐藏
        /// </summary>
        /// <param name="enable"></param>
        private void SetControllerEnable(bool enable)
        {
            if (this.btn_file.InvokeRequired)
            {
                Action<bool> action = new Action<bool>(SetControllerEnable);
                Invoke(action, new object[] { enable });
            }
            else
            {
                this.btn_file.Enabled = enable;
                this.btn_save.Enabled = enable;
                this.btn_post.Enabled = enable;
                this.btn_refresh.Enabled = enable;
                this.btn_anchor.Enabled = enable;
                this.btn_pic.Enabled = enable;
                this.btn_phone.Enabled = enable;
            }

        }

        /// <summary>
        /// 获取新路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetNewPath(string fileName)
        {
            int index = FilePath.LastIndexOf(".");

            string name = FilePath.Insert(index, fileName);
            return name;
        }

        /// <summary>
        /// 是否有文件地址
        /// </summary>
        /// <returns></returns>
        private bool isHasFilePath()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                MessageBox.Show("请选择文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path"></param>
        public void WriteTxt(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(txt_log.Text);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="accounts"></param>
        public void WriteTxt(string path, List<Account> accounts)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            accounts.ForEach(s => sw.Write(s));
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="account"></param>
        public void WriteTxt(string path, Account account)
        {
            var bytes = System.Text.Encoding.Default.GetBytes(account.ToString());
            using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                stream.Write(bytes, 0, bytes.Length);
            }

        }

        int LoginRetryNum = 0;
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="crawle"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool Login(NewCrawle crawle, string userName, string password)
        {
            if (!crawle.Login(userName, password))
            {
                LoginRetryNum++;
                if (LoginRetryNum < 3)
                {
                    Login(crawle, userName, password);
                }
                WriteLogger("登录失败");
                return false;
            };
            return true;
        }

        /// <summary>
        /// 获取账号
        /// </summary>
        /// <returns></returns>
        private List<Account> GetAccounts()
        {
            try
            {

                WriteLogger("开始加载文件中账号");
                var list = TxtHelper.GetAccounts(FilePath);
                if (!list.Any())
                {
                    WriteLogger("文件读取到账号请检查文件");
                }
                WriteLogger($"账号加载结束,加载账号数{list.Count}");
                return list;
            }
            catch (Exception e)
            {
                MessageBox.Show("文件格式不正确", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private List<T> GetAccounts<T>(Func<List<T>> func)
        {
            try
            {

                WriteLogger("开始加载文件中账号");
                var list = func();
                if (!list.Any())
                {
                    WriteLogger("文件读取到账号请检查文件");
                }
                WriteLogger($"账号加载结束,加载账号数{list.Count}");
                return list;
            }
            catch (Exception e)
            {
                MessageBox.Show("文件格式不正确", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region 主方法

        #region 1. 自动发布
        /// <summary>
        /// 自动发布
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_save_Click(object sender, EventArgs e)
        {
            if (!ReadFileAutoPostAndReplay())
            {
                return;
            }
            IsIgnoreTitle = cb_removeTitle.Checked;
            if (!CheckParamter())
            {
                return;
            }
            else
            {
                SetControllerEnable(false);
            }
            AutoPost();
        }

        /// <summary>
        /// 自动发布
        /// </summary>
        /// <returns></returns>        
        private Task AutoPost()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var crawler = new NewCrawle())
                    {
                        if (!crawler.Login(UserName, Password))
                        {
                            WriteLogger("登录失败");
                            return;
                        }

                        var urlList = crawler.GetArticleUrls(_MenuTypesSets.First(), Articles.Count);
                        if (!urlList.Any())
                        {
                            WriteLogger("文章加载失败");
                            return;
                        }

                        for (int i = 0; i < Articles.Count; i++)
                        {
                            if (i < urlList.Count)
                            {
                                crawler.SubmitArticle(_MenuTypesSets.First(), urlList[i], Articles[i]);
                            }
                            else
                            {
                                WriteLogger("文章发布失败,网站已无文章修改");
                                break;
                            }
                        }

                        SetControllerEnable(true);
                        WriteLogger("执行结束");
                    }
                }
                catch (Exception e)
                {

                    WriteLogger("执行错误:" + e.Message);
                    WriteLogger("执行结束");
                    SetControllerEnable(true);
                }
            });
        }

        #endregion

        #region 2. 替换发布
        private void btn_post_Click(object sender, EventArgs e)
        {
            if (!ReadFileAutoPostAndReplay())
            {
                return;
            }
            LoadDic();

            if (ReplaceDic.Count <= 0)
            {
                MessageBox.Show("没有需要替换的关键词，请输入替换内容", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("用户密码为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SetControllerEnable(false);
            ReplaceArticle();
        }
        private bool ReadFileAutoPostAndReplay()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                MessageBox.Show("未找到文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            var txtHelper = new TxtHelper();
            Articles = txtHelper.GetArticles(FilePath);
            UserName = txtHelper.UserName;
            Password = txtHelper.Password;

            WriteLogger($"加载文件数量：{Articles.Count}");

            WriteLogger($"登录账号：{UserName}");
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {

                MessageBox.Show("请检查文件未找到账号信息", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        private void LoadDic()
        {
            if (dgv_replace.RowCount > 0)
            {

                for (int i = 0; i < dgv_replace.RowCount; i++)
                {
                    if (dgv_replace.Rows[i].Cells[0].Value != null)
                    {
                        string key = dgv_replace.Rows[i].Cells[0].Value.ToString();
                        if (!ReplaceDic.ContainsKey(key))
                        {
                            ReplaceDic.Add(key.Trim(), dgv_replace.Rows[i].Cells[1]?.Value?.ToString() ?? "");
                        }

                    }
                }

            }
        }
        private void ReplaceArticle()
        {
            Task.Run(() =>
            {
                using (var crawle = new NewCrawle())
                {

                    if (!crawle.Login(UserName, Password))
                    {
                        WriteLogger("登录失败");
                        return;
                    };


                    foreach (var item in _MenuTypesSets)
                    {
                        var urlList = crawle.GetArticleUrls(item, -1);
                        if (!urlList.Any())
                        {
                            WriteLogger($"{item.GetDescription()}文章加载失败");
                            continue;
                        }
                        else
                        {
                            WriteLogger($"{item.GetDescription()}文章开始替换关键词");
                        }

                        int index = 0;
                        foreach (var url in urlList)
                        {
                            int count = index++;
                            WriteLogger($"{item.GetDescription()}正在替换修改第{count}文章,剩余{urlList.Count - count}");
                            crawle.ReplaceArticle(url, ReplaceDic, item == MenuType.produceList);
                        }
                    }

                    SetControllerEnable(true);
                    WriteLogger("执行结束");
                }
            });
        }

        #endregion

        #region 3. 爬取锚点
        private void btn_pic_Click(object sender, EventArgs e)
        {

            if (!isHasFilePath())
            {
                return;
            }
            string filePath = GetNewPath($"-已查锚图");
            var accounts = GetAccounts();
            if (accounts == null || !accounts.Any())
            {
                return;
            }
            SetControllerEnable(false);
            Task.Run(() =>
            {

                using (var crawle = new NewCrawle())
                {
                    try
                    {
                        foreach (var account in accounts)
                        {
                            LoginRetryNum = 0;
                            if (!Login(crawle, account.UserName, account.Password))
                            {

                                WriteLogger("登录失败");
                                continue;
                            };
                            WriteLogger("开始获取锚点");
                            account.WriteAnchor = crawle.GetAnchorList();
                            WriteLogger("获取锚点结束");
                            var urls = crawle.GetArticleUrls(MenuType.produceList, -1, account.StartPaged, account.EndPaged);
                            if (urls == null || !urls.Any())
                            {
                                WriteLogger("获取文章路径失败");
                                continue;
                            }

                            WriteLogger("开始获取图片");
                            foreach (var url in urls)
                            {
                                WriteLogger($"获取{url}图片");
                                var pics = crawle.GetProductPic(url);
                                if (pics.Any())
                                {
                                    account.WritePicUrl.AddRange(pics);
                                }
                            }
                            WriteLogger("获取图片结束");

                            WriteTxt(filePath, account);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLogger($"执行出错，{ex.Message},{ex.StackTrace}");
                        SetControllerEnable(true);
                        return;
                    }
                    SetControllerEnable(true);
                    WriteLogger($"已导出文件{filePath}");
                    WriteLogger("执行结束");
                }

            });
        }



        #endregion

        #region 4. 收录更新

        private string proxt_address = "";

        /// <summary>
        /// 收录更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_refresh_Click(object sender, EventArgs e)
        {
       
            if (_MenuTypesSets.Count != 1 && (_MenuTypesSets.FirstOrDefault() != MenuType.WholesaleList || _MenuTypesSets.FirstOrDefault() != MenuType.WholesaleList))
            {
                MessageBox.Show("请选择一个栏目,不支持多选", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!isHasFilePath())
            {
                return;
            }

            var accounts = GetAccounts();
            if (accounts == null || !accounts.Any())
            {
                return;
            }

            SetControllerEnable(false);
            RefreshSetTop(accounts);
        }
        private void RefreshSetTop(List<Account> accounts)
        {

            Task.Run(() =>
            {
                try
                {
                    using (var crawle = new NewCrawle())
                    {
                        string filePath = GetNewPath($"-已查收录");
                        foreach (var account in accounts)
                        {

                            if (!crawle.Login(account.UserName, account.Password))
                            {
                                WriteLogger("登录失败,请检查账号或者密码！");
                                continue;
                            };

                            int total = 0;
                            int pageTotal;

                            var list = crawle.GetArticlesTitles(_MenuTypesSets.First(), -1, out total, out pageTotal);
                            if (!list.Any())
                            {
                                WriteLogger("没有获取到文章");
                                return;
                            }
                                                
                            BaiduSearch(proxt_address, list, a =>
                             {
                                 List<ArticleTitle> one = new List<ArticleTitle>() { a };
                                 crawle.RefreshSetTop(one, _MenuTypesSets.First());

                             });
                            var topList = list.Where(s => s.Result == BaiduResponseResult.Included).ToList();
                            string includedMessage = "";
                            if (topList == null || !topList.Any())
                            {
                                includedMessage = $"栏目：{_MenuTypesSets.First().GetDescription()}，收录文章数：{0},未收录文章数量：{total},异常查询{list.Where(s => (int)s.Result > 1)},未收录页数第：{1}--{pageTotal}";
                                account.Included = includedMessage;
                                WriteLogger(includedMessage);
                                return;
                            }
                            int count = topList.Count;
                            WriteLogger($"收录文章数:{count},开始刷新置顶");
                            crawle.RefreshSetTop(topList, _MenuTypesSets.First());
                            int needPage = ((count + 16 - 1) / 16) + 1;

                            includedMessage = $"栏目：{_MenuTypesSets.First().GetDescription()}，收录文章数：{count},未收录文章数量：{total - count},未收录页数第：{needPage}--{pageTotal}";
                            account.Included = includedMessage;

                            WriteLogger(includedMessage);
                            WriteTxt(filePath, account);
                        }
                        WriteLogger($"已导出文件{filePath}");
                        WriteLogger($"执行结束");
                        SetControllerEnable(true);

                    }

                }
                catch (Exception e)
                {

                    WriteLogger($"执行异常{e.Message},{e.StackTrace}");
                    WriteLogger($"执行结束");
                    SetControllerEnable(true);
                }
            });
        }



        private void BaiduSearch(string proxy, List<ArticleTitle> articles, Action<ArticleTitle> action)
        {

            using (var baidu = new NewCrawle(proxy))
            {

                foreach (var item in articles)
                {                    
                    try
                    {
                        int retryNonefind = 0;
                        int retryCount = 0;
                        var result = Policy.HandleResult<BaiduResponseResult>(r => r == BaiduResponseResult.IpBlackIntercept)
                            .RetryForever()
                            .Wrap(Policy.HandleResult<BaiduResponseResult>(r => r == BaiduResponseResult.None && retryNonefind < 3).Retry(3))                   
                            .Wrap(Policy.HandleResult<BaiduResponseResult>(r => r == BaiduResponseResult.Exception).Retry(1))                                  
                            .Execute(() =>
                            {
                                var r = baidu.IsBaiduRecord(item.Title);
                                if (r == BaiduResponseResult.None)
                                {
                                    retryNonefind++;
                                    WriteLogger($"标题：{item.Title},查无重搜索第 { retryNonefind} 次");
                                }
                                else if (r == BaiduResponseResult.IpBlackIntercept)
                                {
                                    retryCount++;
                                    WriteLogger($"标题：{item.Title},IP重新搜索第 { retryCount} 次");
                                }
                                if (r == BaiduResponseResult.Included)
                                {
                                    action(item);
                                }
                                return r;
                            });
                        //var result = baidu.IsBaiduRecord(item.Title);                        
                        item.Result = result;
                        WriteLogger($"标题：{item.Title},收录情况:{result.GetDescription()}");
                        Thread.Sleep(300);
                    }
                    catch (Exception e)
                    {
                        WriteLogger($"查询异常请将异常发送给管理员，{e.Message}");
                        continue;
                    }
                }
            }
        }
        #endregion

        #region 5. 锚点更新
        private void bt_anchor_Click(object sender, EventArgs e)
        {
            if (!isHasFilePath())
            {
                return;
            }
            SetControllerEnable(false);
            InsertAnchors();

        }
        private void InsertAnchors()
        {
            Task.Run(() =>
            {
                var accounts = GetAccounts();
                if (accounts == null || !accounts.Any())
                {
                    return;
                }
                WriteLogger($"读取账号数量{accounts.Count}");
                try
                {
                    using (var crawle = new NewCrawle())
                    {

                        foreach (var account in accounts)
                        {

                            if (!crawle.Login(account.UserName, account.Password))
                            {
                                WriteLogger("登录失败");
                                continue;
                            };

                            var artcles = crawle.GetArticleUrls(MenuType.news_list, -1, account.StartPaged, account.EndPaged);
                            if (!artcles.Any())
                            {
                                WriteLogger("文章加载失败");
                                continue;
                            }
                            WriteLogger($"账号：{account.UserName}，获取文章数:{artcles.Count}");
                            int index = 0;

                            foreach (var artcle in artcles)
                            {
                                try
                                {
                                    crawle.InsertAnchor(artcle, account.GetAnchorContent(index++));
                                    WriteLogger($"已插入第{index}条,地址：{artcle}");
                                }
                                catch (Exception e)
                                {
                                    WriteLogger($"第{index}条插入失败,地址：{artcle}，{e.Message},{e.StackTrace}");
                                    continue;
                                }
                            }
                            WriteLogger("插入锚点结束");
                        }
                    }
                    WriteLogger($"执行结束");
                    SetControllerEnable(true);
                }
                catch (Exception e)
                {
                    WriteLogger($"浏览器被意外关闭，请重新启动任务，{e.Message},{e.StackTrace}");
                }
            });
        }

        #endregion

        #region 6. 更新手机
        private void btn_phone_Click(object sender, EventArgs e)
        {

            if (!isHasFilePath())
            {
                return;
            }

            var accounts = GetAccounts();
            if (accounts == null || !accounts.Any())
            {
                return;
            }
            SetControllerEnable(false);

            Task.Run(() =>
            {
                WriteLogger($"开始修改手机");
                using (var crawle = new NewCrawle())
                {
                    foreach (var account in accounts)
                    {
                        if (!crawle.Login(account.UserName, account.Password))
                        {
                            WriteLogger("登录失败");
                            continue;
                        };
                        crawle.UpdatePhone(account.Phone);
                        WriteLogger($"{account?.Company}修改手机成功");
                    }
                    SetControllerEnable(true);
                }

            });


        }

        #endregion

        #region 7. 友情链接

        private void btn_link_Click(object sender, EventArgs e)
        {
            if (!isHasFilePath())
            {
                return;
            }
            var accounts = GetAccounts(() => TxtHelper.GetLinkAccounts(FilePath));
            if (accounts == null || !accounts.Any())
            {
                return;
            }
            SetControllerEnable(false);
            Task.Run(() =>
            {
                WriteLogger($"开始添加友情链接");
                try
                {
                    using (var crawle = new NewCrawle())
                    {
                        foreach (var account in accounts)
                        {
                            if (!crawle.Login(account.UserName, account.Password))
                            {
                                WriteLogger("登录失败");
                                continue;
                            };
                            WriteLogger($"{account?.Company}添加链接数量 {account.LinkUrl.Count()}");
                            crawle.AddLinkList(account.LinkUrl);
                            WriteLogger($"{account?.Company}添加链接完成");
                        }
                        SetControllerEnable(true);
                    }
                }
                catch (Exception ex)
                {
                    WriteLogger($"链接添加异常，请将错误复制给管理员{ex.Message},{ex.StackTrace}");
                    throw;
                }
            });
        }
        #endregion

        #endregion

        private void btn_function_setting_Click(object sender, EventArgs e)
        {
            if (!isHasFilePath())
            {
                return;
            }
            var accounts = GetAccounts(() => TxtHelper.GetLinkAccounts(FilePath));
            if (accounts == null || !accounts.Any())
            {
                return;
            }
            SetControllerEnable(false);
            Task.Run(() =>
            {
                WriteLogger($"开始功能设置");
                try
                {
                    using (var crawle = new NewCrawle())
                    {
                        foreach (var account in accounts)
                        {
                            if (!crawle.Login(account.UserName, account.Password))
                            {
                                WriteLogger("登录失败");
                                continue;
                            };
                            WriteLogger($"{account?.Company}功能设置");
                            crawle.ChangeFunctionSetting(account.IsCopy, account.IsApp);
                            WriteLogger($"{account?.Company}功能设置完成");
                        }
                        SetControllerEnable(true);
                    }
                }
                catch (Exception ex)
                {
                    WriteLogger($"链接添加异常，请将错误复制给管理员{ex.Message},{ex.StackTrace}");
                    throw;
                }
            });
        }
    }

}
