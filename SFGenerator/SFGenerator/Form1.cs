using System;
using System.Drawing;
using System.Windows.Forms;

namespace SFGenerator
{
    public partial class Form_Main : Form
    {
        Font font = null;
        Color color = Color.White;

        bool refresh = true;
        bool refreshing = false;

        string result_text = "";
        Font result_font = null;
        Bitmap result_bitmap = null;

        public Form_Main()
        {
            font = Font;
            color = Color.White;

            InitializeComponent();
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            FontDialog.Font = font;

            TextBox_SpaceGlyph.Font = new Font(FontDialog.Font.Name, 12, FontDialog.Font.Style);
            TextBox_Glyphs.Font = new Font(FontDialog.Font.Name, 12, FontDialog.Font.Style);

            ColorDialog.Color = color;
        }

        private void Button_Font_Click(object sender, EventArgs e)
        {
            DialogResult result = FontDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                TextBox_SpaceGlyph.Font = new Font(FontDialog.Font.Name, 9, FontDialog.Font.Style);
            }
        }

        private void Button_Color_Click(object sender, EventArgs e)
        {
            DialogResult result = ColorDialog.ShowDialog();

            if (result == DialogResult.OK)
            {

            }
        }

        private void TextBox_SpaceGlyph_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button_Apply_Click(object sender, EventArgs e)
        {
            if (refresh)
            {
                TextBox_Glyphs_Refresh();
            }

            font = FontDialog.Font;
            color = ColorDialog.Color;

            TextBox_Glyphs.Font = new Font(font.Name, 12, font.Style);

            string text = TextBox_Glyphs.Text;
            string text_largest = " ";
            if (TextBox_SpaceGlyph.Text.Length > 0)
            {
                text_largest = TextBox_SpaceGlyph.Text.Trim().Substring(0, 1);
            }

            int space_h = (int)NumericUpDown_HSpace.Value;
            int space_v = (int)NumericUpDown_VSpace.Value;

            Size size = TextRenderer.MeasureText(text_largest, font);
            for (int i = 0; i < text.Length; i++)
            {
                string s = text.Substring(i, 1);
                Size sizes = TextRenderer.MeasureText(s, font);

                if (size.Width < sizes.Width || size.Height < sizes.Height)
                {
                    size = sizes;
                }
            }

            size.Width = (int)(size.Width * 0.8);
            size.Height = (int)(size.Height * 0.8);

            int width = ((space_h * 2) + size.Width) * text.Length;
            int height = (space_v * 2) + size.Height;

            int x = space_h + size.Width / 2;
            int y = space_v + size.Height / 2;


            Bitmap bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bitmap);

            if (CheckBox_AntiAliasing.Checked)
            {
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            }
            else
            {
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            }

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            graphics.Clear(Color.Transparent);

            StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            for (int i = 0; i < text.Length; i++)
            {
                string s = text.Substring(i, 1);

                if (s.Equals(" "))
                {
                    s = TextBox_SpaceGlyph.Text;
                }

                graphics.DrawString(s, font, new SolidBrush(color), x, y, format);
                x += (space_h * 2 + size.Width);

            }
            graphics.Flush();

            PictureBox_Preview.Image = bitmap;
            PictureBox_Preview.Width = bitmap.Width;
            PictureBox_Preview.Height = bitmap.Height;

            TrackBar_Zoom_Refresh();

            Label_Preview.Text = string.Format("{0} Frames ({1}×{2} | {3}×{4})", text.Length, (space_h * 2) + size.Width, (space_v * 2) + size.Height, width, height);

            Button_ExportPNG.Enabled = true;
            Button_ExportGlyph.Enabled = true;

