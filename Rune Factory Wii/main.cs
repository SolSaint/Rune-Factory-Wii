using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rune_Factory_Wii
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }

        OpenFileDialog opf = new OpenFileDialog();

        private void btnUnpack_Click(object sender, EventArgs e)
        {
            opf.Filter = "RUNEFACTORY RFF2|*.bin";
            if(opf.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(opf.FileName))
                {
                    using(var rd = new BinaryReader(File.OpenRead(opf.FileName)))
                    {
                        StreamWriter fileDir = new StreamWriter(Path.GetDirectoryName(opf.FileName) + "\\" + Path.GetFileNameWithoutExtension(opf.FileName) + ".FileDir");
                        rd.BaseStream.Seek(12, SeekOrigin.Begin);
                        int count = BEReadInt32(rd.ReadBytes(4));
                        rd.BaseStream.Seek(36, SeekOrigin.Current);
                        for(int i = 0; i< count; i++)
                        {
                            long size = BEReadInt64(rd.ReadBytes(8));
                            long offset = BEReadInt64(rd.ReadBytes(8));
                            LogFile(new BinaryReader(File.OpenRead(Path.GetDirectoryName(opf.FileName) + "\\" + Path.GetFileNameWithoutExtension(opf.FileName) + ".dat")), fileDir, Path.GetDirectoryName(opf.FileName) + "\\" + Path.GetFileNameWithoutExtension(opf.FileName) + "_unk\\", offset, size, i, ctDir.Checked);
                        }
                        fileDir.Close();
                    }
                }
            }
        }

        private void btnCooked_Click(object sender, EventArgs e)
        {
            opf.Filter = "File Dir Extract|*.FileDir";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(opf.FileName))
                {

                }
            }
        }

        public static Int32 BEReadInt32(byte[] buffer)
        {
            Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static Int64 BEReadInt64(byte[] buffer)
        {
            Array.Reverse(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        public void LogFile(BinaryReader reader, StreamWriter fileDir, string outdir, long offset, long size, int idS, bool flag)
        {
            if (!flag)
            {
                if(!Directory.Exists(outdir))
                    Directory.CreateDirectory(outdir);
                string outFile = outdir + "\\" + idS.ToString() + ".dat";
                BinaryWriter wt = new BinaryWriter(File.Create(outFile));
                fileDir.WriteLine(outFile);
                reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                byte[] buffer = reader.ReadBytes((int)size);
                wt.Write(buffer);
                wt.Flush();
                wt.Close();
            }
            else
            {
                if (File.Exists("dir.txt"))
                {
                    try
                    {
                        string[] dirdata = File.ReadAllLines("dir.txt");
                        var split = dirdata[idS].Split(':');
                        if (!Directory.Exists(outdir))
                            Directory.CreateDirectory(outdir);
                        string outFile = outdir + "\\" + split[1];
                        if (!Directory.Exists(Path.GetDirectoryName(outFile)))
                            Directory.CreateDirectory(Path.GetDirectoryName(outFile));
                        BinaryWriter wt = new BinaryWriter(File.Create(outFile));
                        fileDir.WriteLine(outFile);
                        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                        byte[] buffer = reader.ReadBytes((int)size);
                        wt.Write(buffer);
                        wt.Flush();
                        wt.Close();
                    }
                    catch
                    {
                        if (!Directory.Exists(outdir))
                            Directory.CreateDirectory(outdir);
                        string outFile = outdir + "\\" + idS.ToString() + ".dat";
                        BinaryWriter wt = new BinaryWriter(File.Create(outFile));
                        fileDir.WriteLine(outFile);
                        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                        byte[] buffer = reader.ReadBytes((int)size);
                        wt.Write(buffer);
                        wt.Flush();
                        wt.Close();
                    }
                }
                else
                {
                    if (!Directory.Exists(outdir))
                        Directory.CreateDirectory(outdir);
                    string outFile = outdir + "\\" + idS.ToString() + ".dat";
                    BinaryWriter wt = new BinaryWriter(File.Create(outFile));
                    fileDir.WriteLine(outFile);
                    reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                    byte[] buffer = reader.ReadBytes((int)size);
                    wt.Write(buffer);
                    wt.Flush();
                    wt.Close();
                }
            }
        }
    }
}
