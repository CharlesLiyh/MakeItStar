using System.Windows;
using System.IO;
using System;

namespace MakeItStar
{
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
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
    }
}