            result_text = text;
            result_font = font;
            result_bitmap = bitmap;
        }

        private void TextBox_Glyphs_TextChanged(object sender, EventArgs e)
        {
            refresh = true;
            if (refreshing)
            {
                refresh = false;
                refreshing = false;
            }

            Button_GlyphsRefresh.Enabled = refresh;
        }

        private void Button_ImportFromFile_Click(object sender, EventArgs e)
        {
            DialogResult result = OpenFileDialog_Glyphs.ShowDialog();

            if (result == DialogResult.OK)
            {
                string[] filepaths = OpenFileDialog_Glyphs.FileNames;
                foreach (var filepath in filepaths)
                {
                    string text = System.IO.File.ReadAllText(filepath);

                    TextBox_Glyphs.Text += text;
                }
                refresh = true;
                Button_GlyphsRefresh.Enabled = true;
            }
        }

        private void Button_GlyphsRefresh_Click(object sender, EventArgs e)
        {
            TextBox_Glyphs_Refresh();
        }

        #region
        private void TextBox_Glyphs_Refresh()
        {
            string text = TextBox_Glyphs.Text;
            TextBox_Glyphs.Text = " ";

            text = text.Replace("\r\n", "");
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");

            while (text.Length != 0)
            {
                string c = text.Substring(0, 1);
                if (!TextBox_Glyphs.Text.Contains(c))
                {
                    int i = 0;
                    for (i = 0; i < TextBox_Glyphs.Text.Length; i++)
                    {
                        if (c.CompareTo(TextBox_Glyphs.Text.Substring(i, 1)) <= 0)
                        {
                            break;
                        }
                    }
                    TextBox_Glyphs.Text = TextBox_Glyphs.Text.Insert(i, c);
                }
                text = text.Replace(c, "");
            }

            refresh = false;
            refreshing = true;

            Button_GlyphsRefresh.Enabled = false;
        }
        #endregion

        private void NumericUpDown_BRed_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown_B_Refresh();
        }

        private void NumericUpDown_BGreen_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown_B_Refresh();
        }

        private void NumericUpDown_BBlue_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown_B_Refresh();
        }

        private void TrackBar_Zoom_Scroll(object sender, EventArgs e)
        {
            TrackBar_Zoom_Refresh();
        }

        private void Button_ZoomReset_Click(object sender, EventArgs e)
        {
            TrackBar_Zoom.Value = 1;
            TrackBar_Zoom_Refresh();
        }

        private void Button_ExportGlyph_Click(object sender, EventArgs e)
        {
            if (result_bitmap != null)
            {
                SaveFileDialog_Glyphs.FileName = string.Format("{0} {1} {2}_strip{3}.txt", result_font.Name, result_font.Size, result_font.Style.ToString(), result_text.Length);
                DialogResult result = SaveFileDialog_Glyphs.ShowDialog();

                if (result == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(SaveFileDialog_Glyphs.FileName, result_text);
                }
            }
        }

        private void Button_ExportPNG_Click(object sender, EventArgs e)
        {
            if (result_bitmap != null)
            {
                SaveFileDialog_Sprite.FileName = string.Format("{0} {1} {2}_strip{3}.png", result_font.Name, result_font.Size, result_font.Style.ToString(), result_text.Length);
                DialogResult result = SaveFileDialog_Sprite.ShowDialog();

                if (result == DialogResult.OK)
                {
                    result_bitmap.Save(SaveFileDialog_Sprite.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        #region
        private void NumericUpDown_B_Refresh()
        {
            Panel_Preview.BackColor = Color.FromArgb(255, (int)NumericUpDown_BRed.Value, (int)NumericUpDown_BGreen.Value, (int)NumericUpDown_BBlue.Value);
        }

        private void TrackBar_Zoom_Refresh()
        {
            if (PictureBox_Preview.Image != null)
            {
                PictureBox_Preview.Width = PictureBox_Preview.Image.Width * TrackBar_Zoom.Value;
                PictureBox_Preview.Height = PictureBox_Preview.Image.Height * TrackBar_Zoom.Value;
            }
        }
        #endregion
    }
}