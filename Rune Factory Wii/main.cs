using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    }
}
