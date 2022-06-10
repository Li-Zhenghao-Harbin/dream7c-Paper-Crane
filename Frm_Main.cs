using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace 柒幻_千纸鹤
{
    public partial class Frm_Main : Form
    {
        public Frm_Main()
        {
            InitializeComponent();
        }

        //Load
        private bool firstLoadForm = true;
        //Config
        public static bool showScrollbars;
        public static bool acceptTab;
        public static bool showCopyCodeResponse;
        public static bool codeInOneLine;
        //Layout
        private readonly bool[] attributesLayoutDisplay = new bool[4];
        //Blocks
        private const int maxBlocksCount = 36;
        private const int maxRowAndColumnCount = 6;
        private readonly RichTextBox[,] rtxs = new RichTextBox[maxRowAndColumnCount, maxRowAndColumnCount];
        public static List<RichTextBox> rtxss = new List<RichTextBox>(maxBlocksCount);
        private readonly Panel[,] blocks = new Panel[maxRowAndColumnCount, maxRowAndColumnCount];
        //Select
        private bool pressShift = false;
        private bool pressCtrl = false;
        private readonly bool[,] selectedBlocks = new bool[maxRowAndColumnCount, maxRowAndColumnCount];
        private int selectedCount = 1;
        private readonly Color selectedColor = Color.FromArgb(153, 180, 209);
        private readonly Color commonColor = Color.FromArgb(240, 240, 240);
        //Attributes
        private readonly double[] blockWidth = new double[maxBlocksCount];
        private readonly double[] blockHeight = new double[maxBlocksCount];
        private readonly bool[] blockFontBold = new bool[maxBlocksCount];
        private readonly bool[] blockFontItalic = new bool[maxBlocksCount];
        private readonly bool[] blockFontUnderline = new bool[maxBlocksCount];
        private readonly Font[] blockFont = new Font[maxBlocksCount];
        private readonly string[] blockForeColor = new string[maxBlocksCount];
        private readonly string[] blockBackColor = new string[maxBlocksCount];
        private readonly int[] blockParagraphHeight = new int[maxBlocksCount];
        private readonly int[] blockAlign = new int[maxBlocksCount];
        //View type
        private bool viewType = false; //true: content; false: background
        public static RichTextBox rtxContent;
        //Mouse
        private int lastTargetRow = 0;
        private int lastTargetColumn = 0;
        //Map
        private readonly Button[,] map_blocks = new Button[maxRowAndColumnCount, maxRowAndColumnCount];
        //Row and column
        private int rowCount = 1;
        private int columnCount = 1;
        private int targetRow = 0;
        private int targetColumn = 0;
        //FAR
        public static int FAR_type;

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll ")]
        private static extern bool BitBlt(
            IntPtr hdcDest,
            int nXDest,
            int nYDest,
            int nWidth,
            int nHeight,
            IntPtr hdcSrc,
            int nXSrc,
            int nYSrc,
            System.Int32 dwRop
        );

        private void AutoResizeBlocks()
        {
            int width_background = Pn_background.Width;
            int height_background = Pn_background.Height;
            int width_map = Pn_map_background.Width;
            int height_map = Pn_map_background.Height;
            foreach (Panel b in blocks)
            {
                b.Visible = false;
            }
            foreach (Button b in map_blocks)
            {
                b.Visible = false;
            }
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    //blocks
                    blocks[i, j].Visible = true;
                    blocks[i, j].Size = new Size(width_background / columnCount, height_background / rowCount);
                    if (i > 0)
                    {
                        Panel b = blocks[i - 1, j];
                        blocks[i, j].Top = b.Top + b.Height;
                    }
                    if (j > 0)
                    {
                        Panel b = blocks[i, j - 1];
                        blocks[i, j].Left = b.Left + b.Width;
                    }
                    //map
                    map_blocks[i, j].Visible = true;
                    map_blocks[i, j].Size = new Size(width_map / columnCount, height_map / rowCount);
                    if (i > 0)
                    {
                        Button b = map_blocks[i - 1, j];
                        map_blocks[i, j].Top = b.Top + b.Height;
                    }
                    if (j > 0)
                    {
                        Button b = map_blocks[i, j - 1];
                        map_blocks[i, j].Left = b.Left + b.Width;
                    }
                }
            }
            SetStatus();
        }

        private void SetStatus()
        {
            statusStrip_lbl_display_row_column.Text = "共 " + rowCount + " 行 " + columnCount + " 列 当前选中：第 " + (targetRow + 1) + " 行 " + (targetColumn + 1) + " 列";
            selectedCount = 0;
            foreach (bool sb in selectedBlocks)
            {
                if (sb)
                {
                    selectedCount++;
                }
            }
            if (selectedCount > 1)
            {
                statusStrip_lbl_display_row_column.Text += " 等 " + selectedCount + " 项";
            }
        }

        private void SetProcess(bool ready)
        {
            if (ready)
            {
                statusStrip_lbl_ready.Text = "就绪";
                statusStrip_lbl_ready.ForeColor = Color.Green;
            }
            else
            {
                statusStrip_lbl_ready.Text = "执行中";
                statusStrip_lbl_ready.ForeColor = Color.OrangeRed;
            }
        }

        public void SaveConfig()
        {
            XmlDocument doc = new XmlDocument();
            string strFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            doc.Load(strFileName);
            XmlNodeList nodes = doc.GetElementsByTagName("add");
            for (int i = 1; i < nodes.Count; i++)
            {
                XmlAttribute att = nodes[i].Attributes["key"];
                switch (att.Value)
                {
                    case "FontName":
                        att = nodes[i].Attributes["value"];
                        att.Value = Combo_attributes_font_name.Text;
                        break;
                    case "FontSize":
                        att = nodes[i].Attributes["value"];
                        att.Value = Num_attributes_font_size.Value.ToString();
                        break;
                    case "Bold":
                        att = nodes[i].Attributes["value"];
                        att.Value = Ck_attributes_font_type_bold.Checked.ToString();
                        break;
                    case "Italic":
                        att = nodes[i].Attributes["value"];
                        att.Value = Ck_attributes_font_type_italic.Checked.ToString();
                        break;
                    case "Underline":
                        att = nodes[i].Attributes["value"];
                        att.Value = Ck_attributes_font_type_underline.Checked.ToString();
                        break;
                    case "ForeColor":
                        att = nodes[i].Attributes["value"];
                        att.Value = Tx_attributes_appearance_forecolor.Text;
                        break;
                    case "BackColor":
                        att = nodes[i].Attributes["value"];
                        att.Value = Tx_attributes_appearance_backcolor.Text;
                        break;
                    case "ParagraphHeight":
                        att = nodes[i].Attributes["value"];
                        att.Value = Num_attributes_paragraph_height.Value.ToString();
                        break;
                    case "Alignment":
                        att = nodes[i].Attributes["value"];
                        if (Radio_attributes_paragraph_align_left.Checked)
                        {
                            att.Value = "left";
                        }
                        else if (Radio_attributes_paragraph_align_mid.Checked)
                        {
                            att.Value = "mid";
                        }
                        else if (Radio_attributes_paragraph_align_right.Checked)
                        {
                            att.Value = "right";
                        }
                        break;
                    case "ShowScrollBar":
                        att = nodes[i].Attributes["value"];
                        att.Value = showScrollbars.ToString();
                        break;
                    case "ShowCopyCodeResponse":
                        att = nodes[i].Attributes["value"];
                        att.Value = showCopyCodeResponse.ToString();
                        break;
                    case "acceptTab":
                        att = nodes[i].Attributes["value"];
                        att.Value = showScrollbars.ToString();
                        break;
                    case "codeInOneRow":
                        att = nodes[i].Attributes["value"];
                        att.Value = showCopyCodeResponse.ToString();
                        break;
                }
            }
            doc.Save(strFileName);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private Color GetColorFromHtml(string str)
        {
            return ColorTranslator.FromHtml("#" + str);
        }

        private bool GetColorFromHtml(string str, out Color color)
        {
            color = default;
            try
            {
                color = ColorTranslator.FromHtml("#" + str);
                return true;
            }
            catch { }
            return false;
        }

        private void SetFontStyle(int i)
        {
            RichTextBox rtx = rtxss[i];
            if (blockFontBold[i] & blockFontItalic[i] & blockFontUnderline[i])
            {
                rtx.Font = new Font(rtx.Font, FontStyle.Bold ^ FontStyle.Italic ^ FontStyle.Underline);
            }
            else if (blockFontBold[i] & blockFontItalic[i] & !blockFontUnderline[i])
            {
                rtx.Font = new Font(rtx.Font, FontStyle.Bold ^ FontStyle.Italic);
            }
            else if (blockFontBold[i] & !blockFontItalic[i] & blockFontUnderline[i])
            {
                rtx.Font = new Font(rtx.Font, FontStyle.Bold ^ FontStyle.Underline);
            }
            else if (!blockFontBold[i] & blockFontItalic[i] & blockFontUnderline[i])
            {
                rtx.Font = new Font(rtx.Font, FontStyle.Italic ^ FontStyle.Underline);
            }
            else if (blockFontBold[i] & !blockFontItalic[i] & !blockFontUnderline[i])
            {
                rtx.Font = new Font(rtx.Font, FontStyle.Bold);
            }
            else if (!blockFontBold[i] & blockFontItalic[i] & !blockFontUnderline[i])
            {
                rtx.Font = new Font(rtx.Font, FontStyle.Italic);
            }
            else if (!blockFontBold[i] & !blockFontItalic[i] & blockFontUnderline[i])
            {
                rtx.Font = new Font(rtx.Font, FontStyle.Underline);
            }
            else if (!blockFontBold[i] & !blockFontItalic[i] & !blockFontUnderline[i])
            {
                rtx.Font = new Font(rtx.Font, FontStyle.Regular);
            }
        }

        private void GenerateBlocksAndMaps()
        {
            for (int i = 0; i < maxRowAndColumnCount; i++)
            {
                for (int j = 0; j < maxRowAndColumnCount; j++)
                {
                    int t = i * maxRowAndColumnCount + j;
                    //blocks
                    Panel pn_block = new Panel()
                    {
                        Name = "pn_block_" + t,
                        Tag = t,
                        BackColor = commonColor,
                        Padding = new Padding(5, 5, 5, 5),
                        Visible = false
                    };
                    Pn_background.Controls.Add(pn_block);
                    blocks[i, j] = pn_block;
                    RichTextBox rtx_block = new RichTextBox()
                    {
                        Name = "rtx_block" + t,
                        Tag = t,
                        Font = blockFont[t],
                        ForeColor = GetColorFromHtml(blockForeColor[t]),
                        BackColor = GetColorFromHtml(blockBackColor[t]),
                        ScrollBars = showScrollbars ? RichTextBoxScrollBars.Both : RichTextBoxScrollBars.None,
                        BorderStyle = BorderStyle.None,
                        Dock = DockStyle.Fill,
                        LanguageOption = RichTextBoxLanguageOptions.UIFonts,
                        DetectUrls = false
                    };
                    rtxss.Add(rtx_block);
                    SetFontStyle(t);
                    rtx_block.SelectAll();
                    switch (blockAlign[t])
                    {
                        case 0:
                            rtx_block.SelectionAlignment = HorizontalAlignment.Left;
                            break;
                        case 1:
                            rtx_block.SelectionAlignment = HorizontalAlignment.Center;
                            break;
                        case 2:
                            rtx_block.SelectionAlignment = HorizontalAlignment.Right;
                            break;
                    }
                    pn_block.Controls.Add(rtx_block);
                    rtx_block.Click += Rtx_block_Click;
                    rtx_block.DoubleClick += Rtx_block_DoubleClick;
                    rtx_block.KeyDown += Rtx_block_KeyDown;
                    rtx_block.KeyUp += Rtx_block_KeyUp;
                    rtxs[i, j] = rtx_block;
                    //maps
                    Button btn_map = new Button()
                    {
                        Name = "btn_map" + t,
                        Tag = t,
                        BackColor = commonColor,
                        Visible = false
                    };
                    Pn_map_background.Controls.Add(btn_map);
                    map_blocks[i, j] = btn_map;
                }
            }
        }

        private void ChangeViewType(bool type)
        {
            Pn_content.Visible = type;
            生成图片PToolStripMenuItem.Enabled = Pn_attributes_layout_container.Enabled =
                Pn_attributes_font_container.Enabled = Pn_attributes_appearance_container.Enabled =
                Pn_attributes_paragraph_container.Enabled = Pn_background.Visible =
                插入行RToolStripMenuItem.Enabled = 插入列CToolStripMenuItem.Enabled =
                隐藏行RToolStripMenuItem.Enabled = 隐藏列CToolStripMenuItem.Enabled =
                统一尺寸SToolStripMenuItem.Enabled = 选择全部AToolStripMenuItem.Enabled = !type;
            查找FToolStripMenuItem.Enabled = 替换RToolStripMenuItem.Enabled =
                toolStripSeparator_FAR.Visible = Ts_search.Visible = Ts_replace.Visible = type;
            RichTextBox t1 = Rtx_content;
            RichTextBox t2 = rtxs[targetRow, targetColumn];
            Text = "柒幻 千纸鹤 - [" + (type ? "内容模式]" : "设计模式]");
            if (type)
            {
                t1.Text = t2.Text;
                t1.Font = t2.Font;
                t1.ForeColor = t2.ForeColor;
                t1.BackColor = t2.BackColor;
                t1.SelectAll();
                t1.SelectionAlignment = t2.SelectionAlignment;
                t1.Focus();
            }
            else
            {
                t2.Text = t1.Text;
                t2.Focus();
            }
        }

        private void Rtx_block_DoubleClick(object sender, EventArgs e)
        {
            ChangeViewType(viewType = true);
        }

        private void Rtx_content_DoubleClick(object sender, EventArgs e)
        {
            ChangeViewType(viewType = false);
        }

        private void SetPressKeyStatus()
        {
            if (!pressCtrl && !pressShift)
            {
                statusStrip_lbl_presskey.Visible = false;
            }
            else
            {
                statusStrip_lbl_presskey.Visible = true;
                if (pressCtrl && pressShift)
                {
                    statusStrip_lbl_presskey.Text = "按下 Shift, Ctrl";
                }
                else if (pressShift)
                {
                    statusStrip_lbl_presskey.Text = "按下 Shift";
                }
                else if (pressCtrl)
                {
                    statusStrip_lbl_presskey.Text = "按下 Ctrl";
                }
            }
        }

        private void Rtx_block_KeyDown(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                pressShift = true;
                lastTargetRow = targetRow;
                lastTargetColumn = targetColumn;
            }
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                pressCtrl = true;
            }
            SetPressKeyStatus();
        }

        private void Rtx_block_KeyUp(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Shift) != Keys.Shift)
            {
                pressShift = false;
            }
            if ((ModifierKeys & Keys.Shift) != Keys.Control)
            {
                pressCtrl = false;
            }
            SetPressKeyStatus();
        }

        private int ConvertToRowIndex(int i)
        {
            return i / maxRowAndColumnCount;
        }

        private int ConvertToColumnIndex(int i)
        {
            return i % maxRowAndColumnCount;
        }

        private int GetCurrentIndex(int i = -1, int j = -1)
        {
            if (i == -1 && j == -1)
            {
                return targetRow * maxRowAndColumnCount + targetColumn;
            }
            return i * maxRowAndColumnCount + j;
        }

        private RichTextBox GetCurrentRichTextBox()
        {
            return rtxs[targetRow, targetColumn];
        }

        private List<RichTextBox> GetAllSelectedRichTextBoxs()
        {
            List<RichTextBox> res = new List<RichTextBox>();
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (selectedBlocks[i, j])
                    {
                        res.Add(rtxs[i, j]);
                    }
                }
            }
            return res;
        }

        private void SelectBlockByRowAndColumn(int row, int column)
        {
            selectedBlocks[targetRow = row, targetColumn = column] = true;
            blocks[row, column].BackColor = map_blocks[row, column].BackColor = selectedColor;
        }

        private void ShiftBlocks1(int begin, int end, int type = 0)
        {
            for (int i = begin; i <= end; i++)
            {
                if (type == 0)
                {
                    SelectBlockByRowAndColumn(targetRow, i);
                }
                else
                {
                    SelectBlockByRowAndColumn(i, targetColumn);
                }
            }
        }

        private void ShiftBlocks2(int begin1, int end1, int begin2, int end2)
        {
            for (int i = begin1; i <= end1; i++)
            {
                for (int j = begin2; j <= end2; j++)
                {
                    SelectBlockByRowAndColumn(i, j);
                }
            }
        }

        private void LoadAttributes()
        {
            Tx_attributes_layout_width.Text = blockWidth[targetColumn].ToString();
            Tx_attributes_layout_height.Text = blockHeight[targetRow].ToString();
            Combo_attributes_font_name.Text = blockFont[GetCurrentIndex()].Name.ToString();
            Num_attributes_font_size.Text = blockFont[GetCurrentIndex()].Size.ToString();
            Ck_attributes_font_type_bold.Checked = blockFontBold[GetCurrentIndex()];
            Ck_attributes_font_type_italic.Checked = blockFontItalic[GetCurrentIndex()];
            Ck_attributes_font_type_underline.Checked = blockFontUnderline[GetCurrentIndex()];
            Tx_attributes_appearance_forecolor.Text = blockForeColor[GetCurrentIndex()].ToString();
            Tx_attributes_appearance_backcolor.Text = blockBackColor[GetCurrentIndex()].ToString();
            Num_attributes_paragraph_height.Text = blockParagraphHeight[GetCurrentIndex()].ToString();
            switch (blockAlign[GetCurrentIndex()])
            {
                case 0:
                    Radio_attributes_paragraph_align_left.Checked = true;
                    break;
                case 1:
                    Radio_attributes_paragraph_align_mid.Checked = true;
                    break;
                case 2:
                    Radio_attributes_paragraph_align_right.Checked = true;
                    break;
            }
        }

        private void Rtx_block_Click(object sender, EventArgs e)
        {
            RichTextBox t = (RichTextBox)sender;
            int tag = (int)t.Tag;
            int currentRow = targetRow = ConvertToRowIndex(tag);
            int currentColumn = targetColumn = ConvertToColumnIndex(tag);
            if (pressShift)
            {
                if (targetRow != lastTargetRow && targetColumn != lastTargetColumn)
                {
                    if (targetColumn > lastTargetColumn)
                    {
                        if (targetRow < lastTargetRow)
                        {
                            ShiftBlocks2(targetRow, lastTargetRow, lastTargetColumn, targetColumn);
                        }
                        else if (targetRow > lastTargetRow)
                        {
                            ShiftBlocks2(lastTargetRow, targetRow, lastTargetColumn, targetColumn);
                        }
                    }
                    else if (targetColumn < lastTargetColumn)
                    {
                        if (targetRow < lastTargetRow)
                        {
                            ShiftBlocks2(targetRow, lastTargetRow, targetColumn, lastTargetColumn);
                        }
                        else if (targetRow > lastTargetRow)
                        {
                            ShiftBlocks2(lastTargetRow, targetRow, targetColumn, lastTargetColumn);
                        }
                    }
                }
                else if (targetRow == lastTargetRow)
                {
                    if (targetColumn > lastTargetColumn)
                    {
                        ShiftBlocks1(lastTargetColumn, targetColumn);
                    }
                    else if (targetColumn < lastTargetColumn)
                    {
                        ShiftBlocks1(targetColumn, lastTargetColumn);
                    }
                }
                else if (targetColumn == lastTargetColumn)
                {
                    if (targetRow > lastTargetRow)
                    {
                        ShiftBlocks1(lastTargetRow, targetRow, 1);
                    }
                    else if (targetRow < lastTargetRow)
                    {
                        ShiftBlocks1(targetRow, lastTargetRow, 1);
                    }
                }
            }
            else if (pressCtrl)
            {
                SelectBlockByRowAndColumn(ConvertToRowIndex(tag), ConvertToColumnIndex(tag));
            }
            else
            {
                foreach (Panel b in blocks)
                {
                    b.BackColor = commonColor;
                }
                foreach (Button b in map_blocks)
                {
                    b.BackColor = commonColor;
                }
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        selectedBlocks[i, j] = false;
                    }
                }
                SelectBlockByRowAndColumn(ConvertToRowIndex(tag), ConvertToColumnIndex(tag));
            }
            targetRow = currentRow;
            targetColumn = currentColumn;
            LoadAttributes();
            SetStatus();
        }

        private void Frm_Main_Load(object sender, EventArgs e)
        {
            SetProcess(false);
            //System
            GetFontNamesFromSystem();
            Rtx_content.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
            rtxContent = Rtx_content;
            //Layout
            for (int i = 0; i < attributesLayoutDisplay.Length; i++)
            {
                attributesLayoutDisplay[i] = true;
            }
            //Menu
            查找FToolStripMenuItem.Enabled = 替换RToolStripMenuItem.Enabled =
                toolStripSeparator_FAR.Visible = Ts_search.Visible = Ts_replace.Visible = false;
            //Settings
            try
            {
                for (int i = 0; i < maxBlocksCount; i++)
                {
                    blockWidth[i] = blockHeight[i] = 100;
                    blockFont[i] = new Font(ConfigurationManager.AppSettings["FontName"].ToString(), Convert.ToInt32(ConfigurationManager.AppSettings["FontSize"]));
                    blockFontBold[i] = Convert.ToBoolean(ConfigurationManager.AppSettings["Bold"]);
                    blockFontItalic[i] = Convert.ToBoolean(ConfigurationManager.AppSettings["Italic"]);
                    blockFontUnderline[i] = Convert.ToBoolean(ConfigurationManager.AppSettings["Underline"]);
                    blockForeColor[i] = ConfigurationManager.AppSettings["ForeColor"].ToString();
                    blockBackColor[i] = ConfigurationManager.AppSettings["BackColor"].ToString();
                    blockParagraphHeight[i] = Convert.ToInt32(ConfigurationManager.AppSettings["ParagraphHeight"]);

                    //switch (ConfigurationManager.AppSettings["Alignment"].ToString())
                    //{
                    //    case "left":
                    //        blockAlign[i] = 0;
                    //        break;
                    //    case "mid":
                    //        blockAlign[i] = 1;
                    //        break;
                    //    case "right":
                    //        blockAlign[i] = 2;
                    //        break;
                    //}
                    if (ConfigurationManager.AppSettings["Alignment"].ToString() == "left")
                    {
                        blockAlign[i] = 0;
                    }
                    else if (ConfigurationManager.AppSettings["Alignment"].ToString() == "mid")
                    {
                        blockAlign[i] = 1;
                    }
                    else if (ConfigurationManager.AppSettings["Alignment"].ToString() == "right")
                    {
                        blockAlign[i] = 2;
                    }
                }
                showScrollbars = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowScrollBar"]);
                acceptTab = Convert.ToBoolean(ConfigurationManager.AppSettings["acceptTab"]);
                showCopyCodeResponse = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowCopyCodeResponse"]);
                codeInOneLine = Convert.ToBoolean(ConfigurationManager.AppSettings["codeInOneLine"]);
                //First load attribute
                Tx_attributes_layout_width.Text = Tx_attributes_layout_height.Text = "100";
                Combo_attributes_font_name.Text = blockFont[0].Name;
                Num_attributes_font_size.Value = (decimal)blockFont[0].Size;
                Ck_attributes_font_type_bold.Checked = blockFontBold[0];
                Ck_attributes_font_type_italic.Checked = blockFontItalic[0];
                Ck_attributes_font_type_underline.Checked = blockFontUnderline[0];
                Tx_attributes_appearance_forecolor.Text = blockForeColor[0];
                Tx_attributes_appearance_backcolor.Text = blockBackColor[0];
                Num_attributes_paragraph_height.Value = blockParagraphHeight[0];
                switch (blockAlign[0])
                {
                    case 0:
                        Radio_attributes_paragraph_align_left.Checked = true;
                        break;
                    case 1:
                        Radio_attributes_paragraph_align_mid.Checked = true;
                        break;
                    case 2:
                        Radio_attributes_paragraph_align_right.Checked = true;
                        break;
                }
            }
            catch
            {
                ReportError("配置文件异常");
                Application.Exit();
                return;
            }
            GenerateBlocksAndMaps();
            SelectBlockByRowAndColumn(0, 0);
            AutoResizeBlocks();
            rtxss.ForEach(rtx => rtx.ScrollBars = showScrollbars ? RichTextBoxScrollBars.Both : RichTextBoxScrollBars.None);
            SetProcess(true);
            firstLoadForm = false;
        }

        private void Frm_Main_Resize(object sender, EventArgs e)
        {
            if (!firstLoadForm)
            {
                AutoResizeBlocks();
                //Block size and align
                int width = Pn_background.Width;
                int height = Pn_background.Height;
                for (int i = 0; i < columnCount; i++)
                {
                    for (int j = 0; j < rowCount; j++)
                    {
                        blocks[j, i].Width = (int)(blockWidth[i] / 100.0 * width);
                    }
                }
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        blocks[i, j].Height = (int)(blockHeight[i] / 100.0 * height);
                    }
                }
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 1; j < columnCount; j++)
                    {
                        blocks[i, j].Left = blocks[i, j - 1].Left + blocks[i, j - 1].Width;
                    }
                }
                for (int i = 0; i < columnCount; i++)
                {
                    for (int j = 1; j < rowCount; j++)
                    {
                        blocks[j, i].Top = blocks[j - 1, i].Top + blocks[j - 1, i].Height;
                    }
                }
            }
        }

        private void ResetRowOrColumn(int type = 0)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (type == 0)
                    {
                        blocks[i, j].Width = rowCount / 100;
                    }
                    else
                    {
                        blocks[i, j].Height = columnCount / 100;
                    }
                }
            }
            for (int i = 0; i < columnCount; i++)
            {
                blockWidth[i] = 100.0 / columnCount;
            }
            for (int i = 0; i < rowCount; i++)
            {
                blockHeight[i] = 100.0 / rowCount;
            }
        }

        private void 插入行RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rowCount < maxRowAndColumnCount)
            {
                rowCount++;
                ResetRowOrColumn(1);
                LoadAttributes();
                AutoResizeBlocks();
            }
        }

        private void 插入列CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (columnCount < maxRowAndColumnCount)
            {
                columnCount++;
                ResetRowOrColumn();
                LoadAttributes();
                AutoResizeBlocks();
            }
        }

        private void 隐藏行RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rowCount > 1)
            {
                rowCount--;
                ResetRowOrColumn(1);
                LoadAttributes();
                AutoResizeBlocks();
            }
        }

        private void 隐藏列CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (columnCount > 1)
            {
                columnCount--;
                ResetRowOrColumn();
                LoadAttributes();
                AutoResizeBlocks();
            }
        }

        private void SplitContainer_Panel1_Resize(object sender, EventArgs e)
        {
            if (!firstLoadForm)
            {
                AutoResizeBlocks();
            }
        }

        private void SplitContainer_Panel2_Resize(object sender, EventArgs e)
        {
            if (!firstLoadForm)
            {
                AutoResizeBlocks();
            }
        }

        private void SplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!firstLoadForm)
            {
                AutoResizeBlocks();
            }
        }

        private void ReportError(string errorMessage = "参数不合法")
        {
            if (errorMessage == "参数不合法")
            {
                LoadAttributes();
            }
            MessageBox.Show(errorMessage, "错误");
        }

        private void Tx_attributes_layout_width_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string str = Tx_attributes_layout_width.Text;
                if (!double.TryParse(str, out double w) && w > 0)
                {
                    ReportError();
                    return;
                }
                //Set attributes
                int width = Pn_background.Width;
                HashSet<int> set = new HashSet<int>();
                for (int i = 0; i < columnCount; i++)
                {
                    for (int j = 0; j < rowCount; j++)
                    {
                        if (selectedBlocks[j, i])
                        {
                            set.Add(i);
                            break;
                        }
                    }
                }
                double totalWidth = 0;
                for (int i = 0; i < columnCount; i++)
                {
                    totalWidth += set.Contains(i) ? w : blockWidth[i];
                }
                if (totalWidth > 100)
                {
                    //debug
                    MessageBox.Show("总宽度不能超过100，当前为" + totalWidth, "错误");
                    return;
                }
                for (int i = 0; i < columnCount; i++)
                {
                    if (set.Contains(i))
                    {
                        blockWidth[i] = w;
                        for (int j = 0; j < rowCount; j++)
                        {
                            blocks[j, i].Width = (int)(w / 100.0 * width);
                        }
                    }
                }
                //Block align
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 1; j < columnCount; j++)
                    {
                        blocks[i, j].Left = blocks[i, j - 1].Left + blocks[i, j - 1].Width;
                    }
                }
            }
        }

        private void Tx_attributes_layout_height_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string str = Tx_attributes_layout_height.Text;
                if (!double.TryParse(str, out double h) && h > 0)
                {
                    ReportError();
                    return;
                }
                //Set attributes
                int height = Pn_background.Height;
                HashSet<int> set = new HashSet<int>();
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        if (selectedBlocks[i, j])
                        {
                            set.Add(i);
                            break;
                        }
                    }
                }
                double totalHeight = 0;
                for (int i = 0; i < rowCount; i++)
                {
                    totalHeight += set.Contains(i) ? h : blockHeight[i];
                }
                if (totalHeight > 100)
                {
                    //debug
                    MessageBox.Show("总高度不能超过100，当前为" + totalHeight, "错误");
                    return;
                }
                for (int i = 0; i < rowCount; i++)
                {
                    if (set.Contains(i))
                    {
                        blockHeight[i] = h;
                        for (int j = 0; j < columnCount; j++)
                        {
                            blocks[i, j].Height = (int)(h / 100.0 * height);
                        }
                    }
                }
                //Block align
                for (int i = 0; i < columnCount; i++)
                {
                    for (int j = 1; j < rowCount; j++)
                    {
                        blocks[j, i].Top = blocks[j - 1, i].Top + blocks[j - 1, i].Height;
                    }
                }
            }
        }

        private void GetFontNamesFromSystem()
        {
            FontFamily[] fontFamilies;
            Combo_attributes_font_name.Items.Clear();
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            fontFamilies = installedFontCollection.Families;
            int count = fontFamilies.Length;
            for (int i = 0; i < count; i++)
            {
                Combo_attributes_font_name.Items.Add(fontFamilies[i].Name);
            }
        }

        private void Frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
        }

        private void Combo_attributes_font_name_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string fontName = Combo_attributes_font_name.Text;
                if (Combo_attributes_font_name.Items.Contains(fontName))
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < columnCount; j++)
                        {
                            if (selectedBlocks[i, j])
                            {
                                blockFont[GetCurrentIndex(i, j)] = rtxs[i, j].Font = new Font(fontName, rtxs[i, j].Font.Size);
                            }
                        }
                    }
                }
                else
                {
                    ReportError();
                }
            }
        }

        private void Combo_attributes_font_name_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (selectedBlocks[i, j])
                    {
                        blockFont[GetCurrentIndex(i, j)] = rtxs[i, j].Font = new Font(Combo_attributes_font_name.Items[Combo_attributes_font_name.SelectedIndex].ToString(), rtxs[i, j].Font.Size);
                    }
                }
            }
        }

        private void Num_attributes_font_size_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string str = Num_attributes_font_size.Value.ToString();
                if (!int.TryParse(str, out int w))
                {
                    ReportError();
                    return;
                }
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        if (selectedBlocks[i, j])
                        {
                            blockFont[GetCurrentIndex(i, j)] = rtxs[i, j].Font = new Font(rtxs[i, j].Font.Name, w);
                        }
                    }
                }
            }
        }

        private void Ck_attributes_font_type_bold_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (selectedBlocks[i, j])
                    {
                        int t = GetCurrentIndex(i, j);
                        blockFontBold[t] = Ck_attributes_font_type_bold.Checked;
                        SetFontStyle(t);
                    }
                }
            }
        }

        private void Ck_attributes_font_type_italic_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (selectedBlocks[i, j])
                    {
                        int t = GetCurrentIndex(i, j);
                        blockFontItalic[t] = Ck_attributes_font_type_italic.Checked;
                        SetFontStyle(t);
                    }
                }
            }
        }

        private void Ck_attributes_font_type_underline_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (selectedBlocks[i, j])
                    {
                        int t = GetCurrentIndex(i, j);
                        blockFontUnderline[t] = Ck_attributes_font_type_underline.Checked;
                        SetFontStyle(t);
                    }
                }
            }
        }

        private void Tx_attributes_appearance_forecolor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string str = Tx_attributes_appearance_forecolor.Text;
                if (!GetColorFromHtml(str, out Color color))
                {
                    ReportError();
                    return;
                }
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        if (selectedBlocks[i, j])
                        {
                            try
                            {
                                blockForeColor[GetCurrentIndex(i, j)] = str;
                                rtxs[i, j].ForeColor = color;
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        private void Tx_attributes_appearance_backcolor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string str = Tx_attributes_appearance_backcolor.Text;
                if (!GetColorFromHtml(str, out Color color))
                {
                    ReportError();
                    return;
                }
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        if (selectedBlocks[i, j])
                        {
                            try
                            {
                                rtxs[i, j].BackColor = color;
                                blockBackColor[GetCurrentIndex(i, j)] = str;
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        private void Num_attributes_paragraph_height_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string str = Num_attributes_paragraph_height.Text;
                if (!int.TryParse(str, out int w))
                {
                    ReportError();
                    return;
                }
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        if (selectedBlocks[i, j])
                        {
                            blockParagraphHeight[GetCurrentIndex(i, j)] = w;
                        }
                    }
                }
            }
        }

        private void Radio_attributes_paragraph_align_left_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (selectedBlocks[i, j])
                    {
                        blockAlign[GetCurrentIndex(i, j)] = 0;
                        rtxs[i, j].SelectAll();
                        rtxs[i, j].SelectionAlignment = HorizontalAlignment.Left;
                    }
                }
            }
        }

        private void Radio_attributes_paragraph_align_mid_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (selectedBlocks[i, j])
                    {
                        blockAlign[GetCurrentIndex(i, j)] = 1;
                        rtxs[i, j].SelectAll();
                        rtxs[i, j].SelectionAlignment = HorizontalAlignment.Center;
                    }
                }
            }
        }

        private void Radio_attributes_paragraph_align_right_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (selectedBlocks[i, j])
                    {
                        blockAlign[GetCurrentIndex(i, j)] = 2;
                        rtxs[i, j].SelectAll();
                        rtxs[i, j].SelectionAlignment = HorizontalAlignment.Right;
                    }
                }
            }
        }

        private void 选择全部AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    SelectBlockByRowAndColumn(i, j);
                    SetStatus();
                }
            }
        }

        private void Btn_attributes_layout_Click(object sender, EventArgs e)
        {
            Pn_attributes_layout_container.Visible = attributesLayoutDisplay[0] = !attributesLayoutDisplay[0];
        }

        private void Btn_attributes_font_Click(object sender, EventArgs e)
        {
            Pn_attributes_font_container.Visible = attributesLayoutDisplay[1] = !attributesLayoutDisplay[1];
        }

        private void Btn_attributes_appearance_Click(object sender, EventArgs e)
        {
            Pn_attributes_appearance_container.Visible = attributesLayoutDisplay[2] = !attributesLayoutDisplay[2];
        }

        private void Btn_attributes_paragraph_Click(object sender, EventArgs e)
        {
            Pn_attributes_paragraph_container.Visible = attributesLayoutDisplay[3] = !attributesLayoutDisplay[3];
        }

        private void Frm_Main_Shown(object sender, EventArgs e)
        {
            rtxss[0].Focus();
        }

        private void 新建项目NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = Application.ExecutablePath;
                Process.Start(fileName);
            }
            catch { }
        }

        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;
            byte curByte;
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("not expected byte format");
            }
            return true;
        }

        public static Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF };
            Encoding reVal = Encoding.Default;
            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int.TryParse(fs.Length.ToString(), out int i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        public static Encoding GetType(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        private void 置入IToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Title = "打开文件",
                Filter = "文本文档|*.txt",
                FileName = "",
            };
            if (openDialog.ShowDialog() == DialogResult.OK && openDialog.FileName.Length > 0)
            {
                try
                {
                    string path = openDialog.FileName;
                    //Encoding f_coding = GetType(path);
                    //StreamReader sr = new StreamReader(path, f_coding);
                    StreamReader sr = new StreamReader(path);
                    string tx = sr.ReadToEnd();
                    if (viewType)
                    {
                        Rtx_content.Text = tx;
                    }
                    else
                    {
                        List<RichTextBox> list = GetAllSelectedRichTextBoxs();
                        for (int i = 0; i < list.Count; i++)
                        {
                            list[i].Text = tx;
                        }
                    }
                }
                catch { }
            }
            openDialog.Dispose();
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ShowFARDialog(int type = 0)
        {
            FAR_type = type;
            new Frm_FAR().Show();
        }

        private void 查找FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFARDialog();
        }

        private void 替换RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFARDialog(1);
        }

        private void ContentUndo()
        {
            if (viewType)
            {
                Rtx_content.Undo();
            }
            else
            {
                GetCurrentRichTextBox().Undo();
            }
        }

        private void ContentRedo()
        {
            if (viewType)
            {
                Rtx_content.Redo();
            }
            else
            {
                GetCurrentRichTextBox().Redo();
            }
        }

        private void ContentCut()
        {
            if (viewType)
            {
                Rtx_content.Cut();
            }
            else
            {
                List<RichTextBox> list = GetAllSelectedRichTextBoxs();
                list.ForEach(rtx => rtx.Cut());
            }
        }

        private void ContentCopy()
        {
            try
            {
                Clipboard.SetDataObject(viewType ? Rtx_content.SelectedText : GetCurrentRichTextBox().SelectedText, true);
            }
            catch { };
        }

        private void ContentPaste()
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
            {
                if (viewType)
                {
                    Clipboard.SetDataObject(new RichTextBox
                    {
                        Font = Rtx_content.Font,
                        ForeColor = Rtx_content.ForeColor,
                        BackColor = Rtx_content.BackColor,
                        Text = Clipboard.GetText()
                    }.Text, true); // Reset text format
                    Rtx_content.Paste();
                }
                else
                {
                    List<RichTextBox> list = GetAllSelectedRichTextBoxs();
                    list.ForEach(rtx => Clipboard.SetDataObject(new RichTextBox
                    {
                        Font = rtx.Font,
                        ForeColor = rtx.ForeColor,
                        BackColor = rtx.BackColor,
                        Text = Clipboard.GetText()
                    }.Text, true));
                    list.ForEach(rtx => rtx.Paste());
                }
            }
        }

        private void ContentDelete()
        {
            if (viewType)
            {
                Rtx_content.SelectedText = "";
            }
            else
            {
                GetCurrentRichTextBox().SelectedText = "";
            }
        }

        private void 撤销UToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContentUndo();
        }

        private void 重做RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContentRedo();
        }

        private void 剪切TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContentCut();
        }

        private void 复制CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContentCopy();
        }

        private void 粘贴PToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContentPaste();
        }

        private void 删除DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContentDelete();
        }

        private void 全选AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewType)
            {
                Rtx_content.SelectAll();
            }
            else
            {
                GetCurrentRichTextBox().SelectAll();
            }
        }

        private void Ts_cut_Click(object sender, EventArgs e)
        {
            ContentCut();
        }

        private void Ts_copy_Click(object sender, EventArgs e)
        {
            ContentCopy();
        }

        private void Ts_paste_Click(object sender, EventArgs e)
        {
            ContentPaste();
        }

        private void Ts_delete_Click(object sender, EventArgs e)
        {
            ContentDelete();
        }

        private void Ts_undo_Click(object sender, EventArgs e)
        {
            ContentUndo();
        }

        private void Ts_redo_Click(object sender, EventArgs e)
        {
            ContentRedo();
        }

        private void Ts_search_Click(object sender, EventArgs e)
        {
            ShowFARDialog();
        }

        private void Ts_replace_Click(object sender, EventArgs e)
        {
            ShowFARDialog(1);
        }

        private void 内容模式CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeViewType(viewType = true);
        }

        private void 设计模式DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeViewType(viewType = false);
        }

        private void ExportImage()
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    blocks[i, j].BackColor = commonColor;
                }
            }
            if (viewType)
            {
                MessageBox.Show("内容模式下无法保存图片，请转到设计模式后重试", "提示");
            }
            else
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                Graphics g1 = Pn_background.CreateGraphics();
                Bitmap img = new Bitmap(Pn_background.Width, Pn_background.Height, g1);
                Graphics g2 = Graphics.FromImage(img);
                IntPtr dc1 = g1.GetHdc();
                IntPtr dc2 = g2.GetHdc();
                BitBlt(dc2, 0, 0, this.Pn_background.Width, this.Pn_background.Height, dc1, 0, 0, 13369376);
                g1.ReleaseHdc(dc1);
                g2.ReleaseHdc(dc2);
                saveDialog.Title = "输出为图片";
                saveDialog.Filter = "Jpg 图片|*.jpg";
                saveDialog.FileName = "新建图片";
                if (saveDialog.ShowDialog() == DialogResult.OK && saveDialog.FileName.Length > 0)
                {
                    img.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (selectedBlocks[i, j])
                    {
                        blocks[i, j].BackColor = selectedColor;
                    }
                }
            }
        }


        private void 生成图片PToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportImage();
        }

        private string AddIndent(int num)
        {
            if (codeInOneLine)
            {
                return "";
            }
            StringBuilder result = new StringBuilder();
            result.Append("\n");
            while (num-- > 0)
            {
                result.Append("\t");
            }
            return result.ToString();
        }

        private void GenerateCode()
        {
            SetProcess(false);
            StringBuilder result = new StringBuilder();
            string startDiv = "<div style=\"clear:both;\">";
            string endDiv = "</div>";
            result.Append(startDiv);
            result.Append(AddIndent(1));
            for (int i = 0; i < rowCount; i++)
            {
                if (i > 0)
                {
                    result.Append(AddIndent(1));
                }
                result.Append(startDiv);
                result.Append(AddIndent(2));
                for (int j = 0; j < columnCount; j++)
                {
                    if (j > 0)
                    {
                        result.Append(AddIndent(2));
                    }
                    result.Append("<div style=\"float:left;width:");
                    result.Append(blockWidth[j]);
                    result.Append("%;font-size:");
                    result.Append(blockFont[GetCurrentIndex(i, j)].Size);
                    result.Append("px;color:#");
                    result.Append(blockForeColor[GetCurrentIndex(i, j)]);
                    result.Append(";background-color:#");
                    result.Append(blockBackColor[GetCurrentIndex(i, j)]);
                    result.Append(";line-height:");
                    result.Append(blockParagraphHeight[GetCurrentIndex(i, j)]);
                    result.Append("px;text-align:");
                    switch (blockAlign[GetCurrentIndex(i, j)])
                    {
                        case 0:
                            result.Append("left");
                            break;
                        case 1:
                            result.Append("center");
                            break;
                        case 2:
                            result.Append("right");
                            break;
                    }
                    result.Append(";\">");
                    result.Append(AddIndent(3));
                    result.Append("<font name=\"");
                    result.Append(blockFont[GetCurrentIndex(i, j)].Name);
                    result.Append("\">");
                    result.Append(AddIndent(4));
                    if (blockFontBold[GetCurrentIndex(i, j)])
                    {
                        result.Append("<b>");
                    }
                    if (blockFontItalic[GetCurrentIndex(i, j)])
                    {
                        result.Append("<i>");
                    }
                    if (blockFontUnderline[GetCurrentIndex(i, j)])
                    {
                        result.Append("<u>");
                    }
                    if (rtxss[GetCurrentIndex(i, j)].Text.ToString() != "")
                    {
                        result.Append(rtxss[GetCurrentIndex(i, j)].Text.ToString().Replace("\n", "<br/>").Replace(" ", "&nbsp;"));
                    }
                    else
                    {
                        result.Append("&nbsp;");
                    }
                    if (blockFontUnderline[GetCurrentIndex(i, j)])
                    {
                        result.Append("</u>");
                    }
                    if (blockFontItalic[GetCurrentIndex(i, j)])
                    {
                        result.Append("</i>");
                    }
                    if (blockFontBold[GetCurrentIndex(i, j)])
                    {
                        result.Append("</b>");
                    }
                    result.Append(AddIndent(3));
                    result.Append("</font>");
                    result.Append(AddIndent(2));
                    result.Append(endDiv);
                }
                result.Append(AddIndent(1));
                result.Append(endDiv);
            }
            result.Append(AddIndent(0));
            result.Append(endDiv);
            Clipboard.SetDataObject(result.ToString(), true);
            SetProcess(true);
            if (showCopyCodeResponse)
            {
                MessageBox.Show("已复制HTML代码到剪贴板", "提示");
            }
        }

        private void 复制代码到剪切板CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateCode();
        }

        private void 统一尺寸SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetRowOrColumn();
            ResetRowOrColumn(1);
            AutoResizeBlocks();
        }

        private void 选项OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm_Option frm_Option = new Frm_Option();
            frm_Option.ShowDialog();
        }

        private void 关于柒幻千纸鹤AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Frm_About frm_About = new Frm_About();
            frm_About.ShowDialog();
        }
    }
}
