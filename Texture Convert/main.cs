using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Texture_Convert
{
    public partial class main : Form
    {
        public class TextureInfo
        {
            public string name { get; set; }
            public int hash { get; set; }
            public int position { get; set; }
            public UInt16 width { get; set; }
            public UInt16 height { get; set; }
            public int Length { get; set; }
            public TextureInfo(string _n, int hs, int pos, UInt16 wd, UInt16 hg, int len)
            {
                this.name = _n;
                this.hash = hs;
                this.position = pos;
                this.width = wd;
                this.height = hg;
                this.Length = len;
            }
        }

        public List<TextureInfo> textureInfos = new List<TextureInfo>();
        public OpenFileDialog opf = new OpenFileDialog();
        public SaveFileDialog svf = new SaveFileDialog();
        enum Type
        {
            RGBA16p2,
            RGBA16,
            RGBA32,
            RGBA16Type,
            None
        }

        private Type typeTexture;

        public main()
        {
            InitializeComponent();
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            opf.Filter = "HXTB0001 HVT file|*.hvt";
            if(opf.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(opf.FileName))
                {
                    dirpath.Text = opf.FileName;
                    lname.Text = $"Name: {Path.GetFileName(opf.FileName)}";
                    BinaryReaderBE rd = new BinaryReaderBE(new FileStream(opf.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                    if(rd.ReadInt64() == 3544385890569967688)
                    {
                        rd.Skip(4);
                        int count = rd.BEReadInt32();
                        rd.Skip(8);
                        lsize.Text = $"Size: {rd.BEReadInt32().ToString()}";
                        linsize.Text = $"In Size: {rd.BEReadInt32().ToString()}";
                        textureInfos.Clear();
                        for (int i = 0; i< count; i++)
                        {
                            string _name = Encoding.ASCII.GetString(rd.ReadBytes(16)).Replace("\0", string.Empty);
                            int _hash = rd.ReadInt32();
                            int next_pos = rd.BEReadInt32();
                            rd.Skip(8);
                            long _p = rd.BaseStream.Position;

                            rd.BaseStream.Seek(next_pos, SeekOrigin.Begin);
                            int _pos = rd.BEReadInt32();
                            rd.Skip(4);
                            UInt16 _wd = rd.BEReadUInt16();
                            UInt16 _hg = rd.BEReadUInt16();
                            int _len = rd.BEReadInt32();
                            rd.BaseStream.Seek(_p, SeekOrigin.Begin);
                            textureInfos.Add(new TextureInfo(_name, _hash, _pos, _wd, _hg, _len));
                        }
                        listTex.DataSource = textureInfos.Select(t => t.name).ToList();
                        btnExport.Enabled = true;
                        btnImport.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Not support this file!");
                        return;
                    }
                    rd.Close();
                }
            }
        }

        private void listTex_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listTex.SelectedIndex;
            ltname.Text = $"Texture Name: {textureInfos[index].name}";
            ltwidth.Text = $"Width: {textureInfos[index].width.ToString()}";
            ltheight.Text = $"Height: {textureInfos[index].height.ToString()}";
            if(textureInfos[index].Length == textureInfos[index].width * textureInfos[index].height / 2)
            {
                lttype.Text = $"Texture Type: R4G4B4A4";
                typeTexture = Type.RGBA16p2;
            }
            else if(textureInfos[index].Length == textureInfos[index].width * textureInfos[index].height * 2)
            {
                lttype.Text = $"Texture Type: R5G5B5A1";
                typeTexture = Type.RGBA16;
            }
            else if (textureInfos[index].Length == textureInfos[index].width * textureInfos[index].height * 4)
            {
                lttype.Text = $"Texture Type: RGBA32";
                typeTexture = Type.RGBA32;
            }
            else if (textureInfos[index].Length == textureInfos[index].width * textureInfos[index].height)
            {
                lttype.Text = $"Texture Type: A16Type";
                typeTexture = Type.RGBA16Type;
            }
            else
            {
                lttype.Text = $"Texture Type: Unknown";
                typeTexture = Type.None;
            }

            lhash.Text = $"Hash?: 0x{textureInfos[index].hash.ToString("X4")}";
        }

        public void DumpTexture(BinaryWriter wt, int pos, int len, int width, int height)
        {
            var rd = new BinaryReader(new FileStream(dirpath.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            rd.BaseStream.Seek(pos, SeekOrigin.Begin);
            var ms = new BinaryReader(new MemoryStream(rd.ReadBytes(len)));
            int c = 0;
            long k = 0;
            switch (typeTexture)
            {
                case Type.RGBA16p2:
                    wt.Write(RGBA4444);
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j <= width/8; j++)
                        {
                            if (j == width/8)
                            {
                                if (c < 7)
                                {
                                    c++;
                                    ms.BaseStream.Seek(c * 4 + k, SeekOrigin.Begin);
                                }
                                else
                                {
                                    k = ms.BaseStream.Position - 28;
                                    ms.BaseStream.Seek(k, SeekOrigin.Begin);
                                    c = 0;
                                }
                            }
                            else
                            {
                                for(int n = 0; n < 4; n++)
                                {
                                    string byte_type = ms.ReadByte().ToString("X2");
                                    byte bit_a = byte.Parse($"{byte_type[0]}{byte_type[0]}", System.Globalization.NumberStyles.HexNumber);
                                    byte bit_b = byte.Parse($"{byte_type[1]}{byte_type[1]}", System.Globalization.NumberStyles.HexNumber);
                                    wt.Write(new byte[] { bit_a, bit_a, bit_b, bit_b });
                                }
                                Debug.WriteLine(ms.BaseStream.Position);
                                ms.BaseStream.Seek(28, SeekOrigin.Current);
                            }
                        }
                    }
                    wt.BaseStream.Seek(12, SeekOrigin.Begin);
                    wt.Write(height);
                    wt.Write(width);
                    break;
                case Type.RGBA16:
                    wt.Write(RGBA5551);
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j <= width / 4; j++)
                        {
                            if (j == width / 4)
                            {
                                if (c < 3)
                                {
                                    c++;
                                    ms.BaseStream.Seek(c * 8 + k, SeekOrigin.Begin);
                                }
                                else
                                {
                                    k = ms.BaseStream.Position - 24;
                                    ms.BaseStream.Seek(k, SeekOrigin.Begin);
                                    c = 0;
                                }
                            }
                            else
                            {
                                for (int n = 0; n < 4; n++)
                                {
                                    byte[] ln = ms.ReadBytes(2);
                                    Array.Reverse(ln);
                                    wt.Write(ln);
                                }
                                Debug.WriteLine(ms.BaseStream.Position);
                                ms.BaseStream.Seek(24, SeekOrigin.Current);
                            }
                        }
                    }
                    wt.BaseStream.Seek(12, SeekOrigin.Begin);
                    wt.Write(height);
                    wt.Write(width);
                    break;
                case Type.RGBA32:
                    /*for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j <= width/2; j++)
                        {
                            if (j == width/2)
                            {
                                if (c < 3)
                                {
                                    c++;
                                    ms.BaseStream.Seek(c * 8 + k, SeekOrigin.Begin);
                                }
                                else
                                {
                                    k = ms.BaseStream.Position - 24;
                                    ms.BaseStream.Seek(k, SeekOrigin.Begin);
                                    c = 0;
                                }
                            }
                            else
                            {
                                for (int n = 0; n < 2; n++)
                                {
                                    byte[] ln = ms.ReadBytes(4);
                                    Array.Reverse(ln);
                                    wt.Write(ln);
                                }
                                Debug.WriteLine(ms.BaseStream.Position);
                                ms.BaseStream.Seek(24, SeekOrigin.Current);
                            }
                        }
                    }*/
                    MessageBox.Show("Not support type texture!");
                    break;
                case Type.RGBA16Type:
                    wt.Write(RGBA5551);
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j <= width / 4; j++)
                        {
                            if (j == width / 4)
                            {
                                if (c < 7)
                                {
                                    c++;
                                    ms.BaseStream.Seek(c * 4 + k, SeekOrigin.Begin);
                                }
                                else
                                {
                                    k = ms.BaseStream.Position - 28;
                                    ms.BaseStream.Seek(k, SeekOrigin.Begin);
                                    c = 0;
                                }
                            }
                            else
                            {
                                for (int n = 0; n < 4; n++)
                                {
                                    wt.Write((byte)0);
                                    wt.Write(ms.ReadByte());
                                }
                                Debug.WriteLine(ms.BaseStream.Position);
                                ms.BaseStream.Seek(28, SeekOrigin.Current);
                            }
                        }
                    }
                    wt.BaseStream.Seek(12, SeekOrigin.Begin);
                    wt.Write(height);
                    wt.Write(width);
                    break;
                    break;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            svf.Filter = "DirectDraw Surface Image|*.dds";
            svf.FileName = $"{textureInfos[listTex.SelectedIndex].name}";
            if(svf.ShowDialog() == DialogResult.OK)
            {
                using (var wt = new BinaryWriter(File.Create(svf.FileName)))
                {
                    DumpTexture(wt, textureInfos[listTex.SelectedIndex].position, textureInfos[listTex.SelectedIndex].Length, textureInfos[listTex.SelectedIndex].width, textureInfos[listTex.SelectedIndex].height);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {

        }


        public byte[] RGBA4444 = {
                0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x0F, 0x10, 0x02, 0x00, 0x00, 0x01, 0x00, 0x00,
                0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00,
                0xF0, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                };


        public byte[] RGBA5551 = {
                0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x0F, 0x10, 0x02, 0x00, 0x00, 0x01, 0x00, 0x00,
                0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x7C, 0x00, 0x00,
                0xE0, 0x03, 0x00, 0x00, 0x1F, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                };
    }
}
