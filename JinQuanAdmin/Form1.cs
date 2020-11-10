using JinQuanAdmin.Common;
using JinQuanAdmin.Crawler;
using JinQuanAdmin.Model;
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
        public Form1()
        {
            InitializeComponent();

        }

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

        HashSet<MenuType> menuTypesSets = new HashSet<MenuType>();
        private void Ck_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ck = sender as CheckBox;
            MenuType type = (MenuType)Convert.ToInt32(ck.Tag);
            if (ck.Checked)
            {
                WriteLogger("选择栏目：" + ck.Text);
                menuTypesSets.Add(type);

            }
            else
            {
                menuTypesSets.Remove(type);
                WriteLogger("取消选择栏目：" + ck.Text);
            }
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

        #region WebWrowser  操作




        /// <summary>
        /// 延时发布
        /// </summary>
        private void DelayPost(int m = 0)
        {
            if (m == 0)
            {
                Random random = new Random();

                m = random.Next(10, 21);
                WriteLogger($"等待发布中，{m}秒");
            }
            DateTime current = DateTime.Now;
            while (current.AddMilliseconds(m * 1000) > DateTime.Now)
            {
                Application.DoEvents();
            }
            return;
        }

        #endregion

        private string UserName { get; set; }
        private string Password { get; set; }
        private List<Article> Articles { get; set; }

        #region 控件事件

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
        public static bool IsIgnoreTitle;
        private void ReplaceArticles()
        {
            if (ReplaceDic.Count > 0)
            {
                foreach (var item in Articles)
                {
                    foreach (var dic in ReplaceDic)
                    {
                        item.Content = item.Content.Replace(dic.Key, dic.Value);
                    }
                }
            }
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

        string FilePath = "";
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

            if (menuTypesSets.Count != 1)
            {
                MessageBox.Show("请选择一个栏目,不支持多选", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        #endregion

        #region 控件委托修改

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

        #endregion

        #region 执行方法

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

                        var urlList = crawler.GetArticleUrls(menuTypesSets.First(), Articles.Count);
                        if (!urlList.Any())
                        {
                            WriteLogger("文章加载失败");
                            return;
                        }

                        for (int i = 0; i < Articles.Count; i++)
                        {
                            if (i < urlList.Count)
                            {
                                crawler.SubmitArticle(menuTypesSets.First(), urlList[i], Articles[i]);
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

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


                    foreach (var item in menuTypesSets)
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
        private string proxt_address = "";

        /// <summary>
        /// 收录更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_refresh_Click(object sender, EventArgs e)
        {
            if (menuTypesSets.Count != 1 && (menuTypesSets.FirstOrDefault() != MenuType.WholesaleList || menuTypesSets.FirstOrDefault() != MenuType.WholesaleList))
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
            proxt_address = txt_ptoxy.Text.Trim();
            SetControllerEnable(false);
            RefreshSetTop(accounts);
        }


        public string GetNewPath(string fileName)
        {
            int index = FilePath.LastIndexOf(".");

            string name = FilePath.Insert(index, fileName);
            return name;
        }


        private void RefreshSetTop(List<Account> accounts)
        {

            Task.Run(() =>
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

                            int total;
                            int pageTotal;
                            var list = crawle.GetArticlesTitles(menuTypesSets.First(), -1, out total, out pageTotal);
                            if (!list.Any())
                            {
                                WriteLogger("没有获取到文章");
                                return;
                            }
                            Retry = 0;
                            BaiduSearch(proxt_address, list);
                            var topList = list.Where(s => s.IsIncluded).ToList();
                            string includedMessage = "";
                            if (topList == null || !topList.Any())
                            {
                                includedMessage = $"栏目：{menuTypesSets.First().GetDescription()}，收录文章数：{0},未收录文章数量：{total},未收录页数第：{1}--{pageTotal}";
                                account.Included = includedMessage;
                                WriteLogger(includedMessage);
                                return;
                            }
                            int count = topList.Count;
                            WriteLogger($"收录文章数:{count},开始刷新置顶");
                            crawle.RefreshSetTop(topList);
                            int needPage = (count + 16) / 16;

                            includedMessage = $"栏目：{menuTypesSets.First().GetDescription()}，收录文章数：{count},未收录文章数量：{total - count},未收录页数第：{needPage}--{pageTotal}";
                            account.Included = includedMessage;

                            WriteLogger(includedMessage);
                            WriteTxt(filePath, account);
                        }
                        WriteLogger($"已导出文件{filePath}");
                        WriteLogger($"执行结束");
                        SetControllerEnable(true);
                    }
                });
        }
        private static int Retry = 0;

        private void BaiduSearch(string proxy, List<ArticleTitle> articles)
        {
            using (var baidu = new NewCrawle(proxy))
            {
                try
                {
                    foreach (var item in articles)
                    {
                        item.IsIncluded = baidu.IsBaiduRecord(item.Title);
                        string isIncluded = item.IsIncluded ? "有" : "无";
                        WriteLogger($"标题：{item.Title},收录情况:{isIncluded}");
                        Thread.Sleep(300);
                    }

                }
                catch (Exception e)
                {

                    WriteLogger($"百度反扒虫，{e.Message}");
                    Retry++;
                    if (Retry > 3)
                    {
                        return;
                    }
                    else
                    {
                        BaiduSearch(proxy, articles);
                    }

                }
            }
        }

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
        /// 锚点更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            Task.Run((Action)(() =>
            {
                var accounts = GetAccounts();
                if (accounts == null || !accounts.Any())
                {
                    return;
                }
                WriteLogger($"读取账号数量{accounts.Count}");
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
                            WriteLogger($"地址：{artcle}，开始插入锚点");
                            crawle.InsertAnchor(artcle, account.GetAnchorContent(index++));
                        }
                        WriteLogger("插入锚点结束");
                    }
                }
                WriteLogger($"执行结束");
                SetControllerEnable(true);

            }));
        }

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

        public void WriteTxt(string path, Account account)
        {
            var bytes = System.Text.Encoding.Default.GetBytes(account.ToString());
            using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                stream.Write(bytes, 0, bytes.Length);
            }

        }

        private Task CreateCrawle(List<Account> accounts, Action<List<Account>> action)
        {
            return Task.Run(() =>
            {
                using (var crawle = new NewCrawle())
                {
                    try
                    {
                        action(accounts);
                    }
                    catch (Exception ex)
                    {
                        WriteLogger($"执行出错，{ex.Message},{ex.StackTrace}");
                        return;
                    }
                }

            });
        }

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

        int retry = 0;
        private bool Login(NewCrawle crawle, string userName, string password)
        {
            if (!crawle.Login(userName, userName))
            {
                retry++;
                if (retry < 3)
                {
                    Login(crawle, userName, password);
                }
                WriteLogger("登录失败");
                return false;
            };
            return true;
        }
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


    }

}
