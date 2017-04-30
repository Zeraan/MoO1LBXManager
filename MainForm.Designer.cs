namespace LBXManager
{
	partial class MainForm
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
			this.fileList = new System.Windows.Forms.ListBox();
			this.lbxFilePathTextBox = new System.Windows.Forms.TextBox();
			this.browseButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.saveTextButton = new System.Windows.Forms.Button();
			this.saveBMPButton = new System.Windows.Forms.Button();
			this.importBMPButton = new System.Windows.Forms.Button();
			this.dimensionLabel = new System.Windows.Forms.Label();
			this.loadButton = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.numOfFiles = new System.Windows.Forms.Label();
			this.lbxVersionLabel = new System.Windows.Forms.Label();
			this.framesBar = new System.Windows.Forms.TrackBar();
			this.label5 = new System.Windows.Forms.Label();
			this.widthLabel = new System.Windows.Forms.Label();
			this.heightLabel = new System.Windows.Forms.Label();
			this.frameLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.framesBar)).BeginInit();
			this.SuspendLayout();
			// 
			// fileList
			// 
			this.fileList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.fileList.FormattingEnabled = true;
			this.fileList.Location = new System.Drawing.Point(12, 84);
			this.fileList.Name = "fileList";
			this.fileList.Size = new System.Drawing.Size(280, 446);
			this.fileList.TabIndex = 0;
			this.fileList.SelectedIndexChanged += new System.EventHandler(this.fileList_SelectedIndexChanged);
			// 
			// lbxFilePathTextBox
			// 
			this.lbxFilePathTextBox.Location = new System.Drawing.Point(12, 36);
			this.lbxFilePathTextBox.Name = "lbxFilePathTextBox";
			this.lbxFilePathTextBox.Size = new System.Drawing.Size(442, 20);
			this.lbxFilePathTextBox.TabIndex = 1;
			// 
			// browseButton
			// 
			this.browseButton.Location = new System.Drawing.Point(460, 36);
			this.browseButton.Name = "browseButton";
			this.browseButton.Size = new System.Drawing.Size(81, 23);
			this.browseButton.TabIndex = 2;
			this.browseButton.Text = "Browse";
			this.browseButton.UseVisualStyleBackColor = true;
			this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(90, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "LBX File Location";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 63);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(84, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Files in this LBX:";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(308, 84);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(320, 240);
			this.pictureBox1.TabIndex = 5;
			this.pictureBox1.TabStop = false;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(308, 63);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(77, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Image Preview";
			// 
			// textBox2
			// 
			this.textBox2.AcceptsReturn = true;
			this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox2.Location = new System.Drawing.Point(308, 455);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox2.Size = new System.Drawing.Size(320, 75);
			this.textBox2.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(316, 431);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(69, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Text Preview";
			// 
			// saveTextButton
			// 
			this.saveTextButton.Location = new System.Drawing.Point(440, 426);
			this.saveTextButton.Name = "saveTextButton";
			this.saveTextButton.Size = new System.Drawing.Size(188, 23);
			this.saveTextButton.TabIndex = 9;
			this.saveTextButton.Text = "Save Text Changes";
			this.saveTextButton.UseVisualStyleBackColor = true;
			// 
			// saveBMPButton
			// 
			this.saveBMPButton.Location = new System.Drawing.Point(500, 386);
			this.saveBMPButton.Name = "saveBMPButton";
			this.saveBMPButton.Size = new System.Drawing.Size(131, 23);
			this.saveBMPButton.TabIndex = 10;
			this.saveBMPButton.Text = "Export Images";
			this.saveBMPButton.UseVisualStyleBackColor = true;
			this.saveBMPButton.Click += new System.EventHandler(this.saveBMPButton_Click);
			// 
			// importBMPButton
			// 
			this.importBMPButton.Location = new System.Drawing.Point(308, 386);
			this.importBMPButton.Name = "importBMPButton";
			this.importBMPButton.Size = new System.Drawing.Size(160, 23);
			this.importBMPButton.TabIndex = 11;
			this.importBMPButton.Text = "Import Images";
			this.importBMPButton.UseVisualStyleBackColor = true;
			this.importBMPButton.Click += new System.EventHandler(this.importBMPButton_Click);
			// 
			// dimensionLabel
			// 
			this.dimensionLabel.AutoSize = true;
			this.dimensionLabel.Location = new System.Drawing.Point(475, 345);
			this.dimensionLabel.Name = "dimensionLabel";
			this.dimensionLabel.Size = new System.Drawing.Size(0, 13);
			this.dimensionLabel.TabIndex = 13;
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(548, 36);
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new System.Drawing.Size(75, 23);
			this.loadButton.TabIndex = 14;
			this.loadButton.Text = "Load";
			this.loadButton.UseVisualStyleBackColor = true;
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// numOfFiles
			// 
			this.numOfFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numOfFiles.AutoSize = true;
			this.numOfFiles.Location = new System.Drawing.Point(13, 537);
			this.numOfFiles.Name = "numOfFiles";
			this.numOfFiles.Size = new System.Drawing.Size(83, 13);
			this.numOfFiles.TabIndex = 15;
			this.numOfFiles.Text = "Number of Files:";
			// 
			// lbxVersionLabel
			// 
			this.lbxVersionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lbxVersionLabel.AutoSize = true;
			this.lbxVersionLabel.Location = new System.Drawing.Point(308, 537);
			this.lbxVersionLabel.Name = "lbxVersionLabel";
			this.lbxVersionLabel.Size = new System.Drawing.Size(68, 13);
			this.lbxVersionLabel.TabIndex = 16;
			this.lbxVersionLabel.Text = "LBX Version:";
			// 
			// framesBar
			// 
			this.framesBar.Location = new System.Drawing.Point(381, 330);
			this.framesBar.Name = "framesBar";
			this.framesBar.Size = new System.Drawing.Size(160, 45);
			this.framesBar.TabIndex = 12;
			this.framesBar.ValueChanged += new System.EventHandler(this.framesBar_ValueChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(311, 344);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(41, 13);
			this.label5.TabIndex = 17;
			this.label5.Text = "Frames";
			// 
			// widthLabel
			// 
			this.widthLabel.AutoSize = true;
			this.widthLabel.Location = new System.Drawing.Point(548, 331);
			this.widthLabel.Name = "widthLabel";
			this.widthLabel.Size = new System.Drawing.Size(38, 13);
			this.widthLabel.TabIndex = 18;
			this.widthLabel.Text = "Width:";
			// 
			// heightLabel
			// 
			this.heightLabel.AutoSize = true;
			this.heightLabel.Location = new System.Drawing.Point(548, 345);
			this.heightLabel.Name = "heightLabel";
			this.heightLabel.Size = new System.Drawing.Size(41, 13);
			this.heightLabel.TabIndex = 19;
			this.heightLabel.Text = "Height:";
			// 
			// frameLabel
			// 
			this.frameLabel.AutoSize = true;
			this.frameLabel.Location = new System.Drawing.Point(548, 361);
			this.frameLabel.Name = "frameLabel";
			this.frameLabel.Size = new System.Drawing.Size(44, 13);
			this.frameLabel.TabIndex = 20;
			this.frameLabel.Text = "Frames:";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(657, 561);
			this.Controls.Add(this.frameLabel);
			this.Controls.Add(this.heightLabel);
			this.Controls.Add(this.widthLabel);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.lbxVersionLabel);
			this.Controls.Add(this.numOfFiles);
			this.Controls.Add(this.loadButton);
			this.Controls.Add(this.dimensionLabel);
			this.Controls.Add(this.framesBar);
			this.Controls.Add(this.importBMPButton);
			this.Controls.Add(this.saveBMPButton);
			this.Controls.Add(this.saveTextButton);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.browseButton);
			this.Controls.Add(this.lbxFilePathTextBox);
			this.Controls.Add(this.fileList);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.framesBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox fileList;
		private System.Windows.Forms.TextBox lbxFilePathTextBox;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button saveTextButton;
		private System.Windows.Forms.Button saveBMPButton;
		private System.Windows.Forms.Button importBMPButton;
		private System.Windows.Forms.Label dimensionLabel;
		private System.Windows.Forms.Button loadButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Label numOfFiles;
		private System.Windows.Forms.Label lbxVersionLabel;
		private System.Windows.Forms.TrackBar framesBar;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label widthLabel;
		private System.Windows.Forms.Label heightLabel;
		private System.Windows.Forms.Label frameLabel;
	}
}

