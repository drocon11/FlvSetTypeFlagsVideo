
using System;
using System.IO;

class App
{
    public static void Main(string[] args)
    {
        try
        {
            foreach (string arg in args)
            {
                if (File.Exists(arg))
                {
                    DoFile(new FileInfo(arg));
                }
                else if (Directory.Exists(arg))
                {
                    DoDirectory(new DirectoryInfo(arg));
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    static void DoFile(FileInfo file)
    {
        if (String.Equals(file.Extension, ".flv", StringComparison.OrdinalIgnoreCase))
        {
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                byte[] header = new byte[9]; // header size = 9 for FLV version 1
                if (fs.Read(header, 0, header.Length) == header.Length)
                {
                    if (header[0] == 'F' && header[1] == 'L' && header[2] == 'V' &&
                        header[3] == 0x01 && (header[4] & 0x01) == 0x00)
                    {
                        header[4] |= 0x01; // TypeFlagsVideo (0x01), TypeFlagsAudio (0x04)
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.Write(header, 0, header.Length);
                    }
                }
            }
        }
    }

    static void DoDirectory(DirectoryInfo dir)
    {
        foreach (FileInfo f in dir.GetFiles())
        {
            DoFile(f);
        }
        foreach (DirectoryInfo d in dir.GetDirectories())
        {
            DoDirectory(d);
        }
    }
}
