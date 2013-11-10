using System.Windows;
using System.IO;
using System;
using System.Windows.Interop;

namespace MakeItStar
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Next clipboard viewer window 
        /// </summary>
        private IntPtr hWndNextViewer;

        /// <summary>
        /// The <see cref="HwndSource"/> for this window.
        /// </summary>
        private HwndSource hWndSource;
        
        public MainWindow() {
            InitializeComponent();
        }

        private void InitCBViewer()
        {
            WindowInteropHelper wih = new WindowInteropHelper(this);
            hWndSource = HwndSource.FromHwnd(wih.Handle);

            hWndSource.AddHook(this.WndProc);   // start processing window messages
            hWndNextViewer = Win32.SetClipboardViewer(hWndSource.Handle);   // set this window as a viewer
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            InitCBViewer();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected void OnDrop(object sender, DragEventArgs e) {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            FileInfo fi = new FileInfo(fileName);

            string star = "【★】";
            string dstName;
            if (fi.Name.StartsWith(star))
            {
                dstName = fi.Name.Substring(3);
            }
            else
            {
                dstName = star + fi.Name;
            }

            string dstPath = string.Format("{0}\\{1}", fi.DirectoryName, dstName);
            if (File.Exists(dstPath))
                File.Delete(dstPath);

            fi.MoveTo(dstPath);
        }

        private void ReplaceText()
        {
            string textFromClipboard = Clipboard.GetText();
            string newText = textFromClipboard.Replace(':', ' ');
            newText = newText.Replace('/', ' ');
            
            Clipboard.Clear();
            Clipboard.SetText(newText);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WM_CHANGECBCHAIN:
                    if (wParam == hWndNextViewer)
                    {
                        // clipboard viewer chain changed, need to fix it.
                        hWndNextViewer = lParam;
                    }
                    else if (hWndNextViewer != IntPtr.Zero)
                    {
                        // pass the message to the next viewer.
                        Win32.SendMessage(hWndNextViewer, msg, wParam, lParam);
                    }
                    break;

                case Win32.WM_DRAWCLIPBOARD:
                    // clipboard content changed
                    if (Clipboard.ContainsText())
                    {
                        ReplaceText();
                    }
                    // pass the message to the next viewer.
                    Win32.SendMessage(hWndNextViewer, msg, wParam, lParam);
                    break;
            }

            return IntPtr.Zero;
        }
    }
}
