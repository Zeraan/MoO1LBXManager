using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LBXManager
{
	public partial class MainWindow : Form
	{
		private Bitmap[] currentBitmaps;

		public MainWindow()
		{
			InitializeComponent();
		}

		#region Member Variables
		private List<LBXFile> lbxFiles;
		#endregion

		#region Functions
		/// <summary>
		/// Scans the selected directory and load in any LBX files found
		/// </summary>
		private void LoadLBXFiles()
		{
			lbxFiles = new List<LBXFile>();
			DirectoryInfo di = new DirectoryInfo(lbxDirectoryPathTextBox.Text);
			var files = di.GetFiles("*.lbx");
			string reason;
			foreach (var file in files)
			{
				if (file.Name.ToLower().StartsWith("fonts"))
				{
					if (!LBXFile.LoadExternalPalette(file, out reason))
					{
						MessageBox.Show(this, reason, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				var lbxFile = LoadLBXFile(file, out reason);
				if (lbxFile != null)
				{
					var treeNode = new TreeNode(file.Name);
					treeNode.Tag = lbxFile;
					if (lbxFile.LBXFileType == LBXFileTypeEnum.IMAGE)
					{
						foreach (var internalFile in lbxFile.InternalFiles)
						{
							var internalNode = new TreeNode(internalFile.FileName + " " + internalFile.Comment);
							internalNode.Tag = internalFile;
							treeNode.Nodes.Add(internalNode);
						}
					}
					lbxFilesTreeView.Nodes.Add(treeNode);
					lbxFiles.Add(lbxFile);
				}
				else
				{
					MessageBox.Show(this, reason, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			lbxFilesTreeView.Enabled = true;
		}

		/// <summary>
		/// Loads from a LBX file into FileClass
		/// </summary>
		/// <param name="fileToLoad">The file to load</param>
		/// <returns>A new FileClass containing the LBX data</returns>
		private LBXFile LoadLBXFile(FileInfo fileToLoad, out string reason)
		{
			var lbxFile = new LBXFile();
			if (lbxFile.LoadLBXFile(fileToLoad, out reason))
			{
				return lbxFile;
			}
			return null;
		}

		private void LoadTextFromLBXFile(LBXFile lbxFile)
		{
			framesBar.Enabled = false;
			framesBar.SetRange(0, 1);
			framesBar.Value = 0;

			pictureBox1.Image = null;
			pictureBox1.Refresh();

			currentBitmaps = null;

			exportImagesButton.Enabled = false;

			textPreviewTextBox.Text = lbxFile.GetText();
		}

		private void LoadImageFromLBXFile(LBXFile lbxFile, InternalFileClass internalFile)
		{
			currentBitmaps = internalFile.GetBitmaps(lbxFile);

			if (currentBitmaps.Length > 1)
			{
				framesBar.Enabled = true;
				framesBar.SetRange(0, currentBitmaps.Length - 1);
			}
			else
			{
				framesBar.Enabled = false;
				framesBar.SetRange(0, 1);
			}
			framesBar.Value = 0;

			widthLabel.Text = "Width: " + internalFile.Width;
			heightLabel.Text = "Height: " + internalFile.Height;
			frameLabel.Text = "Frames: " + internalFile.Frames;

			pictureBox1.Image = currentBitmaps[0];
			pictureBox1.Refresh();

			exportImagesButton.Enabled = true;

			textPreviewTextBox.Text = string.Empty;
		}
		#endregion

		#region Events
		private void openDirectoryButton_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.ShowNewFolderButton = false;
			folderBrowserDialog.SelectedPath = Environment.CurrentDirectory;
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				lbxDirectoryPathTextBox.Text = folderBrowserDialog.SelectedPath;
				LoadLBXFiles();
			}
		}

		private void exportImagesButton_Click(object sender, EventArgs e)
		{
			if (currentBitmaps == null) //Sanity check
			{
				return;
			}
			using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
			{
				if (folderBrowser.ShowDialog() == DialogResult.OK)
				{
					try
					{
						var selectedNode = lbxFilesTreeView.SelectedNode;
						InternalFileClass internalFile = selectedNode.Tag as InternalFileClass;

						for (int i = 0; i < currentBitmaps.Length; i++)
						{
							currentBitmaps[i].Save(Path.Combine(folderBrowser.SelectedPath, internalFile.FileName + i + ".BMP"));
						}
					}
					catch (Exception exception)
					{
						MessageBox.Show(exception.Message);
					}
				}
			}
		}

		/// <summary>
		/// Load the image or text or clear out everything if unknown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbxFilesTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			var selectedNode = e.Node;
			//Determine if this was an internal file or LBX file

			if (selectedNode.Nodes.Count > 0) //Parent LBX file, don't do anything
			{
				return;
			}
			InternalFileClass internalFile = selectedNode.Tag as InternalFileClass;
			if (internalFile != null)
			{
				//An image file
				LBXFile lbxFile = selectedNode.Parent.Tag as LBXFile;
				if (lbxFile != null) //Sanity check
				{
					LoadImageFromLBXFile(lbxFile, internalFile);
				}
				else
				{
					MessageBox.Show(this, "I dunno what happened, but apparently parent node doesn't have LBX File loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
			else
			{
				//Probably a text file
				LBXFile lbxFile = selectedNode.Tag as LBXFile;
				LoadTextFromLBXFile(lbxFile);
			}
		}

		private void framesBar_ValueChanged(object sender, EventArgs e)
		{
			pictureBox1.Image = currentBitmaps[framesBar.Value];
			pictureBox1.Refresh();
		}
		#endregion
	}
}
