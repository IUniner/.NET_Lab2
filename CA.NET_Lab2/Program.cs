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
        static string SourceDirectory = "C:\\ClientData\\data\\";
        static string TargetDirectory = "C:\\ServerData\\archive\\";
        static DirectoryInfo DirGen = new DirectoryInfo("C:\\");
        static void Main(string[] args)
        {

            //DirectoryInfo DirGen = new DirectoryInfo(path + "12\\");
            //FileSystemWatcher watcher = new FileSystemWatcher(path);

            using (FileSystemWatcher changeWatcher = new FileSystemWatcher(SourceDirectory))
            {
                List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
                watchers.Add(changeWatcher);
                watchers.Add(new FileSystemWatcher(TargetDirectory));
                //FileInfo selectFile = new FileInfo(e.FullPath)
                watchers[0].EnableRaisingEvents = true;
                watchers[0].IncludeSubdirectories = true;
                watchers[1].EnableRaisingEvents = true;
                watchers[1].IncludeSubdirectories = true;


                watchers[0].Changed += changeWatcher_Changed;
                watchers[0].Created += changeWatcher_Created;
                watchers[0].Deleted += changeWatcher_Deleted;
                watchers[0].Renamed += changeWatcher_Renamed;

                watchers[1].Changed += changeWatcher_Changed;
                watchers[1].Created += changeWatcher_Created;
                watchers[1].Deleted += changeWatcher_Deleted;
                watchers[1].Renamed += changeWatcher_Renamed;

                Console.Read();
            }
            /*using (FileSystemWatcher changeWatcher = new FileSystemWatcher(TargetDirectory))
            {
                //FileInfo selectFile = new FileInfo(e.FullPath)
                changeWatcher.EnableRaisingEvents = true;
                changeWatcher.IncludeSubdirectories = true;

                changeWatcher.Changed += changeWatcher_Changed;
                changeWatcher.Created += changeWatcher_Created;
                changeWatcher.Deleted += changeWatcher_Deleted;
                changeWatcher.Renamed += changeWatcher_Renamed;

                Console.Read();
            }*/
        }
        static void changeWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine("File|> {0} renamed to {1} at time: {2}", e.OldName, e.FullPath, DateTime.Now.ToLocalTime());
            fileHandler(e.FullPath);
        }
        static void changeWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (!(new FileInfo(e.FullPath).Extension == ".gz" & e.FullPath.IndexOf(SourceDirectory)!=-1))
            {
                Console.WriteLine("File|> {0} deleted at time: {1}", e.FullPath, DateTime.Now.ToLocalTime());
            }   
        }
        static void changeWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (!(new FileInfo(e.FullPath).Extension == ".gz" & e.FullPath.IndexOf(SourceDirectory) != -1))
            {
                Console.WriteLine("File|> {0} created at time: {1} {2}", e.FullPath, DateTime.Now.ToLocalTime(), DateTime.Now.Month);
            }
            fileHandler(e.FullPath);
        }
        static void changeWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File|> {0} changed at time: {1}", e.FullPath, DateTime.Now.ToLocalTime());
            fileHandler(e.FullPath);
        }
        static void fileHandler(string eFPath)
        {
            try
            {
                if ((currentArchive = new FileInfo(eFPath)).Extension == ".txt" && eFPath.IndexOf(SourceDirectory) != -1)
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
                if ((currentArchive = new FileInfo(eFPath)).Extension == ".gz" && eFPath.IndexOf(TargetDirectory) != -1) //e.FullPath.Substring(e.FullPath.IndexOf(TargetDirectory),TargetDirectory.Length) == TargetDirectory
                {
                    Decompress(new FileInfo(eFPath));
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("fileHandler exeption: " + ex.Message);
            }
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
                return fileToCompress;
            }
        }
        static FileInfo Decompress(FileInfo fileToDecompress)
        {
            try
            {
                currentArchive = fileToDecompress;
                if (fileToDecompress.Extension == ".gz")
                {
                    using (FileStream originalFileStream = fileToDecompress.OpenRead())
                    {
                        //string currentFileName = fileToDecompress.FullName;
                        //string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);
                        using (FileStream decompressedFileStream = File.Create(fileToDecompress.FullName.Replace(".gz", ".txt")))
                        {
                            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                            {
                                decompressionStream.CopyTo(decompressedFileStream);
                            }
                        }
                        currentArchive = new FileInfo(fileToDecompress.FullName.Replace(".gz", ".txt"));
                        fileToDecompress.Delete();
                    }
                }
                return currentArchive;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Decompress error:" + ex.Message);
                return fileToDecompress;
            }
        }
    }
}