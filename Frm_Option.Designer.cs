namespace 柒幻_千纸鹤
{
    partial class Frm_Option
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
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Ck_show_copy_code_response = new System.Windows.Forms.CheckBox();
            this.Ck_show_scrollbar = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.Btn_Cancel = new System.Windows.Forms.Button();
            this.Btn_Confirm = new System.Windows.Forms.Button();
            this.Ck_show_accept_tab = new System.Windows.Forms.CheckBox();
            this.Ck_show_code_in_one_line = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.Controls.Add(this.TabControl);
            this.flowLayoutPanel.Controls.Add(this.flowLayoutPanel2);
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(789, 399);
            this.flowLayoutPanel.TabIndex = 2;
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.tabPage1);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.HotTrack = true;
            this.TabControl.Location = new System.Drawing.Point(3, 3);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(783, 330);
            this.TabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.Ck_show_code_in_one_line);
            this.tabPage1.Controls.Add(this.Ck_show_accept_tab);
            this.tabPage1.Controls.Add(this.Ck_show_copy_code_response);
            this.tabPage1.Controls.Add(this.Ck_show_scrollbar);
            this.tabPage1.Location = new System.Drawing.Point(8, 39);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(767, 283);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "通用";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // Ck_show_copy_code_response
            // 
            this.Ck_show_copy_code_response.AutoSize = true;
            this.Ck_show_copy_code_response.Checked = true;
            this.Ck_show_copy_code_response.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Ck_show_copy_code_response.Location = new System.Drawing.Point(15, 115);
            this.Ck_show_copy_code_response.Name = "Ck_show_copy_code_response";
            this.Ck_show_copy_code_response.Size = new System.Drawing.Size(210, 28);
            this.Ck_show_copy_code_response.TabIndex = 1;
            this.Ck_show_copy_code_response.Text = "复制代码后提示";
            this.Ck_show_copy_code_response.UseVisualStyleBackColor = true;
            // 
            // Ck_show_scrollbar
            // 
            this.Ck_show_scrollbar.AutoSize = true;
            this.Ck_show_scrollbar.Checked = true;
            this.Ck_show_scrollbar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Ck_show_scrollbar.Location = new System.Drawing.Point(15, 15);
            this.Ck_show_scrollbar.Name = "Ck_show_scrollbar";
            this.Ck_show_scrollbar.Size = new System.Drawing.Size(306, 28);
            this.Ck_show_scrollbar.TabIndex = 0;
            this.Ck_show_scrollbar.Text = "设计模式显示垂直滚动条";
            this.Ck_show_scrollbar.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.Btn_Cancel);
            this.flowLayoutPanel2.Controls.Add(this.Btn_Confirm);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 339);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.flowLayoutPanel2.Size = new System.Drawing.Size(783, 46);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // Btn_Cancel
            // 
            this.Btn_Cancel.AutoSize = true;
            this.Btn_Cancel.Location = new System.Drawing.Point(600, 3);
            this.Btn_Cancel.Name = "Btn_Cancel";
            this.Btn_Cancel.Size = new System.Drawing.Size(170, 40);
            this.Btn_Cancel.TabIndex = 0;
            this.Btn_Cancel.Text = "取消";
            this.Btn_Cancel.UseVisualStyleBackColor = true;
            this.Btn_Cancel.Click += new System.EventHandler(this.Btn_Cancel_Click);
            // 
            // Btn_Confirm
            // 
            this.Btn_Confirm.AutoSize = true;
            this.Btn_Confirm.Location = new System.Drawing.Point(424, 3);
            this.Btn_Confirm.Name = "Btn_Confirm";
            this.Btn_Confirm.Size = new System.Drawing.Size(170, 40);
            this.Btn_Confirm.TabIndex = 1;
            this.Btn_Confirm.Text = "确认";
            this.Btn_Confirm.UseVisualStyleBackColor = true;
            this.Btn_Confirm.Click += new System.EventHandler(this.Btn_Confirm_Click);
            // 
            // Ck_show_accept_tab
            // 
            this.Ck_show_accept_tab.AutoSize = true;
            this.Ck_show_accept_tab.Checked = true;
            this.Ck_show_accept_tab.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Ck_show_accept_tab.Location = new System.Drawing.Point(15, 65);
            this.Ck_show_accept_tab.Name = "Ck_show_accept_tab";
            this.Ck_show_accept_tab.Size = new System.Drawing.Size(222, 28);
            this.Ck_show_accept_tab.TabIndex = 2;
            this.Ck_show_accept_tab.Text = "内容模式接受TAB";
            this.Ck_show_accept_tab.UseVisualStyleBackColor = true;
            // 
            // Ck_show_code_in_one_line
            // 
            this.Ck_show_code_in_one_line.AutoSize = true;
            this.Ck_show_code_in_one_line.Checked = true;
            this.Ck_show_code_in_one_line.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Ck_show_code_in_one_line.Location = new System.Drawing.Point(15, 165);
            this.Ck_show_code_in_one_line.Name = "Ck_show_code_in_one_line";
            this.Ck_show_code_in_one_line.Size = new System.Drawing.Size(210, 28);
            this.Ck_show_code_in_one_line.TabIndex = 3;
            this.Ck_show_code_in_one_line.Text = "合并生成代码行";
            this.Ck_show_code_in_one_line.UseVisualStyleBackColor = true;
            // 
            // Frm_Option
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 399);
            this.Controls.Add(this.flowLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_Option";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "选项";
            this.Load += new System.EventHandler(this.Frm_Option_Load);
            this.flowLayoutPanel.ResumeLayout(false);
            this.TabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button Btn_Cancel;
        private System.Windows.Forms.Button Btn_Confirm;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox Ck_show_scrollbar;
        private System.Windows.Forms.CheckBox Ck_show_copy_code_response;
        private System.Windows.Forms.CheckBox Ck_show_code_in_one_line;
        private System.Windows.Forms.CheckBox Ck_show_accept_tab;
    }
}