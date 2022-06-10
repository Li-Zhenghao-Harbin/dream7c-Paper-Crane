using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 柒幻_千纸鹤
{
    public partial class Frm_Option : Form
    {
        public Frm_Option()
        {
            InitializeComponent();
        }

        protected string folder_path;

        private void Frm_Option_Load(object sender, EventArgs e)
        {
            Ck_show_scrollbar.Checked = Frm_Main.showScrollbars;
            Ck_show_accept_tab.Checked = Frm_Main.acceptTab;
            Ck_show_copy_code_response.Checked = Frm_Main.showCopyCodeResponse;
            Ck_show_code_in_one_line.Checked = Frm_Main.codeInOneLine;
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Btn_Confirm_Click(object sender, EventArgs e)
        {
            Frm_Main.rtxss.ForEach(rtx => rtx.ScrollBars = Ck_show_scrollbar.Checked ? RichTextBoxScrollBars.Both : RichTextBoxScrollBars.None);
            Frm_Main.showScrollbars = Ck_show_scrollbar.Checked;
            Frm_Main.rtxContent.AcceptsTab = Frm_Main.acceptTab = Ck_show_accept_tab.Checked;
            Frm_Main.showCopyCodeResponse = Ck_show_copy_code_response.Checked;
            Frm_Main.codeInOneLine = Ck_show_code_in_one_line.Checked;
            Frm_Main frm_Main = new Frm_Main();
            frm_Main.SaveConfig();
            Close();
        }
    }
}
