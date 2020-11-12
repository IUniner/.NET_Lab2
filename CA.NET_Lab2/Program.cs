using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Text;


namespace CA.NET_Lab2
{
    class Program
    {
        //System.Console.InputEncoding = Encoding.GetEncoding(1251);
        static FileInfo currentFile = null;
        static FileInfo currentArchive = null;
        static string SourceDirectory = "C:\\ClientData\\";
        static string TargetDirectory = "C:\\ServerData\\";
        static DirectoryInfo DirGen = new DirectoryInfo("C:\\");
        static void Main(string[] args)
        {
            
            //DirectoryInfo DirGen = new DirectoryInfo(path + "12\\");
            //FileSystemWatcher watcher = new FileSystemWatcher(path);

            using (FileSystemWatcher changeWatcher = new FileSystemWatcher(SourceDirectory))
            {
                //FileInfo selectFile = new FileInfo(e.FullPath)
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
            Console.WriteLine("File: {0} renamed to {1} at time: {2}", e.OldName, e.FullPath, DateTime.Now.ToLocalTime());
        }
        static void changeWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File: {0} deleted at time: {1}", e.FullPath, DateTime.Now.ToLocalTime());
        }
        static void changeWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                Console.WriteLine("File: {0} created at time: {1} {2}", e.FullPath, DateTime.Now.ToLocalTime(), DateTime.Now.Month);
                if ((currentArchive = new FileInfo(e.FullPath)).Extension == ".txt")
                {
                    currentArchive = Compress(currentArchive);
                    if (currentArchive.Extension == ".gz")
                    {
                        int ifSpace = currentArchive.Name.IndexOf("_") + 1;
                        //System. fileTime = new DateTime();
                        //fileTime.Year = currentArchive.Name.Substring(ifSpace, 4);
                        DirGen = new DirectoryInfo(TargetDirectory + currentArchive.Name.Substring(ifSpace, 4) + "\\" + currentArchive.Name.Substring(ifSpace + 5, 2) + "\\" + currentArchive.Name.Substring(ifSpace + 8, 2));
                        if (!DirGen.Exists)
                            DirGen.Create();
                        File.Move(currentArchive.FullName, DirGen.FullName + "\\" + currentArchive.Name); //currentArchive.CopyTo(DirGen.FullName + "\\" + currentArchive.Name);
                        File.Delete(currentArchive.FullName);
                    }
                }
            }
            catch(FileNotFoundException ex)
            {
                Console.WriteLine("changeWatcher_Created exeption: " + ex.Message);
            }
        }
        static void changeWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File: {0} changed at time: {1}", e.FullPath, DateTime.Now.ToLocalTime());
        }
        static void sendFile(FileInfo currentFile)
        {

        }

        static FileInfo Compress(FileInfo fileToCompress)
        {
            try
            {
                currentFile = fileToCompress;
                using (FileStream originalFileStream = fileToCompress.OpenRead())
                {
                    if ((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) 
                        != FileAttributes.Hidden & 
                       fileToCompress.Extension != ".gz")
                    {
                        using (FileStream compressedFileStream = File.Create(fileToCompress.FullName.Replace(".txt", ".gz")))
                        {
                            using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                            {
                                originalFileStream.CopyTo(compressionStream);
                            }
                        }
                        currentFile = new FileInfo(fileToCompress.FullName.Replace(".txt", ".gz"));
                    }
                    return currentFile;
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Compress error:" + ex.Message);
                return currentFile;
            }
        }
    }
}
