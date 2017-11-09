using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiagnosticTool.GUIViews
{
    public enum GUI_MessageType
    {
        Incoming,
        Outgoing,
        Outgoing_sniffer,
        Normal,
        Warning,
        Error
    };

    public class DisplayFormatData
    {
        private Color[] MessageColor = { Color.Blue, Color.Green, Color.OrangeRed, Color.Black, Color.YellowGreen, Color.Red};

        private RichTextBox RichTextBoxContainer;

        public DisplayFormatData(RichTextBox richTextBox)
        {
            RichTextBoxContainer = richTextBox;
        }

        public void displayData(GUI_MessageType type, string message)
        {
            int text_start;
            int text_end;

            text_start = RichTextBoxContainer.TextLength;
            RichTextBoxContainer.AppendText(message);
            text_end = RichTextBoxContainer.TextLength;
            RichTextBoxContainer.Select(text_start, text_end - text_start);
            RichTextBoxContainer.SelectionFont = new Font("Courier New", 9, FontStyle.Bold);
            RichTextBoxContainer.SelectionColor = MessageColor[(int)type];
            RichTextBoxContainer.SelectionLength = 0;
            RichTextBoxContainer.ScrollToCaret();
        }
    }
}
