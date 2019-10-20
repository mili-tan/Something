namespace LitchiGetSyllabus
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
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.semester = new System.Windows.Forms.NumericUpDown();
            this.week = new System.Windows.Forms.NumericUpDown();
            this.yr2 = new System.Windows.Forms.NumericUpDown();
            this.yr1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.MakeTransparent = new System.Windows.Forms.CheckBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.semester)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.week)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yr2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yr1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button1.Enabled = false;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.button1.Location = new System.Drawing.Point(0, 520);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(982, 33);
            this.button1.TabIndex = 1;
            this.button1.Text = "开始采集";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.semester, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.week, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.yr2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(129, -3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(248, 112);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // semester
            // 
            this.semester.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.semester.Location = new System.Drawing.Point(3, 42);
            this.semester.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.semester.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.semester.Name = "semester";
            this.semester.Size = new System.Drawing.Size(118, 27);
            this.semester.TabIndex = 0;
            this.semester.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // week
            // 
            this.week.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.week.Location = new System.Drawing.Point(3, 79);
            this.week.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.week.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.week.Name = "week";
            this.week.Size = new System.Drawing.Size(118, 27);
            this.week.TabIndex = 1;
            this.week.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // yr2
            // 
            this.yr2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.yr2.Location = new System.Drawing.Point(3, 5);
            this.yr2.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.yr2.Name = "yr2";
            this.yr2.Size = new System.Drawing.Size(118, 27);
            this.yr2.TabIndex = 2;
            this.yr2.Value = new decimal(new int[] {
            2020,
            0,
            0,
            0});
            // 
            // yr1
            // 
            this.yr1.Location = new System.Drawing.Point(3, 3);
            this.yr1.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.yr1.Name = "yr1";
            this.yr1.Size = new System.Drawing.Size(123, 27);
            this.yr1.TabIndex = 3;
            this.yr1.Value = new decimal(new int[] {
            2019,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(127, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "学年";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(127, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 23);
            this.label2.TabIndex = 4;
            this.label2.Text = "学期";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(127, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 23);
            this.label3.TabIndex = 5;
            this.label3.Text = "周";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.yr1);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.panel1.Location = new System.Drawing.Point(45, 44);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(377, 107);
            this.panel1.TabIndex = 4;
            // 
            // MakeTransparent
            // 
            this.MakeTransparent.AutoSize = true;
            this.MakeTransparent.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.MakeTransparent.Location = new System.Drawing.Point(45, 158);
            this.MakeTransparent.Name = "MakeTransparent";
            this.MakeTransparent.Size = new System.Drawing.Size(136, 24);
            this.MakeTransparent.TabIndex = 5;
            this.MakeTransparent.Text = "透明化图片背景";
            this.MakeTransparent.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(45, 188);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(377, 124);
            this.listBox1.TabIndex = 6;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(45, 319);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(377, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 553);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.MakeTransparent);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.semester)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.week)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yr2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yr1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown semester;
        private System.Windows.Forms.NumericUpDown week;
        private System.Windows.Forms.NumericUpDown yr2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown yr1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox MakeTransparent;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}

