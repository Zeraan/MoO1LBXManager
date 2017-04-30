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
		private List<InternalFileClass> lbxFiles;
		#endregion

		#region Functions
		/// <summary>
		/// Scans the selected directory and load in any LBX files found
		/// </summary>
		private void LoadLBXFiles()
		{
			lbxFiles = new List<InternalFileClass>();
			DirectoryInfo di = new DirectoryInfo(lbxDirectoryPathTextBox.Text);
			var files = di.GetFiles("*.lbx");
			foreach (var file in files)
			{
				lbxFilesTreeView.Nodes.Add(new TreeNode(file.Name));
			}
			lbxFilesTreeView.Enabled = true;
		}

		/// <summary>
		/// Loads from a LBX file into FileClass
		/// </summary>
		/// <param name="fileToLoad">The file to load</param>
		/// <returns>A new FileClass containing the LBX data</returns>
		private InternalFileClass LoadLBXFile(FileInfo fileToLoad)
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
		#endregion
	}
}
