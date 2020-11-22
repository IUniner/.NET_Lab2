using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace CA.NET_Lab2
{
    class Program
    {
        static readonly private byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        static FileInfo currentFile = null;
        static FileInfo currentArchive = null;
        static readonly string SourceDirectory = "C:\\ClientData\\data\\";
        static readonly string TargetDirectory = "C:\\ServerData\\archive\\";
        static DirectoryInfo DirGen = new DirectoryInfo("C:\\");
        static void Main()
        {
            //DirectoryInfo DirGen = new DirectoryInfo(path + "12\\");
            //FileSystemWatcher watcher = new FileSystemWatcher(path);
            using (FileSystemWatcher changeWatcher = new FileSystemWatcher(SourceDirectory))
            {
                List<FileSystemWatcher> watchers = new List<FileSystemWatcher>
                {
                    changeWatcher,
                    new FileSystemWatcher(TargetDirectory)
                };
                watchers[0].EnableRaisingEvents = true;
                watchers[0].IncludeSubdirectories = true;
                watchers[1].EnableRaisingEvents = true;
                watchers[1].IncludeSubdirectories = true;

                watchers[0].Changed += changeWatcher_Changed;
                watchers[1].Changed += changeWatcher_Changed;
                watchers[0].Created += changeWatcher_Created;
                watchers[1].Created += changeWatcher_Created;
                watchers[0].Deleted += changeWatcher_Deleted;
                watchers[1].Deleted += changeWatcher_Deleted;
                watchers[0].Renamed += changeWatcher_Renamed;
                watchers[1].Renamed += changeWatcher_Renamed;

                Console.Read();
            }
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
            if (e.FullPath.IndexOf(".") != -1)
            {
                Console.WriteLine("File|> {0} changed at time: {1}", e.FullPath, DateTime.Now.ToLocalTime());
                fileHandler(e.FullPath);
            }
        }
        static void fileHandler(string eFPath)
        {
            try
            {
                if (eFPath.IndexOf(".txt") !=-1 && eFPath.IndexOf(SourceDirectory) != -1)
                {
                    currentFile = new FileInfo(eFPath);
                    if (eFPath.IndexOf("AES.txt") == -1) //&& currentFile.FullName.IndexOf("_AES.gz") == -1)
                        currentFile = Encryption(currentFile);

                    currentArchive = Compress(currentFile);
                    //if (currentFile.Exists)
                    //   File.Delete(currentFile.FullName);

                    if (currentArchive.FullName.IndexOf("AES.gz") > 0)
                    {
                        int ifSpace = currentArchive.Name.IndexOf("_") + 1;
                        //System. fileTime = new DateTime();
                        //fileTime.Year = currentArchive.Name.Substring(ifSpace, 4);
                        DirGen = new DirectoryInfo(TargetDirectory + currentArchive.Name.Substring(ifSpace, 4) + "\\" + currentArchive.Name.Substring(ifSpace + 5, 2) + "\\" + currentArchive.Name.Substring(ifSpace + 8, 2));
                        if (!DirGen.Exists)
                            DirGen.Create();
                        if (!new FileInfo(DirGen.FullName + "\\" + currentArchive.Name).Exists)
                        {
                            File.Move(currentArchive.FullName, DirGen.FullName + "\\" + currentArchive.Name); //currentArchive.CopyTo(DirGen.FullName + "\\" + currentArchive.Name);
                            //File.Delete(DirGen.FullName.Replace(TargetDirectory, SourceDirectory) + "\\" + currentFile.Name.Replace(".gz", ".txt"));
                        }
                        if (currentArchive.Exists)
                        {
                            //File.Delete(currentArchive.FullName);
                        }
                        if (new FileInfo(DirGen.FullName.Replace(TargetDirectory, SourceDirectory) + "\\" + currentFile.Name).Exists)
                        {
                            File.Delete(currentFile.FullName);
                        }
                    }
                    if (currentFile.FullName.IndexOf("AES.txt") > 0 && eFPath.IndexOf(SourceDirectory) != -1) //&& currentFile.FullName.IndexOf("_AES.gz") == -1)
                    {
                        //File.Delete(currentFile.FullName);
                    }
                }

                if (eFPath.IndexOf("AES.gz")>0 && eFPath.IndexOf(TargetDirectory) != -1) //e.FullPath.Substring(e.FullPath.IndexOf(TargetDirectory),TargetDirectory.Length) == TargetDirectory
                {
                    //currentFile = Decompress(new FileInfo(eFPath));
                    //Decryption(currentFile);
                    Decryption(Decompress(new FileInfo(eFPath)));
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
                currentArchive = fileToCompress;
                if ((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden)
                    != FileAttributes.Hidden &
                   fileToCompress.Extension != ".gz")
                {
                    using (FileStream originalFileStream = fileToCompress.OpenRead())   // or new FileStream(fileToCompress.FullName, FileMode.OpenOrCreate))
                    {
                        using (FileStream compressedFileStream = File.Create(fileToCompress.FullName.Replace(".txt", ".gz")))
                        {
                            using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                            {
                                originalFileStream.CopyTo(compressionStream);
                            }
                        }
                    }
                    currentArchive = new FileInfo(fileToCompress.FullName.Replace(".txt", ".gz"));
                }            
                return currentArchive;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Compress error:" + ex.Message);
                return currentArchive;
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
                        using (FileStream decompressedFileStream = File.Create(fileToDecompress.FullName.Replace(".gz", ".txt")))
                        {
                            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                            {
                                decompressionStream.CopyTo(decompressedFileStream);
                            }
                        }                       
                    }
                    currentArchive = new FileInfo(fileToDecompress.FullName.Replace(".gz", ".txt"));
                    fileToDecompress.Delete();
                }
                return currentArchive;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Decompress error:" + ex.Message);
                return fileToDecompress;
            }
        }
        static FileInfo Encryption(FileInfo fileToEncryption)
        {
            try
            {
                currentFile = fileToEncryption;
                if (fileToEncryption.Extension == ".txt")
                {
                    using (FileStream originalFileStream = fileToEncryption.OpenRead())
                    {
                        using (FileStream cryptedFileStream = File.Create(fileToEncryption.FullName.Replace(".txt", "_AES.txt")))
                        {
                            Aes aes = Aes.Create();
                            aes.Key = key;
                            byte[] iv = aes.IV;

                            cryptedFileStream.Write(iv, 0, iv.Length);
                            using (CryptoStream encryptionStream = new CryptoStream(cryptedFileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                originalFileStream.CopyTo(encryptionStream);
                            }
                        }
                    }
                    currentFile = new FileInfo(fileToEncryption.FullName.Replace(".txt", "_AES.txt"));
                }
                return currentFile;
            }
            catch(FileNotFoundException ex)
            {
                Console.WriteLine("Encryption error:" + ex.Message);
                return fileToEncryption;
            }
        }
        static FileInfo Decryption(FileInfo fileToDecryption)
        {
            try
            {
                currentFile = fileToDecryption;
                if (fileToDecryption.Name.IndexOf("AES.txt") >0)
                {
                    using (FileStream originalFileStream = fileToDecryption.OpenRead())
                    {
                        Aes aes = Aes.Create();
                        byte[] iv = new byte[aes.IV.Length];

                        originalFileStream.Read(iv, 0, iv.Length);
                        using (FileStream decryptedFileStream = File.Create(fileToDecryption.FullName.Replace("_AES.txt", ".txt")))
                        {
                            using (CryptoStream decryptionFileStream = new CryptoStream(originalFileStream, aes.CreateDecryptor(key,iv), CryptoStreamMode.Read))
                            {
                                decryptionFileStream.CopyTo(decryptedFileStream);
                            }
                        }
                    }
                    currentFile = new FileInfo(fileToDecryption.FullName.Replace("_AES.txt", ".txt"));
                    fileToDecryption.Delete();
                }
                return currentFile;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Decryption error:" + ex.Message);
                return fileToDecryption;
            }
        }

    }

}