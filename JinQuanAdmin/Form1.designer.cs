namespace JinQuanAdmin
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt_log = new System.Windows.Forms.TextBox();
            this.btn_file = new System.Windows.Forms.Button();
            this.gb_ck_menu = new System.Windows.Forms.GroupBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_setting = new System.Windows.Forms.Button();
            this.dgv_replace = new System.Windows.Forms.DataGridView();
            this.c_key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.c_value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_post = new System.Windows.Forms.Button();
            this.cb_removeTitle = new System.Windows.Forms.CheckBox();
            this.btn_refresh = new System.Windows.Forms.Button();
            this.txt_ptoxy = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_anchor = new System.Windows.Forms.Button();
            this.btn_pic = new System.Windows.Forms.Button();
            this.btn_log = new System.Windows.Forms.Button();
            this.btn_phone = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.gb_ck_menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_replace)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txt_log);
            this.groupBox1.Location = new System.Drawing.Point(385, 24);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(777, 373);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "日志";
            // 
            // txt_log
            // 
            this.txt_log.Location = new System.Drawing.Point(15, 22);
            this.txt_log.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txt_log.Multiline = true;
            this.txt_log.Name = "txt_log";
            this.txt_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_log.Size = new System.Drawing.Size(756, 327);
            this.txt_log.TabIndex = 0;
            this.txt_log.TextChanged += new System.EventHandler(this.txt_log_TextChanged);
            // 
            // btn_file
            // 
            this.btn_file.Location = new System.Drawing.Point(21, 24);
            this.btn_file.Margin = new System.Windows.Forms.Padding(4);
            this.btn_file.Name = "btn_file";
            this.btn_file.Size = new System.Drawing.Size(329, 43);
            this.btn_file.TabIndex = 2;
            this.btn_file.Text = "选择文件";
            this.btn_file.UseVisualStyleBackColor = true;
            this.btn_file.Click += new System.EventHandler(this.Button1_Click);
            // 
            // gb_ck_menu
            // 
            this.gb_ck_menu.Controls.Add(this.checkBox3);
            this.gb_ck_menu.Controls.Add(this.checkBox2);
            this.gb_ck_menu.Controls.Add(this.checkBox1);
            this.gb_ck_menu.Location = new System.Drawing.Point(21, 85);
            this.gb_ck_menu.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gb_ck_menu.Name = "gb_ck_menu";
            this.gb_ck_menu.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gb_ck_menu.Size = new System.Drawing.Size(200, 102);
            this.gb_ck_menu.TabIndex = 3;
            this.gb_ck_menu.TabStop = false;
            this.gb_ck_menu.Text = "栏目";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(18, 74);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(89, 19);
            this.checkBox3.TabIndex = 5;
            this.checkBox3.Tag = "3";
            this.checkBox3.Text = "供求信息";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(18, 49);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(89, 19);
            this.checkBox2.TabIndex = 4;
            this.checkBox2.Tag = "2";
            this.checkBox2.Text = "公司新闻";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(18, 25);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(89, 19);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Tag = "1";
            this.checkBox1.Text = "产品展示";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(20, 203);
            this.btn_save.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(92, 48);
            this.btn_save.TabIndex = 4;
            this.btn_save.Text = "自动发布";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // btn_setting
            // 
            this.btn_setting.Location = new System.Drawing.Point(248, 159);
            this.btn_setting.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_setting.Name = "btn_setting";
            this.btn_setting.Size = new System.Drawing.Size(119, 28);
            this.btn_setting.TabIndex = 5;
            this.btn_setting.Text = "设置浏览器";
            this.btn_setting.UseVisualStyleBackColor = true;
            this.btn_setting.Click += new System.EventHandler(this.btn_setting_Click);
            // 
            // dgv_replace
            // 
            this.dgv_replace.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_replace.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_replace.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.c_key,
            this.c_value});
            this.dgv_replace.Location = new System.Drawing.Point(21, 421);
            this.dgv_replace.Margin = new System.Windows.Forms.Padding(4);
            this.dgv_replace.Name = "dgv_replace";
            this.dgv_replace.RowHeadersWidth = 51;
            this.dgv_replace.RowTemplate.Height = 23;
            this.dgv_replace.Size = new System.Drawing.Size(1149, 272);
            this.dgv_replace.TabIndex = 6;
            // 
            // c_key
            // 
            this.c_key.HeaderText = "替换前";
            this.c_key.MinimumWidth = 6;
            this.c_key.Name = "c_key";
            // 
            // c_value
            // 
            this.c_value.HeaderText = "替换后";
            this.c_value.MinimumWidth = 6;
            this.c_value.Name = "c_value";
            // 
            // btn_post
            // 
            this.btn_post.Location = new System.Drawing.Point(133, 203);
            this.btn_post.Name = "btn_post";
            this.btn_post.Size = new System.Drawing.Size(95, 48);
            this.btn_post.TabIndex = 7;
            this.btn_post.Text = "替换发布";
            this.btn_post.UseVisualStyleBackColor = true;
            this.btn_post.Click += new System.EventHandler(this.btn_post_Click);
            // 
            // cb_removeTitle
            // 
            this.cb_removeTitle.AutoSize = true;
            this.cb_removeTitle.Location = new System.Drawing.Point(248, 94);
            this.cb_removeTitle.Name = "cb_removeTitle";
            this.cb_removeTitle.Size = new System.Drawing.Size(119, 19);
            this.cb_removeTitle.TabIndex = 8;
            this.cb_removeTitle.Text = "发布忽略标题";
            this.cb_removeTitle.UseVisualStyleBackColor = true;
            // 
            // btn_refresh
            // 
            this.btn_refresh.Location = new System.Drawing.Point(19, 270);
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Size = new System.Drawing.Size(93, 46);
            this.btn_refresh.TabIndex = 9;
            this.btn_refresh.Text = "收录更新";
            this.btn_refresh.UseVisualStyleBackColor = true;
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_Click);
            // 
            // txt_ptoxy
            // 
            this.txt_ptoxy.Location = new System.Drawing.Point(94, 348);
            this.txt_ptoxy.Name = "txt_ptoxy";
            this.txt_ptoxy.Size = new System.Drawing.Size(256, 25);
            this.txt_ptoxy.TabIndex = 10;
            this.txt_ptoxy.Text = "tps134.kdlapi.com:15818";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 351);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "代理地址";
            // 
            // btn_anchor
            // 
            this.btn_anchor.Location = new System.Drawing.Point(133, 270);
            this.btn_anchor.Name = "btn_anchor";
            this.btn_anchor.Size = new System.Drawing.Size(95, 46);
            this.btn_anchor.TabIndex = 12;
            this.btn_anchor.Text = "锚点更新";
            this.btn_anchor.UseVisualStyleBackColor = true;
            this.btn_anchor.Click += new System.EventHandler(this.bt_anchor_Click);
            // 
            // btn_pic
            // 
            this.btn_pic.Location = new System.Drawing.Point(255, 203);
            this.btn_pic.Name = "btn_pic";
            this.btn_pic.Size = new System.Drawing.Size(92, 48);
            this.btn_pic.TabIndex = 13;
            this.btn_pic.Text = "爬取锚图";
            this.btn_pic.UseVisualStyleBackColor = true;
            this.btn_pic.Click += new System.EventHandler(this.btn_pic_Click);
            // 
            // btn_log
            // 
            this.btn_log.Location = new System.Drawing.Point(248, 127);
            this.btn_log.Name = "btn_log";
            this.btn_log.Size = new System.Drawing.Size(119, 26);
            this.btn_log.TabIndex = 14;
            this.btn_log.Text = "导出日志";
            this.btn_log.UseVisualStyleBackColor = true;
            this.btn_log.Click += new System.EventHandler(this.btn_log_Click);
            // 
            // btn_phone
            // 
            this.btn_phone.Location = new System.Drawing.Point(255, 270);
            this.btn_phone.Name = "btn_phone";
            this.btn_phone.Size = new System.Drawing.Size(92, 46);
            this.btn_phone.TabIndex = 17;
            this.btn_phone.Text = "更新手机";
            this.btn_phone.UseVisualStyleBackColor = true;
            this.btn_phone.Click += new System.EventHandler(this.btn_phone_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1202, 718);
            this.Controls.Add(this.btn_phone);
            this.Controls.Add(this.btn_log);
            this.Controls.Add(this.btn_pic);
            this.Controls.Add(this.btn_anchor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_ptoxy);
            this.Controls.Add(this.btn_refresh);
            this.Controls.Add(this.cb_removeTitle);
            this.Controls.Add(this.btn_post);
            this.Controls.Add(this.dgv_replace);
            this.Controls.Add(this.btn_setting);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.gb_ck_menu);
            this.Controls.Add(this.btn_file);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "替换";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gb_ck_menu.ResumeLayout(false);
            this.gb_ck_menu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_replace)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_log;
        private System.Windows.Forms.Button btn_file;
        private System.Windows.Forms.GroupBox gb_ck_menu;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_setting;
        private System.Windows.Forms.DataGridView dgv_replace;
        private System.Windows.Forms.DataGridViewTextBoxColumn c_key;
        private System.Windows.Forms.DataGridViewTextBoxColumn c_value;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btn_post;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox cb_removeTitle;
        private System.Windows.Forms.Button btn_refresh;
        private System.Windows.Forms.TextBox txt_ptoxy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_anchor;
        private System.Windows.Forms.Button btn_pic;
        private System.Windows.Forms.Button btn_log;
        private System.Windows.Forms.Button btn_phone;
    }
}

