namespace DataCollectWinform
{
    partial class FrmCollect
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCollect));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.TsBTnStart = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TsBtnClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.TbTime = new System.Windows.Forms.ToolStripTextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.listEqmInfo = new DataCollectWinform.ListViewNF();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsBTnStart,
            this.toolStripSeparator1,
            this.TsBtnClose,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.TbTime});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(960, 70);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // TsBTnStart
            // 
            this.TsBTnStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.TsBTnStart.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TsBTnStart.Image = ((System.Drawing.Image)(resources.GetObject("TsBTnStart.Image")));
            this.TsBTnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsBTnStart.Name = "TsBTnStart";
            this.TsBTnStart.Size = new System.Drawing.Size(82, 67);
            this.TsBTnStart.Text = "启动";
            this.TsBTnStart.Click += new System.EventHandler(this.TsBTnStart_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 70);
            // 
            // TsBtnClose
            // 
            this.TsBtnClose.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TsBtnClose.Image = ((System.Drawing.Image)(resources.GetObject("TsBtnClose.Image")));
            this.TsBtnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsBtnClose.Name = "TsBtnClose";
            this.TsBtnClose.Size = new System.Drawing.Size(82, 67);
            this.TsBtnClose.Text = "关闭";
            this.TsBtnClose.Visible = false;
            this.TsBtnClose.Click += new System.EventHandler(this.TsBtnClose_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 70);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(86, 67);
            this.toolStripLabel1.Text = "运行时间:";
            // 
            // TbTime
            // 
            this.TbTime.Name = "TbTime";
            this.TbTime.ReadOnly = true;
            this.TbTime.Size = new System.Drawing.Size(200, 70);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.listEqmInfo, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 70);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.25641F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.74359F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(960, 663);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(952, 67);
            this.label1.TabIndex = 1;
            this.label1.Text = "设备信息";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // listEqmInfo
            // 
            this.listEqmInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listEqmInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listEqmInfo.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.listEqmInfo.FullRowSelect = true;
            this.listEqmInfo.GridLines = true;
            this.listEqmInfo.Location = new System.Drawing.Point(3, 70);
            this.listEqmInfo.Name = "listEqmInfo";
            this.listEqmInfo.Size = new System.Drawing.Size(954, 590);
            this.listEqmInfo.TabIndex = 2;
            this.listEqmInfo.UseCompatibleStateImageBehavior = false;
            this.listEqmInfo.View = System.Windows.Forms.View.Details;
            // 
            // FrmCollect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 733);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmCollect";
            this.Text = "采集下发程序";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmCollect_FormClosing);
            this.Load += new System.EventHandler(this.FrmCollect_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton TsBTnStart;
        private System.Windows.Forms.ToolStripButton TsBtnClose;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private ListViewNF listEqmInfo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox TbTime;
        private System.Windows.Forms.Timer timer1;
    }
}