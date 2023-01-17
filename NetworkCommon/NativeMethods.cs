using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NetworkCommon
{
    public static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        public const int WM_VSCROLL = 277; // Vertical scroll
        public const int SB_BOTTOM = 7; // Scroll to bottom 


        public static void AppendText(RichTextBox box, string line)
        {
            try
            {
                box.AppendText(line + Environment.NewLine);
                ScrollRichTextBox(box);
            }
            catch
            {
            }
        }

        public static void ScrollRichTextBox(RichTextBox box)
        {
            if (box == null || box.IsDisposed || box.Disposing)
                return;
            SendMessage(box.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero);
        }
    }
}
