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

namespace Text_Packet
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }

        OpenFileDialog opf = new OpenFileDialog();

        private void btnConvert_Click(object sender, EventArgs e)
        {
            opf.Filter = "All file(*.FileSizeTable;*.BIN)|*.FileSizeTable;*.bin";
            if(opf.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(opf.FileName))
                {
                    if(Path.GetExtension(opf.FileName) == ".bin")
                    {
                        string dir = Path.GetDirectoryName(opf.FileName);
                        string filename = Path.GetFileNameWithoutExtension(opf.FileName);
                        BinaryReaderBE rd = new BinaryReaderBE(new FileStream(opf.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                        switch (rd.ReadByte())
                        {
                            case 0:
                                Script_Text(rd, filename, dir);
                                break;
                            case 0xFE:
                                Text_dat(rd, filename, dir);
                                break;
                            default: Console.WriteLine("File not support!");
                                break;
                        }
                    }
                    else
                    {
                        string dir = Path.GetDirectoryName(opf.FileName);
                        string filename = Path.GetFileNameWithoutExtension(opf.FileName);
                        string[] lines = File.ReadAllLines(opf.FileName);
                        switch (lines[0])
                        {
                            case "var=0":
                                Repack_dat(lines, filename, dir);
                                break;
                            case "var=1":
                                Repack_script(lines, filename, dir);
                                break;
                            default:
                                Console.WriteLine("File not support!");
                                break;
                        }
                    }
                }
            }
        }

        ////text_dat.bin
        public void Text_dat(BinaryReader rd, string filename, string dir)
        {
            StreamWriter wtFs = new StreamWriter(dir + "\\" + filename + ".FileSizeTable");
            wtFs.WriteLine("var=0");
            if (!Directory.Exists(dir + "\\" + filename))
                Directory.CreateDirectory(dir + "\\" + filename);
            rd.BaseStream.Seek(16, SeekOrigin.Begin);
            rd.ReadInt16();
            int count = rd.ReadInt16();
            int skip_byte = rd.ReadInt32();
            Dictionary<int, int> dic = new Dictionary<int, int>();
            for (int i = 0; i < count; i++)
            {
                dic.Add(rd.ReadInt32(), rd.ReadInt32());
            }
            foreach (KeyValuePair<int, int> pair in dic)
            {
                byte[] out_buffer = rd.ReadBytes(pair.Value);
                BinaryReaderBE rdms = new BinaryReaderBE(new MemoryStream(out_buffer));
                StreamWriter wt = new StreamWriter(dir + "\\" + filename + "\\0x" + pair.Key.ToString("X") + ".txt");
                List<int> offset_line = new List<int>();
                rdms.BaseStream.Seek(rdms.ReadInt64() - 16, SeekOrigin.Begin);
                int line = rdms.ReadInt32();
                rdms.ReadBytes(12);
                for (int i = 0; i < line + 2; i++)
                {
                    try
                    {
                        int ofb = rdms.ReadInt32();
                        if (ofb != 0)
                            offset_line.Add(ofb);
                        else
                            rdms.BaseStream.Seek(16, SeekOrigin.Begin);
                    }
                    catch
                    {
                        rdms.BaseStream.Seek(16, SeekOrigin.Begin);
                    }
                }
                offset_line.Sort();
                for (int i = 0; i < line; i++)
                {
                    rdms.BaseStream.Seek(offset_line[i], SeekOrigin.Begin);
                    byte[] txt = rdms.ReadBytes(offset_line[i + 1] - offset_line[i]);
                    StringBuilder sb = new StringBuilder(Encoding.BigEndianUnicode.GetString(txt));
                    sb.Replace("\r", "[r]");
                    sb.Replace("\n", "[n]");
                    sb.Replace("\0", "[0]");
                    Console.WriteLine(sb.ToString());
                    wt.WriteLine(sb.ToString());

                }
                wt.Close();
                BinaryWriter wt_binary = new BinaryWriter(File.Create(dir + "\\" + filename + "\\0x" + pair.Key.ToString("X") + ".bin"));
                wt_binary.Write(out_buffer);
                wt_binary.Close();
                rdms.Close();
                wtFs.WriteLine(dir + "\\" + filename + "\\0x" + pair.Key.ToString("X") + ".bin");
            }
            wtFs.Close();
            Console.WriteLine("\n********************************\n*Get done. Continue............*\n********************************");
        }

        ////script.bin
        public void Script_Text(BinaryReader rd, string filename, string dir)
        {
            StreamWriter wtFs = new StreamWriter(dir + "\\" + filename + ".FileSizeTable");
            wtFs.WriteLine("var=1");
            if (!Directory.Exists(dir + "\\" + filename))
                Directory.CreateDirectory(dir + "\\" + filename);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            long len = rd.ReadInt64();
            rd.BaseStream.Seek(len - 16, SeekOrigin.Begin);
            int count = rd.ReadInt32();
            rd.BaseStream.Seek(len, SeekOrigin.Begin);
            List<int[]> offset = new List<int[]>();
            for (int i = 0; i < count; i++)
            {
                int[] info = new int[3];
                if(i == 1)
                {
                    info[0] = rd.ReadInt32();
                    rd.BaseStream.Seek(16, SeekOrigin.Begin);
                    info[1] = rd.ReadInt32();
                    info[2] = rd.ReadInt32();
                }
                else
                {
                    info[0] = rd.ReadInt32();
                    info[1] = rd.ReadInt32();
                    info[2] = rd.ReadInt32();
                }
                offset.Add(info);
            }
            for(int i = 0; i < count; i++)
            {
                BinaryWriter wt = new BinaryWriter(new FileStream(dir + "\\" + filename + "\\0x" + offset[i][0].ToString("X8") + ".bin", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));
                rd.BaseStream.Seek(offset[i][0], SeekOrigin.Begin);
                wt.Write(rd.ReadBytes((offset[i][2])));
                wt.Close();
                wtFs.WriteLine("0x" + offset[i][1].ToString("X8") + " " + dir + "\\" + filename + "\\0x" + offset[i][0].ToString("X8") + ".bin");
                if(i == 6)
                {
                    BinaryReaderBE rdtxt = new BinaryReaderBE(File.OpenRead(dir + "\\" + filename + "\\0x" + offset[i][0].ToString("X8") + ".bin"));
                    StreamWriter wttxt = new StreamWriter(dir + "\\" + filename + "\\0x" + offset[i][0].ToString("X8") + ".txt");
                    List<int> bf = new List<int>();
                    long nx = rdtxt.ReadInt64();
                    rdtxt.BaseStream.Seek(nx - 16, SeekOrigin.Begin);
                    int cn = rdtxt.ReadInt32();
                    rdtxt.BaseStream.Seek(nx, SeekOrigin.Begin);
                    for (int j = 0; j < cn; j++)
                    {
                        if(j == 1)
                        {
                            int k = rdtxt.ReadInt32();
                            bf.Add(k);
                            rdtxt.ReadInt32();
                            k = rdtxt.ReadInt32();
                            bf.Add(k);
                            rdtxt.BaseStream.Seek(16, SeekOrigin.Begin);
                        }
                        else
                        {
                            int k = rdtxt.ReadInt32();
                            bf.Add(k);
                            rdtxt.ReadInt32();
                        }
                    }
                    bf.Sort();
                    for (int j = 0; j < cn; j++)
                    {
                        rdtxt.BaseStream.Seek(bf[j], SeekOrigin.Begin);
                        byte[] txt = rdtxt.ReadBytes(bf[j + 1] - bf[j]);
                        StringBuilder sb = new StringBuilder(Encoding.BigEndianUnicode.GetString(txt));
                        sb.Replace("\r", "[r]");
                        sb.Replace("\n", "[n]");
                        sb.Replace("\0", "[0]");
                        Console.WriteLine(sb.ToString());
                        wttxt.WriteLine(sb.ToString());
                    }
                    wttxt.Close();
                    rdtxt.Close();
                }
            }
            wtFs.Close();
        }

        ////var=0
        public void Repack_dat(string[] lines, string filename, string dir)
        {
            if(!File.Exists(dir + "\\" + filename + ".bin_bk"))
            {
                Copy(dir + "\\" + filename + ".bin", dir + "\\" + filename + ".bin_bk");
                Console.WriteLine("Backup file {0}...Successfully", dir + "\\" + filename + ".dat");
            }
            var dump = new BinaryWriter(File.Create("dump.temp"));
            var wtmain = new BinaryWriter(new FileStream(dir + "\\" + filename + ".bin", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));
            wtmain.Write(new byte[]{ 0xfe, 0xfe});
            wtmain.Write(WriteInt16BE(1));
            wtmain.Write(WriteInt32BE(16));
            wtmain.BaseStream.Seek(16, SeekOrigin.Begin);
            wtmain.Write(new byte[] { 0xfe, 0xfe });
            wtmain.Write(WriteInt16BE((short)(lines.Length - 1)));
            wtmain.Write(WriteInt32BE((lines.Length - 1) * 8 + 8));
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != "var=0")
                {
                    if (!File.Exists(lines[i] + "_bk"))
                    {
                        Copy(lines[i], lines[i] + "_bk");
                        Console.WriteLine("Backup file {0}...Successfully", Path.GetFileName(lines[i]));
                    }
                    var wt = new BinaryWriter(new FileStream(lines[i], FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));
                    //var rd = new BinaryReaderBE(new FileStream(lines[i] + "_bk", FileMode.Open, FileAccess.Read, FileShare.Read));
                    string[] text_file = File.ReadAllLines(Path.GetDirectoryName(lines[i]) + "\\" + Path.GetFileNameWithoutExtension(lines[i]) + ".txt");
                    List<int> offset = new List<int>();
                    wt.BaseStream.Seek(text_file.Length * 4, SeekOrigin.Begin);
                    offset.Add(text_file.Length * 4);
                    foreach(string line in text_file)
                    {
                        Console.WriteLine("Importing....00--->" + line);
                        StringBuilder sb = new StringBuilder(line);
                        sb.Replace("[r]", "\r");
                        sb.Replace("[n]", "\n");
                        sb.Replace("[0]", "\0");
                        wt.Write(Encoding.BigEndianUnicode.GetBytes(sb.ToString()));
                        offset.Add((int)wt.BaseStream.Position);
                        Console.WriteLine("Successfully!");
                    }
                    int end = (int)wt.BaseStream.Position;
                    wt.Write(WriteInt64BE(text_file.Length));
                    wt.Write(WriteInt32BE(4));
                    wt.Write(WriteInt32BE(1));
                    wt.Write(WriteInt32BE(0));
                    int end2 = (int)wt.BaseStream.Position;
                    for(int j = 0; j < text_file.Length; j++)
                    {
                        if(j == 4)
                        {
                            wt.Write(WriteInt32BE(end));
                            SkipWriter(wt);
                            wt.BaseStream.Seek(0, SeekOrigin.Begin);
                            wt.Write(WriteInt64BE(end2));
                            wt.Write(WriteInt32BE(1));
                            wt.Write(WriteInt32BE(0));
                            wt.Write(WriteInt32BE(offset[j]));
                        }
                        else
                        {
                            wt.Write(WriteInt32BE(offset[j]));
                        }
                    }
                    wt.Close();
                    byte[] all = File.ReadAllBytes(lines[i]);
                    wtmain.Write(WriteInt32BE((int)dump.BaseStream.Position));
                    wtmain.Write(WriteInt32BE(all.Length));
                    dump.Write(all);
                }
            }
            dump.Close();
            var rdd = new BinaryReader(File.OpenRead("dump.temp"));
            wtmain.Write(rdd.ReadBytes((int)rdd.BaseStream.Length));
            rdd.Close();
            File.Delete("dump.temp");
            SkipWriter(wtmain);
            long enddat = wtmain.BaseStream.Position;
            wtmain.BaseStream.Seek(8, SeekOrigin.Begin);
            wtmain.Write(WriteInt64BE(enddat - 16));
            wtmain.Close();
            Console.WriteLine("\n***********************************\n*Pack Done!!. Continue............*\n***********************************");
        }

        ////var=1
        public void Repack_script(string[] lines, string filename, string dir)
        {
            if (!File.Exists(dir + "\\" + filename + ".bin_bk"))
            {
                Copy(dir + "\\" + filename + ".bin", dir + "\\" + filename + ".bin_bk");
                Console.WriteLine("Backup file {0}...Successfully", dir + "\\" + filename + ".dat");
            }
            var wtmain = new BinaryWriter(new FileStream(dir + "\\" + filename + ".bin", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));
            wtmain.BaseStream.Seek(8, SeekOrigin.Begin);
            wtmain.Write(WriteInt32BE(1));
            wtmain.Write(WriteInt16BE(0));
            wtmain.BaseStream.Seek((lines.Length - 1) * 12, SeekOrigin.Begin);
            List<int[]> off1 = new List<int[]>();
            for(int i = 0; i < lines.Length; i++)
            {
                if(lines[i] != "var=1")
                {
                    if (!File.Exists(lines[i].Split(' ')[1] + "_bk") && i == 7)
                    {
                        Copy(lines[i].Split(' ')[1], lines[i].Split(' ')[1] + "_bk");
                        Console.WriteLine("Backup file {0}...Successfully", Path.GetFileName(lines[i].Split(' ')[1]));
                    }

                    if (i != 7)
                    {
                        int[] dk = new int[3];
                        byte[] all = File.ReadAllBytes(lines[i].Split(' ')[1]);
                        dk[0] = (int)wtmain.BaseStream.Position;
                        dk[1] = Convert.ToInt32(lines[i].Split(' ')[0],16);
                        dk[2] = all.Length;
                        off1.Add(dk);
                        wtmain.Write(all);
                    }
                    else
                    {
                        var wt = new BinaryWriter(new FileStream(lines[i].Split(' ')[1], FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));
                        string[] text_file = File.ReadAllLines(Path.GetDirectoryName(lines[i].Split(' ')[1]) + "\\" + Path.GetFileNameWithoutExtension(lines[i].Split(' ')[1]) + ".txt");
                        List<int> offset = new List<int>();
                        wt.BaseStream.Seek(text_file.Length * 8, SeekOrigin.Begin);
                        offset.Add(text_file.Length * 8);
                        foreach (string line in text_file)
                        {
                            Console.WriteLine("Importing....00--->" + line);
                            StringBuilder sb = new StringBuilder(line);
                            sb.Replace("[r]", "\r");
                            sb.Replace("[n]", "\n");
                            sb.Replace("[0]", "\0");
                            wt.Write(Encoding.BigEndianUnicode.GetBytes(sb.ToString()));
                            offset.Add((int)wt.BaseStream.Position);
                            Console.WriteLine("Successfully!");
                        }
                        int end = (int)wt.BaseStream.Position;
                        wt.Write(WriteInt64BE(text_file.Length));
                        wt.Write(WriteInt32BE(8));
                        wt.Write(WriteInt32BE(1));
                        wt.Write(WriteInt32BE(0));
                        int end2 = (int)wt.BaseStream.Position;
                        for (int j = 0; j < text_file.Length; j++)
                        {
                            if (j == 2)
                            {
                                wt.Write(WriteInt32BE(end));
                                wt.BaseStream.Seek(0, SeekOrigin.Begin);
                                wt.Write(WriteInt64BE(end2));
                                wt.Write(WriteInt32BE(1));
                                wt.Write(WriteInt32BE(0));
                                wt.Write(WriteInt32BE(offset[j]));
                                wt.Write(new byte[] { 0xff, 0xff, 0x00, 0x00 });
                            }
                            else if(j < 2)
                            {
                                wt.Write(WriteInt32BE(offset[j]));
                                wt.Write(new byte[] { 0xff, 0xff, 0x00, 0x00 });
                            }
                            else
                            {
                                var rd = new BinaryReaderBE(new FileStream(lines[i].Split(' ')[1] + "_bk", FileMode.Open, FileAccess.Read, FileShare.Read));
                                wt.Write(WriteInt32BE(offset[j]));
                                rd.BaseStream.Seek(wt.BaseStream.Position, SeekOrigin.Begin);
                                wt.Write(rd.ReadBytes(4));
                            }
                        }
                        wt.Close();
                        int[] dk = new int[3];
                        byte[] all = File.ReadAllBytes(lines[i].Split(' ')[1]);
                        dk[0] = (int)wtmain.BaseStream.Position;
                        dk[1] = Convert.ToInt32(lines[i].Split(' ')[0], 16);
                        dk[2] = all.Length;
                        off1.Add(dk);
                        wtmain.Write(all);
                    }
                }
            }
            int end1 = (int)wtmain.BaseStream.Position;
            wtmain.Write(WriteInt64BE(lines.Length - 1));
            wtmain.Write(WriteInt32BE(12));
            wtmain.Write(WriteInt32BE(1));
            wtmain.Write(WriteInt32BE(0));
            int end21 = (int)wtmain.BaseStream.Position;
            for (int j = 0; j < lines.Length - 1; j++)
            {
                if (j == 1)
                {
                    wtmain.Write(WriteInt32BE(off1[j][0]));
                    wtmain.Write(WriteInt32BE(end1));
                    wtmain.BaseStream.Seek(0, SeekOrigin.Begin);
                    wtmain.Write(WriteInt64BE(end21));
                    wtmain.BaseStream.Seek(16, SeekOrigin.Begin);
                    wtmain.Write(WriteInt32BE(off1[j][1]));
                    wtmain.Write(WriteInt32BE(off1[j][2]));
                }
                else
                {
                    wtmain.Write(WriteInt32BE(off1[j][0]));
                    wtmain.Write(WriteInt32BE(off1[j][1]));
                    wtmain.Write(WriteInt32BE(off1[j][2]));
                }
            }
            wtmain.Close();
            Console.WriteLine("\n***********************************\n*Pack Done!!. Continue............*\n***********************************");
        }

        public static void Copy(string inputFilePath, string outputFilePath)
        {
            int bufferSize = 1024 * 1024;
            using (FileStream fileStream = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                FileStream fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.ReadWrite);
                fileStream.SetLength(fs.Length);
                int bytesRead = -1;
                byte[] bytes = new byte[bufferSize];

                while ((bytesRead = fs.Read(bytes, 0, bufferSize)) > 0)
                {
                    fileStream.Write(bytes, 0, bytesRead);
                }
                fs.Close();
            }
        }

        public byte[] WriteInt16BE(Int16 a)
        {
            byte[] buffer = BitConverter.GetBytes(a);
            Array.Reverse(buffer);
            return buffer;
        }

        public byte[] WriteInt32BE(int a)
        {
            byte[] buffer = BitConverter.GetBytes(a);
            Array.Reverse(buffer);
            return buffer;
        }

        public byte[] WriteInt64BE(long a)
        {
            byte[] buffer = BitConverter.GetBytes(a);
            Array.Reverse(buffer);
            return buffer;
        }

        public void SkipWriter(BinaryWriter wt)
        {
        Writer:;
            if(wt.BaseStream.Position % 16 != 0)
            {
                wt.Write((byte)0x00);
                goto Writer;
            }
        }
    }
}
