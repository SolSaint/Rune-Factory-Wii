namespace Texture_Convert
{
    partial class main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dirpath = new System.Windows.Forms.TextBox();
            this.btnBrowser = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lhash = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.lttype = new System.Windows.Forms.Label();
            this.ltheight = new System.Windows.Forms.Label();
            this.ltwidth = new System.Windows.Forms.Label();
            this.ltname = new System.Windows.Forms.Label();
            this.linsize = new System.Windows.Forms.Label();
            this.lsize = new System.Windows.Forms.Label();
            this.lname = new System.Windows.Forms.Label();
            this.listTex = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dirpath
            // 
            this.dirpath.Location = new System.Drawing.Point(12, 12);
            this.dirpath.Name = "dirpath";
            this.dirpath.ReadOnly = true;
            this.dirpath.Size = new System.Drawing.Size(215, 20);
            this.dirpath.TabIndex = 0;
            // 
            // btnBrowser
            // 
            this.btnBrowser.Location = new System.Drawing.Point(233, 12);
            this.btnBrowser.Name = "btnBrowser";
            this.btnBrowser.Size = new System.Drawing.Size(32, 20);
            this.btnBrowser.TabIndex = 1;
            this.btnBrowser.Text = "...";
            this.btnBrowser.UseVisualStyleBackColor = true;
            this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lhash);
            this.groupBox1.Controls.Add(this.btnExport);
            this.groupBox1.Controls.Add(this.btnImport);
            this.groupBox1.Controls.Add(this.lttype);
            this.groupBox1.Controls.Add(this.ltheight);
            this.groupBox1.Controls.Add(this.ltwidth);
            this.groupBox1.Controls.Add(this.ltname);
            this.groupBox1.Controls.Add(this.linsize);
            this.groupBox1.Controls.Add(this.lsize);
            this.groupBox1.Controls.Add(this.lname);
            this.groupBox1.Controls.Add(this.listTex);
            this.groupBox1.Location = new System.Drawing.Point(12, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(253, 495);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Properties";
            // 
            // lhash
            // 
            this.lhash.AutoSize = true;
            this.lhash.Location = new System.Drawing.Point(16, 431);
            this.lhash.Name = "lhash";
            this.lhash.Size = new System.Drawing.Size(41, 13);
            this.lhash.TabIndex = 10;
            this.lhash.Text = "Hash?:";
            // 
            // btnExport
            // 
            this.btnExport.Enabled = false;
            this.btnExport.Location = new System.Drawing.Point(137, 457);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(110, 23);
            this.btnExport.TabIndex = 9;
            this.btnExport.Text = "Export Texture";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImport
            // 
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(6, 457);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(110, 23);
            this.btnImport.TabIndex = 8;
            this.btnImport.Text = "Import Texture";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // lttype
            // 
            this.lttype.AutoSize = true;
            this.lttype.Location = new System.Drawing.Point(16, 418);
            this.lttype.Name = "lttype";
            this.lttype.Size = new System.Drawing.Size(73, 13);
            this.lttype.TabIndex = 7;
            this.lttype.Text = "Texture Type:";
            // 
            // ltheight
            // 
            this.ltheight.AutoSize = true;
            this.ltheight.Location = new System.Drawing.Point(16, 405);
            this.ltheight.Name = "ltheight";
            this.ltheight.Size = new System.Drawing.Size(41, 13);
            this.ltheight.TabIndex = 6;
            this.ltheight.Text = "Height:";
            // 
            // ltwidth
            // 
            this.ltwidth.AutoSize = true;
            this.ltwidth.Location = new System.Drawing.Point(16, 392);
            this.ltwidth.Name = "ltwidth";
            this.ltwidth.Size = new System.Drawing.Size(38, 13);
            this.ltwidth.TabIndex = 5;
            this.ltwidth.Text = "Width:";
            // 
            // ltname
            // 
            this.ltname.AutoSize = true;
            this.ltname.Location = new System.Drawing.Point(16, 379);
            this.ltname.Name = "ltname";
            this.ltname.Size = new System.Drawing.Size(77, 13);
            this.ltname.TabIndex = 4;
            this.ltname.Text = "Texture Name:";
            // 
            // linsize
            // 
            this.linsize.AutoSize = true;
            this.linsize.Location = new System.Drawing.Point(6, 45);
            this.linsize.Name = "linsize";
            this.linsize.Size = new System.Drawing.Size(42, 13);
            this.linsize.TabIndex = 3;
            this.linsize.Text = "In Size:";
            // 
            // lsize
            // 
            this.lsize.AutoSize = true;
            this.lsize.Location = new System.Drawing.Point(6, 32);
            this.lsize.Name = "lsize";
            this.lsize.Size = new System.Drawing.Size(30, 13);
            this.lsize.TabIndex = 2;
            this.lsize.Text = "Size:";
            // 
            // lname
            // 
            this.lname.AutoSize = true;
            this.lname.Location = new System.Drawing.Point(6, 19);
            this.lname.Name = "lname";
            this.lname.Size = new System.Drawing.Size(38, 13);
            this.lname.TabIndex = 1;
            this.lname.Text = "Name:";
            // 
            // listTex
            // 
            this.listTex.FormattingEnabled = true;
            this.listTex.Location = new System.Drawing.Point(6, 74);
            this.listTex.Name = "listTex";
            this.listTex.Size = new System.Drawing.Size(241, 290);
            this.listTex.TabIndex = 0;
            this.listTex.SelectedIndexChanged += new System.EventHandler(this.listTex_SelectedIndexChanged);
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 538);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnBrowser);
            this.Controls.Add(this.dirpath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Texture Tool";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox dirpath;
        private System.Windows.Forms.Button btnBrowser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listTex;
        private System.Windows.Forms.Label lttype;
        private System.Windows.Forms.Label ltheight;
        private System.Windows.Forms.Label ltwidth;
        private System.Windows.Forms.Label ltname;
        private System.Windows.Forms.Label linsize;
        private System.Windows.Forms.Label lsize;
        private System.Windows.Forms.Label lname;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Label lhash;
    }
}

