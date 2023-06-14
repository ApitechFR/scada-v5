namespace Scada.Scheme.Model.PropertyGrid
{
    partial class FrmIDcustomDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmIDcustomDialog));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelProjectPath = new System.Windows.Forms.Label();
            this.labelPath = new System.Windows.Forms.Label();
            this.buttonPorjectPath = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(12, 59);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(239, 414);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(924, 56);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(100, 29);
            this.buttonSearch.TabIndex = 2;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(257, 59);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(661, 22);
            this.textBox1.TabIndex = 3;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(257, 94);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(767, 379);
            this.dataGridView1.TabIndex = 4;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(924, 479);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 33);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(818, 479);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 33);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // labelProjectPath
            // 
            this.labelProjectPath.AutoSize = true;
            this.labelProjectPath.Location = new System.Drawing.Point(51, 26);
            this.labelProjectPath.Name = "labelProjectPath";
            this.labelProjectPath.Size = new System.Drawing.Size(88, 16);
            this.labelProjectPath.TabIndex = 7;
            this.labelProjectPath.Text = "Project Path : ";
            // 
            // labelPath
            // 
            this.labelPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPath.AutoSize = true;
            this.labelPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelPath.Location = new System.Drawing.Point(145, 24);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(175, 18);
            this.labelPath.TabIndex = 8;
            this.labelPath.Text = "Please, select project folder";
            this.labelPath.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonPorjectPath
            // 
            this.buttonPorjectPath.BackColor = System.Drawing.Color.White;
            this.buttonPorjectPath.Image = ((System.Drawing.Image)(resources.GetObject("buttonPorjectPath.Image")));
            this.buttonPorjectPath.Location = new System.Drawing.Point(12, 21);
            this.buttonPorjectPath.Name = "buttonPorjectPath";
            this.buttonPorjectPath.Size = new System.Drawing.Size(33, 27);
            this.buttonPorjectPath.TabIndex = 9;
            this.buttonPorjectPath.UseVisualStyleBackColor = false;
            this.buttonPorjectPath.Click += new System.EventHandler(this.buttonPorjectPath_Click);
            // 
            // FrmIDcustomDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1036, 523);
            this.Controls.Add(this.buttonPorjectPath);
            this.Controls.Add(this.labelPath);
            this.Controls.Add(this.labelProjectPath);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.treeView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FrmIDcustomDialog";
            this.Text = "ID";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        public System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label labelProjectPath;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.Button buttonPorjectPath;
    }
}