using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrontEnd
{
    public partial class MainForm : Form
    {
        string FilePath;

        string[] globalargs;

        public MainForm(string[] args)
        {
            InitializeComponent();
            globalargs = args;
        }

        private void LinesPrinter()
        {
            textBoxLines.Text = "";

            for (int i = 1; i <= textBox1.Lines.Length + 1; ++i)
            {
                textBoxLines.Text += i.ToString() + ".\r\n";
            }
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsFile();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            LinesPrinter();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (globalargs.Length == 0)
            {
                textBox1.Text = "<!DOCTYPE html>\r\n" +
                    "<html lang = \"en\">\r\n" +
                    "<head>\r\n" +
                    "    <meta charset = \"1251\">\r\n" +
                    "    <meta http - equiv = \"X-UA-Compatible\" content = \"IE=edge\">\r\n" +
                    "    <meta name = \"viewport\" content = \"width=device-width, initial-scale=1.0\">\r\n" +
                    "    <title> Document </title>\r\n" +
                    "</head>\r\n" +
                    "<body>\r\n" +
                    "    \r\n" +
                    "</body>\r\n" +
                    "</html>";
            }
            else if (globalargs[0].Split('.')[globalargs[0].Split('.').Length - 1] == "css" ||
                globalargs[0].Split('.')[globalargs[0].Split('.').Length - 1] == "html")
            {
                try
                {
                    foreach (var item in globalargs)
                    {
                        FilePath += item;
                    }

                    var reader = new System.IO.StreamReader(FilePath, Encoding.GetEncoding(1251));
                    textBox1.Text = reader.ReadToEnd();
                    reader.Close();

                    LinesPrinter();
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    MessageBox.Show(ex.Message + "\nНет такого файла",
                             "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                         "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            else
            {
                MessageBox.Show("Неправильное расширение файла", "FrontEnd", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            LinesPrinter();
        }

        private void textBox1_vScroll(Message message)
        {
            message.HWnd = textBoxLines.Handle;
            textBoxLines.PubWndProc(ref message);
        }

        private void FormattedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder output = new StringBuilder();

            if (FilePath != null && FilePath.Split('.')[FilePath.Split('.').Length - 1] == "css")
            {
                var cssDocument = new CssDocument();

                if (cssDocument.BracketsCheck(textBox1))
                {
                    cssDocument.Formatter(textBox1);
                }
                else
                {
                    MessageBox.Show("Код не ликвиден", "FrontEnd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                return;
            }

            var htmlDocument = new HtmlDocument();
            htmlDocument.Formatter(textBox1);
        }

        private void LiquidityCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Select(0, textBox1.Text.Length);
            textBox1.SelectionColor = Color.DarkOrange;

            if (FilePath != null && FilePath.Split('.')[FilePath.Split('.').Length - 1] == "css")
            {
                var cssDocument = new CssDocument();

                if (!cssDocument.BracketsCheck(textBox1))
                {
                    return;
                }

                if (cssDocument.MonadsCheck(textBox1))
                {
                    MessageBox.Show("Ошибок не найдено!", "FrontEnd", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            else
            {
                var htmlDocument = new HtmlDocument();

                if (!htmlDocument.BracketsCheck(textBox1))
                {
                    return;
                }

                MessageBox.Show("Ошибок не найдено!", "FrontEnd", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void OpenFile()
        {

            openFileDialog.Filter = "All|*.html;*.css|.html|*.html|.css|*.css";

            if (openFileDialog.ShowDialog() == DialogResult.Cancel) return;

            try
            {
                var reader = new System.IO.StreamReader(
                openFileDialog.FileName, Encoding.GetEncoding(1251));
                textBox1.Text = reader.ReadToEnd();
                reader.Close();

                LinesPrinter();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message + "\nНет такого файла",
                         "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                     "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            FilePath = openFileDialog.FileName;
        }

        private void SaveFile()
        {
            if (FilePath != null)
            {
                try
                {
                    var writer = new StreamWriter(openFileDialog.FileName, false, Encoding.GetEncoding(1251));
                    writer.Write(textBox1.Text);
                    writer.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            } else
            {
                SaveAsFile();
            }
        }

        private void SaveAsFile()
        {
            saveFileDialog.Filter = ".html | *.html |.css | *.css";

            saveFileDialog.FileName = openFileDialog.FileName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var writer = new StreamWriter(saveFileDialog.FileName, false, Encoding.GetEncoding(1251));
                    writer.Write(textBox1.Text);
                    writer.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}
