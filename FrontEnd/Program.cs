using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrontEnd
{
    public class SynchronizedScrollTextBox : RichTextBox
    {
        public event vScrollEventHandler vScroll;
        public delegate void vScrollEventHandler(Message message);

        public const int WM_VSCROLL = 0x115;

        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == WM_VSCROLL)
            {
                if (vScroll != null)
                {
                    vScroll(msg);
                }
            }
            base.WndProc(ref msg);
        }

        public void PubWndProc(ref System.Windows.Forms.Message msg)
        {
            base.WndProc(ref msg);
        }
    }

    abstract class WebDocument
    {
        protected StringBuilder output;

        protected int openBracketCounter;
        protected int closeBracketCounter;

        public abstract void Formatter(SynchronizedScrollTextBox textBox);

        public abstract bool BracketsCheck(SynchronizedScrollTextBox textBox);
    }

    class CssDocument : WebDocument
    {
        public override void Formatter(SynchronizedScrollTextBox textBox)
        {
            output = new StringBuilder();

            for (int i = 0; i < textBox.Text.Length; i++)
            {
                if (textBox.Text[i] == '{')
                {
                    output.Append(textBox.Text[i] + "\r\n");
                    i++;

                    while (textBox.Text[i] == ' ')
                    {
                        i++;
                    }

                    while (i < textBox.Text.Length && textBox.Text[i] != '}')
                    {
                        while (textBox.Text[i] == '\r' || textBox.Text[i] == '\n' || textBox.Text[i] == ' ')
                        {
                            i++;
                        }

                        output.Append("    ");

                        do
                        {
                            output.Append(textBox.Text[i++]);
                        } while (textBox.Text[i - 1] != ';' && i + 1 < textBox.Text.Length);

                        output.Append("\r\n");

                        while (textBox.Text[i] == '\r' || textBox.Text[i] == '\n')
                        {
                            i++;
                        }
                    }

                    output.Append('}');
                }
                else
                {
                    output.Append(textBox.Text[i]);
                }
            }

            textBox.Text = output.ToString();
        }

        public override bool BracketsCheck(SynchronizedScrollTextBox textBox)
        {
            openBracketCounter = 0;
            closeBracketCounter = 0;

            var textByLines = textBox.Text.Split('\n');
            var linesCounter = 0;

            for (var i = 0; i < textBox.Text.Length; i++)
            {
                if (textBox.Text[i] == '{')
                {
                    openBracketCounter++;
                }

                if (textBox.Text[i] == '}')
                {
                    closeBracketCounter++;
                }

                if (textBox.Text[i] == '\n')
                {
                    linesCounter++;
                }

                if (openBracketCounter - closeBracketCounter > 1 || openBracketCounter < closeBracketCounter)
                {

                    var index = textBox.Text.IndexOf(textByLines[linesCounter], i - textByLines[linesCounter].Length);

                    textBox.Select(index, textByLines[linesCounter].Length);
                    textBox.SelectionColor = Color.Red;

                    return false;
                }
            }

            if (openBracketCounter != closeBracketCounter)
            {
                while (textByLines[linesCounter] == string.Empty)
                {
                    linesCounter--;
                }
                textBox.Select(textBox.Text.LastIndexOf(textByLines[linesCounter]), textByLines[linesCounter].Length);
                textBox.SelectionColor = Color.Red;

                return false;
            }
            else
            if (openBracketCounter == closeBracketCounter)
            {
                return true;
            }

            return false;
        }

        public bool MonadsCheck(SynchronizedScrollTextBox textBox)
        {
            var textByLines = textBox.Text.Split('\n');
            var linesCounter = 0;

            StringComparison comp = StringComparison.InvariantCultureIgnoreCase;

            Formatter(textBox);

            for (var i = 0; i < textBox.Text.Length; i++)
            {

                if (textByLines[linesCounter].Length > 4 && textByLines[linesCounter].Substring(0, 4).Equals("    ", comp) && textByLines[linesCounter][textByLines[linesCounter].Length - 1] != ';')
                {
                    var index = textBox.Text.IndexOf(textByLines[linesCounter]);

                    textBox.Select(index, textByLines[linesCounter].Length);
                    textBox.SelectionColor = Color.Red;

                    return false;
                }

                if (textBox.Text[i] == '\n')
                {
                    linesCounter++;
                }
            }

            return true;
        }
    }

    class HtmlDocument : WebDocument
    {
        private string[] PairedTags = { "div", "header", "head", "article", "aside", "body", "ol", "ul", "li", "span", "button", "p",
        "a", "nav", "h1", "h2", "h3"};

        public override bool BracketsCheck(SynchronizedScrollTextBox textBox)
        {
            openBracketCounter = 0;
            closeBracketCounter = 0;

            var textByLines = textBox.Text.Split('\n');

            var textLenght = 0;

            foreach (var line in textByLines)
            {
                textLenght += line.Length;

                openBracketCounter = 0;
                closeBracketCounter = 0;

                for (var i = 0; i < line.Length; i++)
                {
                    if (line[i] == '<')
                    {
                        openBracketCounter++;
                    }
                    else if (line[i] == '>')
                    {
                        closeBracketCounter++;
                    }

                    if (openBracketCounter - closeBracketCounter > 1 || openBracketCounter < closeBracketCounter)
                    {
                        var index = textBox.Text.IndexOf(line, textLenght - line.Length);

                        textBox.Select(index, line.Length);
                        textBox.SelectionColor = Color.Red;

                        return false;
                    }
                }

                if (openBracketCounter != closeBracketCounter)
                {
                    var index = textBox.Text.IndexOf(line, textLenght - line.Length);

                    textBox.Select(index, line.Length);
                    textBox.SelectionColor = Color.Red;

                    return false;
                }
            }

            return true;
        }

        public override void Formatter(SynchronizedScrollTextBox textBox)
        {
            output = new StringBuilder();

            var tagNestingLevel = 0;
            StringComparison comp = StringComparison.InvariantCultureIgnoreCase;

            for (int i = 0; i < textBox.Text.Length; i++)
            {
                if (textBox.Text[i] == '<')
                {
                    foreach (var tag in PairedTags)
                    {
                        if (i + tag.Length + 1 < textBox.Text.Length && textBox.Text.Substring(i, tag.Length + 1).Equals("<" + tag, comp))
                        {
                            tagNestingLevel++;
                        }
                        else if (i + tag.Length + 2 < textBox.Text.Length && tagNestingLevel != 0 && textBox.Text.Substring(i, tag.Length + 2).Equals("</" + tag, comp))
                        {
                            tagNestingLevel--;
                        }
                    }
                }

                if (textBox.Text[i] == '\n')
                {
                    output.Append('\n');

                    while (textBox.Text[i + 1] == ' ')
                    {
                        i++;
                    }

                    for (var j = 0; j < tagNestingLevel; j++)
                    {
                        if (textBox.Text.Substring(i + 1, 2).Equals("</", comp) && j == 0)
                        {
                            j++;
                            if (tagNestingLevel == 1)
                            {
                                continue;
                            }
                        }
                        output.Append("    ");
                    }
                }
                else
                {
                    output.Append(textBox.Text[i]);
                }
            }

            textBox.Text = output.ToString();
        }
    }

    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(args));
        }
    }
}
