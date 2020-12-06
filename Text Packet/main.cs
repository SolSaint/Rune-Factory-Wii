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
                                goto script;
                            case 0xFE:
                                goto text_dat;
                            default: Console.WriteLine("File not support!");
                                break;
                        }
                        //text_dat.bin
                    text_dat:
                        {
                            StreamWriter wtFs = new StreamWriter(dir + "\\" + filename + ".FileSizeTable");
                            if (!Directory.Exists(dir + "\\" + filename))
                                Directory.CreateDirectory(dir + "\\" + filename);
                            rd.BaseStream.Seek(16, SeekOrigin.Begin);
                            rd.ReadInt16();
                            int count = rd.ReadInt16();
                            int skip_byte = rd.ReadInt32();
                            Dictionary<int, int> dic = new Dictionary<int, int>();
                            for(int i = 0; i < count; i++)
                            {
                                dic.Add(rd.ReadInt32(), rd.ReadInt32());
                            }
                            foreach (KeyValuePair<int,int> pair in dic)
                            {
                                byte[] out_buffer = rd.ReadBytes(pair.Value);
                                BinaryReaderBE rdms = new BinaryReaderBE(new MemoryStream(out_buffer));
                                StreamWriter wt = new StreamWriter(dir + "\\" + filename + "\\0x" + pair.Key.ToString("X") + ".txt");
                                List<int> offset_line = new List<int>();
                                rdms.BaseStream.Seek(rdms.ReadInt64() - 16, SeekOrigin.Begin);
                                int line = rdms.ReadInt32();
                                rdms.ReadBytes(12);
                                for(int i = 0; i < line + 2; i++)
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
                                for(int i = 0; i < line; i++)
                                {
                                    rdms.BaseStream.Seek(offset_line[i], SeekOrigin.Begin);
                                    byte[] txt = rdms.ReadBytes(offset_line[i + 1] - offset_line[i] - 2);
                                    StringBuilder sb = new StringBuilder(Encoding.BigEndianUnicode.GetString(txt));
                                    sb.Replace("\r", "[r]");
                                    sb.Replace("\n", "[n]");
                                    if (!string.IsNullOrEmpty(sb.ToString()))
                                        wt.WriteLine(sb.ToString());
                                    else
                                        wt.WriteLine("[0]");
                                }
                                wt.Close();
                                BinaryWriter wt_binary = new BinaryWriter(File.Create(dir + "\\" + filename +"\\0x" + pair.Key.ToString("X")+".bin"));
                                wt_binary.Write(out_buffer);
                                wt_binary.Close();
                                rdms.Close();
                                wtFs.WriteLine(dir + "\\" + filename + "\\0x" + pair.Key.ToString("X") + ".bin");
                            }
                            wtFs.Close();
                        }
                        //script.bin
                    script:
                        {
                            rd.BaseStream.Seek(0, SeekOrigin.Begin);

                        }
                    }
                    else
                    {
                        //MessageBox.Show("text file");
                    }
                }
            }
        }
    }
}
