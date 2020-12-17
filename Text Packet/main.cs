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
