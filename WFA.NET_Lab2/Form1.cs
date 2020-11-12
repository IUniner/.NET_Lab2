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

namespace WFA.NET_Lab2
{
    public partial class Form1 : Form
    {
        static string path = "C:\\ClientData\\2020\\";
        DirectoryInfo DirGen = new DirectoryInfo(path + "12\\");
        FileSystemWatcher watcher = new FileSystemWatcher(path);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DirectoryInfo DirGen = new DirectoryInfo(path+ "December\\");
            //DirGen.CreateSubdirectory("December");
            //DirGen.MoveTo("C:\\ClientData\\2021\\February\\"); // from DirectoryInfo DirGen = new DirectoryInfo("C:\\ClientData\\2020\\" + "February\\");
            for (int i = 1; i <= 31; ++i) ;
            //DirGen.CreateSubdirectory(i.ToString());
            //DirGen

            using(FileSystemWatcher changeWatcher = new FileSystemWatcher())
            {
                changeWatcher.EnableRaisingEvents = true;
                changeWatcher.IncludeSubdirectories = true;

                changeWatcher.Changed += changeWatcher_Changed;
                changeWatcher.Created += changeWatcher_Created;
                changeWatcher.Deleted += changeWatcher_Deleted;
                changeWatcher.Renamed += changeWatcher_Renamed;

                Console.Read();
            }
        }

        static void changeWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine("File: {0} renamed to {1} at time: {2}", e.OldName, e.Name, DateTime.Now.ToLocalTime());
        }
        static void changeWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }
        static void changeWatcher_Created(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }
        static void changeWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            
        }
    }
}
