namespace LBXManager
{
	partial class MainWindow
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lbxDirectoryPathTextBox = new System.Windows.Forms.TextBox();
			this.openDirectoryButton = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lbxFilesTreeView = new System.Windows.Forms.TreeView();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.frameLabel = new System.Windows.Forms.Label();
			this.heightLabel = new System.Windows.Forms.Label();
			this.widthLabel = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.framesBar = new System.Windows.Forms.TrackBar();
			this.exportImagesButton = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.textPreviewTextBox = new System.Windows.Forms.TextBox();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.framesBar)).BeginInit();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.openDirectoryButton);
			this.groupBox1.Controls.Add(this.lbxDirectoryPathTextBox);
			this.groupBox1.Location = new System.Drawing.Point(13, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(814, 75);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Game Directory";
			// 
			// lbxDirectoryPathTextBox
			// 
			this.lbxDirectoryPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lbxDirectoryPathTextBox.Location = new System.Drawing.Point(7, 20);
			this.lbxDirectoryPathTextBox.Name = "lbxDirectoryPathTextBox";
			this.lbxDirectoryPathTextBox.ReadOnly = true;
			this.lbxDirectoryPathTextBox.Size = new System.Drawing.Size(801, 20);
			this.lbxDirectoryPathTextBox.TabIndex = 0;
			// 
			// openDirectoryButton
			// 
			this.openDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.openDirectoryButton.Location = new System.Drawing.Point(637, 46);
			this.openDirectoryButton.Name = "openDirectoryButton";
			this.openDirectoryButton.Size = new System.Drawing.Size(171, 23);
			this.openDirectoryButton.TabIndex = 1;
			this.openDirectoryButton.Text = "Open Game Directory";
			this.openDirectoryButton.UseVisualStyleBackColor = true;
			this.openDirectoryButton.Click += new System.EventHandler(this.openDirectoryButton_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox2.Controls.Add(this.lbxFilesTreeView);
			this.groupBox2.Location = new System.Drawing.Point(13, 95);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(343, 510);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "LBX Files";
			// 
			// lbxFilesTreeView
			// 
			this.lbxFilesTreeView.Enabled = false;
			this.lbxFilesTreeView.Location = new System.Drawing.Point(7, 20);
			this.lbxFilesTreeView.Name = "lbxFilesTreeView";
			this.lbxFilesTreeView.Size = new System.Drawing.Size(330, 484);
			this.lbxFilesTreeView.TabIndex = 0;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.exportImagesButton);
			this.groupBox3.Controls.Add(this.framesBar);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.frameLabel);
			this.groupBox3.Controls.Add(this.heightLabel);
			this.groupBox3.Controls.Add(this.widthLabel);
			this.groupBox3.Controls.Add(this.pictureBox1);
			this.groupBox3.Location = new System.Drawing.Point(363, 95);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(464, 326);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Image Preview";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(7, 20);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(320, 240);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// frameLabel
			// 
			this.frameLabel.AutoSize = true;
			this.frameLabel.Location = new System.Drawing.Point(333, 50);
			this.frameLabel.Name = "frameLabel";
			this.frameLabel.Size = new System.Drawing.Size(44, 13);
			this.frameLabel.TabIndex = 23;
			this.frameLabel.Text = "Frames:";
			// 
			// heightLabel
			// 
			this.heightLabel.AutoSize = true;
			this.heightLabel.Location = new System.Drawing.Point(333, 34);
			this.heightLabel.Name = "heightLabel";
			this.heightLabel.Size = new System.Drawing.Size(41, 13);
			this.heightLabel.TabIndex = 22;
			this.heightLabel.Text = "Height:";
			// 
			// widthLabel
			// 
			this.widthLabel.AutoSize = true;
			this.widthLabel.Location = new System.Drawing.Point(333, 20);
			this.widthLabel.Name = "widthLabel";
			this.widthLabel.Size = new System.Drawing.Size(38, 13);
			this.widthLabel.TabIndex = 21;
			this.widthLabel.Text = "Width:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 284);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(65, 13);
			this.label5.TabIndex = 24;
			this.label5.Text = "Frame Index";
			// 
			// framesBar
			// 
			this.framesBar.Enabled = false;
			this.framesBar.Location = new System.Drawing.Point(77, 284);
			this.framesBar.Name = "framesBar";
			this.framesBar.Size = new System.Drawing.Size(250, 45);
			this.framesBar.TabIndex = 25;
			// 
			// exportImagesButton
			// 
			this.exportImagesButton.Enabled = false;
			this.exportImagesButton.Location = new System.Drawing.Point(327, 284);
			this.exportImagesButton.Name = "exportImagesButton";
			this.exportImagesButton.Size = new System.Drawing.Size(131, 23);
			this.exportImagesButton.TabIndex = 11;
			this.exportImagesButton.Text = "Export Images";
			this.exportImagesButton.UseVisualStyleBackColor = true;
			this.exportImagesButton.Click += new System.EventHandler(this.exportImagesButton_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.textPreviewTextBox);
			this.groupBox4.Location = new System.Drawing.Point(363, 428);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(464, 177);
			this.groupBox4.TabIndex = 3;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Text Preview";
			// 
			// textPreviewTextBox
			// 
			this.textPreviewTextBox.Location = new System.Drawing.Point(7, 20);
			this.textPreviewTextBox.Multiline = true;
			this.textPreviewTextBox.Name = "textPreviewTextBox";
			this.textPreviewTextBox.ReadOnly = true;
			this.textPreviewTextBox.Size = new System.Drawing.Size(451, 151);
			this.textPreviewTextBox.TabIndex = 0;
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(839, 617);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "MainWindow";
			this.Text = "MainWindow";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.framesBar)).EndInit();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button openDirectoryButton;
		private System.Windows.Forms.TextBox lbxDirectoryPathTextBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TreeView lbxFilesTreeView;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label frameLabel;
		private System.Windows.Forms.Label heightLabel;
		private System.Windows.Forms.Label widthLabel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TrackBar framesBar;
		private System.Windows.Forms.Button exportImagesButton;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TextBox textPreviewTextBox;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
	}
}