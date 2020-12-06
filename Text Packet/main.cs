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
            opf.Filter = "All file(*.TXT;*.BIN)|*.txt;*.bin";
            if(opf.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(opf.FileName))
                {
                    if(Path.GetExtension(opf.FileName) == ".bin")
                    {
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

                                /*
                                BinaryWriter wt = new BinaryWriter(File.Create(pair.Key.ToString("X")+".bin"));
                                wt.Write(out_buffer);
                                wt.Close();*/
                            }
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
