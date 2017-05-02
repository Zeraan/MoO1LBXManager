using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace LBXManager
{
	public partial class MainWindow : Form
	{
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
							var internalNode = new TreeNode(internalFile.fileName + " " + internalFile.comment);
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

		}

		private void LoadImageFromLBXFile(LBXFile lbxFile, InternalFileClass internalFile)
		{

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
		#endregion
	}
}
