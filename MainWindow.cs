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

		#region Experimental Code - Unused
		//TODO - Figure out how to import from bitmaps into LBX format, allowing for modding of graphics for Master of Orion 1
		/*private void saveBMPButton_Click(object sender, EventArgs e)
		{
			if (fileType != 0 || files.Length == 0)
			{
				return;
			}
			using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
			{
				if (folderBrowser.ShowDialog() == DialogResult.OK)
				{
					try
					{
						using (TextWriter baseWriter = new StreamWriter(Path.Combine(folderBrowser.SelectedPath, "LBXHEADER.TXT"), false))
						{
							FileInfo fi = new FileInfo(lbxFilePathTextBox.Text);
							baseWriter.WriteLine("LBXName:" + fi.Name);
							baseWriter.WriteLine("NumOfFiles:" + files.Length);
							foreach (InternalFileClass file in files)
							{
								baseWriter.WriteLine("FileName:" + file.FileName + " Comment:" + file.Comment);
								TryLoadImage(file);
								using (
									TextWriter writer = new StreamWriter(Path.Combine(folderBrowser.SelectedPath, file.FileName + "HDR.TXT"), false)
									)
								{
									writer.WriteLine("Frames:" + bitmaps.Length);
									writer.WriteLine("FrameDelay:" + frameDelay);
									if (numOfInternalColors > 0)
									{
										writer.WriteLine("HasInternalPalette:True");
										writer.WriteLine("NumberOfColors:" + numOfInternalColors);
										writer.WriteLine("PaletteOffset:" + internalPaletteOffset);
										for (int i = 0; i < numOfInternalColors; i++)
										{
											writer.WriteLine("R:" + internalPalette[i * 3] * 4 + " G:" + internalPalette[i * 3 + 1] * 4 + " B:" +
															 internalPalette[i * 3 + 2] * 4);
										}
									}
									else
									{
										writer.WriteLine("HasInternalPalette:False");
									}
									writer.WriteLine();
								}
								for (int i = 0; i < bitmaps.Length; i++)
								{
									bitmaps[i].Save(Path.Combine(folderBrowser.SelectedPath, file.FileName + i + ".BMP"));
								}
							}
						}
					}
					catch (Exception exception)
					{
						MessageBox.Show(exception.Message);
					}
				}
			}
		}

		private void importBMPButton_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.Description = "Folder to import from";
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				MessageBox.Show("This may take a while, please don't kill the application");
				string reason;
				if (!TryCreateLBX(folderBrowserDialog.SelectedPath, out reason))
				{
					MessageBox.Show("Failed to create LBX file: " + reason);
				}
				else
				{
					MessageBox.Show("Done!");
				}
			}
		}

		private bool TryCreateLBX(string importPath, out string reason)
		{
			DirectoryInfo import = new DirectoryInfo(importPath);
			if (!import.Exists)
			{
				reason = "Import directory don't exist";
				return false;
			}
			try
			{
				using (TextReader reader = new StreamReader(Path.Combine(import.FullName, "LBXHEADER.TXT")))
				{
					char[] splitter = new[] { ':' };
					string line = reader.ReadLine();
					string LBXFileName = line.Split(splitter)[1];
					line = reader.ReadLine();
					int numOfFilesToLoad = int.Parse(line.Split(splitter)[1]);
					string[] fileNames = new string[numOfFilesToLoad];
					string[] fileComments = new string[numOfFilesToLoad];
					for (int i = 0; i < numOfFilesToLoad; i++)
					{
						string[] components = reader.ReadLine().Split(new[] { ':', ' ' });
						fileNames[i] = string.Format("{0,-8}", components[1]);
						fileComments[i] = string.Format("{0,-24}", components[3]);
					}
					//Load in file data
					byte[][] filesData = new byte[numOfFilesToLoad][];
					for (int i = 0; i < numOfFilesToLoad; i++)
					{
						filesData[i] = LoadInFile(fileNames[i], importPath);
					}
					uint length = 512U + (32U * (uint)numOfFilesToLoad);
					uint[] offsets = new uint[numOfFilesToLoad + 1];
					for (int i = 0; i < numOfFilesToLoad; i++)
					{
						offsets[i] = length;
						length += (uint)filesData[i].Length;
					}
					offsets[offsets.Length - 1] = length;

					byte[] finalLBXStream = new byte[length];
					finalLBXStream[0] = (byte)(numOfFilesToLoad % 256);
					finalLBXStream[1] = (byte)(numOfFilesToLoad / 256);
					finalLBXStream[2] = 0xAD;
					finalLBXStream[3] = 0xFE;
					for (int i = 0; i < offsets.Length; i++)
					{
						finalLBXStream[8 + (i * 4)] = (byte)(offsets[i] % 256);
						finalLBXStream[9 + (i * 4)] = (byte)((offsets[i] / 256) % 256);
						finalLBXStream[10 + (i * 4)] = (byte)((offsets[i] / 65536) % 256);
						finalLBXStream[11 + (i * 4)] = (byte)(offsets[i] / 16777216);
					}
					for (int i = 0; i < numOfFilesToLoad; i++)
					{
						for (int k = 0; k < 8; k++)
						{
							finalLBXStream[512 + (i * 32) + k] = (byte)fileNames[i][k];
						}
						for (int k = 0; k < 24; k++)
						{
							finalLBXStream[520 + (i * 32) + k] = (byte)fileComments[i][k];
						}
						for (uint j = offsets[i]; j < offsets[i + 1]; j++)
						{
							finalLBXStream[j] = filesData[i][j - offsets[i]];
						}
					}
					string outputPath = Path.Combine(importPath, LBXFileName);
					if (File.Exists(outputPath))
					{
						File.Delete(outputPath);
					}
					using (Stream writer = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
					{
						writer.Write(finalLBXStream, 0, finalLBXStream.Length);
					}
					reason = null;
					return true;
				}
			}
			catch (Exception e)
			{
				reason = e.Message;
				return false;
			}
		}

		private byte[] LoadInFile(string fileName, string importPath)
		{
			using (TextReader reader = new StreamReader(Path.Combine(importPath, fileName + "HDR.TXT")))
			{
				int numOfFrames;
				bool hasColorPalette;
				int numOfColors = 0;
				int paletteOffset = 0;
				byte[] colorPalette = null;

				numOfFrames = int.Parse(reader.ReadLine().Split(new[] { ':' })[1]);
				frameDelay = int.Parse(reader.ReadLine().Split(new[] { ':' })[1]);
				hasColorPalette = string.Compare(reader.ReadLine().Split(new[] { ':' })[1], "True") == 0;
				if (hasColorPalette)
				{
					char[] splitter = new[] { ':' };
					numOfColors = int.Parse(reader.ReadLine().Split(new[] { ':' })[1]);
					paletteOffset = int.Parse(reader.ReadLine().Split(new[] { ':' })[1]);

					colorPalette = new byte[numOfColors * 3];
					for (int i = 0; i < numOfColors; i++)
					{
						string[] components = reader.ReadLine().Split(new[] { ' ' });
						colorPalette[i * 3] = (byte)(byte.Parse(components[0].Split(splitter)[1]) / 3);
						colorPalette[i * 3 + 1] = (byte)(byte.Parse(components[1].Split(splitter)[1]) / 3);
						colorPalette[i * 3 + 2] = (byte)(byte.Parse(components[2].Split(splitter)[1]) / 3);
					}
				}

				int width = 0;
				int height = 0;
				//Holds the entire frame, will process frames later
				byte[][] fullFrames = new byte[numOfFrames][];
				for (int i = 0; i < numOfFrames; i++)
				{
					Bitmap bitmap = new Bitmap(Path.Combine(importPath, fileName + i + ".BMP"));
					BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
															PixelFormat.Format8bppIndexed);
					IntPtr basePtr = bitmapData.Scan0;
					int baseBytes = Math.Abs(bitmapData.Stride) * bitmapData.Height;
					fullFrames[i] = new byte[baseBytes];
					System.Runtime.InteropServices.Marshal.Copy(basePtr, fullFrames[i], 0, baseBytes);
					bitmap.UnlockBits(bitmapData);

					width = bitmap.Width;
					height = bitmap.Height;
				}

				//Now process each frame, stripping out same pixels between each frame
				for (int i = numOfFrames - 1; i > 0; i--)
				{
					ProcessFrame(fullFrames[i], fullFrames[i - 1]);
				}

				byte[][] compressedFrames = new byte[numOfFrames][];
				for (int i = 0; i < numOfFrames; i++)
				{
					compressedFrames[i] = CompressFrame(fullFrames[i], width, height);
				}

				uint actualLength = 18U + (uint)numOfFrames * 4U;
				if (hasColorPalette)
				{
					actualLength += 16U + (uint)(numOfColors * 3);
				}

				actualLength++; //Add 1 for start of frames (01)
				for (int i = 0; i < numOfFrames; i++)
				{
					actualLength += (uint)compressedFrames[i].Length;
				}

				byte[] finalFile = new byte[actualLength];

				//Load the file into one big array

				//Set up the header of the file
				finalFile[0] = (byte)(width % 256);
				finalFile[1] = (byte)(width / 256);
				finalFile[2] = (byte)(height % 256);
				finalFile[3] = (byte)(height / 256);
				finalFile[6] = (byte)(numOfFrames);
				finalFile[8] = (byte)(frameDelay % 256);
				finalFile[9] = (byte)(frameDelay / 256);

				int frameStartOffset = numOfFrames * 4 + 19;
				if (hasColorPalette)
				{
					int colorOffset = numOfFrames * 4 + 18;
					finalFile[15] = (byte)(colorOffset / 16777216);
					finalFile[14] = (byte)((colorOffset % 16777216) / 65536);
					finalFile[13] = (byte)((colorOffset % 65536) / 256);
					finalFile[12] = (byte)(colorOffset % 256);
					frameStartOffset += 8 + numOfColors * 3;
				}

				int[] frameOffsets = new int[numOfFrames + 1];
				//Set up frame offsets
				for (int i = 0; i < numOfFrames; i++)
				{
					frameOffsets[i] = frameStartOffset;
					finalFile[18 + (i * 4) + 3] = (byte)(frameStartOffset / 16777216);
					finalFile[18 + (i * 4) + 2] = (byte)((frameStartOffset % 16777216) / 65536);
					finalFile[18 + (i * 4) + 1] = (byte)((frameStartOffset % 65536) / 256);
					finalFile[18 + (i * 4)] = (byte)(frameStartOffset % 256);
					frameStartOffset += compressedFrames[i].Length;
				}

				//Add the final offset that indicates end of file
				frameOffsets[frameOffsets.Length - 1] = frameStartOffset;
				finalFile[18 + (numOfFrames * 4) + 3] = (byte)(frameStartOffset / 16777216);
				finalFile[18 + (numOfFrames * 4) + 2] = (byte)((frameStartOffset % 16777216) / 65536);
				finalFile[18 + (numOfFrames * 4) + 1] = (byte)((frameStartOffset % 65536) / 256);
				finalFile[18 + (numOfFrames * 4)] = (byte)(frameStartOffset % 256);

				if (hasColorPalette)
				{
					//Copy over the palette
					int startPos = 18 + (numOfFrames * 4) + 4;
					int colorStartPos = startPos + 8;
					finalFile[startPos + 3] = (byte)(paletteOffset / 256);
					finalFile[startPos + 2] = (byte)(paletteOffset % 256);
					finalFile[startPos + 1] = (byte)(colorStartPos / 256);
					finalFile[startPos] = (byte)(colorStartPos % 256);

					finalFile[startPos + 4] = (byte)(numOfColors % 256);
					finalFile[startPos + 5] = (byte)(numOfColors / 256);

					for (int i = 0; i < colorPalette.Length; i++)
					{
						finalFile[colorStartPos + i] = colorPalette[i];
					}
				}

				//Now copy the image's contents
				for (int i = 0; i < numOfFrames; i++)
				{
					for (int j = 0; j < compressedFrames[i].Length; j++)
					{
						finalFile[frameOffsets[i] + j] = compressedFrames[i][j];
					}
				}

				return finalFile;
			}
		}

		private void ProcessFrame(byte[] currentFrame, byte[] previousFrame)
		{
			for (uint i = 0; i < currentFrame.Length; i++)
			{
				if (currentFrame[i] == previousFrame[i])
				{
					currentFrame[i] = 0x00;
				}
			}
		}

		private byte[] CompressFrame(byte[] frame, int width, int height)
		{
			int actualFrameLength = 0;
			int currentX = 0;
			byte[] compressedFrame = new byte[frame.Length];
			int iter = 0;
			int compressedIter = 0;
			int i = 0;
			while (i + iter < frame.Length && frame[iter + i] == 0x80)
			{
				//Shift X to the first actual pixel
				i++;
				if (i == height)
				{
					//Move right one
					iter += height;
					compressedFrame[compressedIter] = 0xFF;
					compressedIter++;
					i = 0;
					actualFrameLength++;
					currentX++;
				}
			}
			while (currentX < width && iter < frame.Length)
			{
				//Process each column
				byte currentY = 0;
				while (currentY < height)
				{
					//Get data for miniheader
					//Find the y position
					while (currentY < height && currentY + iter < frame.Length && frame[iter + currentY] == 0x00)
					{
						currentY++;
					}
					if (currentY >= height)
					{
						//Move right one
						iter += height;
						break;
					}
					byte length = 0;
					bool repeated = false;
					while (currentY + length < height && currentY + iter + length < frame.Length && frame[iter + currentY + length] != 0x00)
					{
						if (length > 0 && frame[iter + currentY + length] == frame[iter + currentY + length - 1])
						{
							repeated = true;
						}
						length++;
					}
					compressedFrame[compressedIter] = (byte)(repeated ? 0x80 : 0x00);
					compressedFrame[compressedIter + 3] = currentY;
					byte actualLength = 0;
					if (!repeated)
					{
						for (int k = 0; k < length; k++)
						{
							compressedFrame[compressedIter + 4 + k] = frame[iter + currentY + k];
							actualLength++;
						}
					}
					else
					{
						int k = 0;
						int l = 0;
						while (l < length)
						{
							byte y = 0;
							byte currentColor = frame[iter + currentY + k];
							while (currentY + k + y < height && currentColor == frame[iter + currentY + k + y])
							{
								y++;
								l++;
							}
							if (y > 0)
							{
								compressedFrame[compressedIter + 4 + k] = (byte)(0xE0 + y - 1);
								compressedFrame[compressedIter + 4 + k + 1] = currentColor;
								k += 2;
								actualLength += 2;
							}
							else
							{
								compressedFrame[compressedIter + 4 + k] = currentColor;
								k++;
								l++;
								actualLength++;
							}
						}
					}
					compressedFrame[compressedIter + 1] = (byte)(actualLength + 2);
					compressedFrame[compressedIter + 2] = actualLength;

					compressedIter += (4 + actualLength);
					currentY += length;

					actualFrameLength += (4 + actualLength);
				}
				currentX++;
			}
			byte[] realCompressedFrame = new byte[actualFrameLength];
			for (int j = 0; j < actualFrameLength; j++)
			{
				realCompressedFrame[j] = compressedFrame[j];
			}
			return realCompressedFrame;
		}*/
		#endregion
	}
}
