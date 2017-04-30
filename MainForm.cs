using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace LBXManager
{
	public partial class MainForm : Form
	{
		private class FileClass
		{
			public uint startPos;
			public uint endPos;
			public string fileName;
			public string comment;
		}

		private byte[] lbxFile;
		private int fileCount;
		private short fileType;
		private FileClass[] files;
		private static bool[] lookup;
		private byte[] externalPalette;
		private int internalPaletteOffset;
		private int numOfInternalColors;
		private byte[] internalPalette;
		private Bitmap[] bitmaps;
		private int frameDelay;

		public MainForm()
		{
			InitializeComponent();
			lookup = new bool[65535];
			for (char c = '0'; c <= '9'; c++)
			{
				lookup[c] = true;
			}
			for (char c = 'A'; c <= 'Z'; c++)
			{
				lookup[c] = true;
			}
			for (char c = 'a'; c <= 'z'; c++)
			{
				lookup[c] = true;
			}
			lookup['.'] = true;
			lookup[','] = true;
			lookup[' '] = true;
		}

		private void browseButton_Click(object sender, EventArgs e)
		{
			openFileDialog.Multiselect = false;
			openFileDialog.Filter = "LBX files (*.LBX)|*.LBX";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				lbxFilePathTextBox.Text = openFileDialog.FileName;
			}
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(lbxFilePathTextBox.Text))
			{
				MessageBox.Show("Please select a LBX file to load.");
				return;
			}
			try
			{
				FileInfo fi = new FileInfo(lbxFilePathTextBox.Text);
				using (FileStream fileStream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.None))
				{
					lbxFile = ReadFile(fileStream);
					if (!ParseMiniHeader())
					{
						lbxFile = null;
						MessageBox.Show("This is not a valid LBX file.");
						return;
					}
				}
				using (FileStream fileStream = new FileStream(Path.Combine(fi.Directory.FullName, "FONTS.LBX"), FileMode.Open, FileAccess.Read, FileShare.None))
				{
					byte[] fontFile = ReadFile(fileStream);
					externalPalette = new byte[768];
					for (int i = 0; i < externalPalette.Length; i++)
					{
						externalPalette[i] = fontFile[13444 + i];
					}
				}
			}
			catch(Exception exception)
			{
				lbxFile = null;
				MessageBox.Show("An exception occured while loading LBX file: " + exception.Message);
			}
		}

		private static byte[] ReadFile(Stream stream)
		{
			byte[] buffer = new byte[32768];
			using (MemoryStream ms = new MemoryStream())
			{
				while (true)
				{
					int read = stream.Read(buffer, 0, buffer.Length);
					if (read <= 0)
					{
						return ms.ToArray();
					}
					ms.Write(buffer, 0, read);
				}
			}
		}

		private bool sigCheck()
		{
			if (lbxFile[2] == 0xAD && lbxFile[3] == 0xFE && lbxFile[4] == 0 && lbxFile[5] == 0)
			{
				return true;
			}
			return false;
		}

		private bool ParseMiniHeader()
		{
			if (lbxFile == null || lbxFile.Length < 8)
			{
				return false;
			}
			fileCount = (short) (lbxFile[1]*256 + lbxFile[0]);
			if (!sigCheck())
			{
				return false;
			}
			fileType = (short) (lbxFile[7]*256 + lbxFile[6]);
			numOfFiles.Text = "Number of Files: " + fileCount;
			lbxVersionLabel.Text = "LBX Version: " + fileType;
			LoadFiles();
			return true;
		}

		private void LoadFiles()
		{
			fileList.Items.Clear();
			files = new FileClass[fileCount];
			for (int i = 0; i < fileCount; i++)
			{
				FileClass newFile = new FileClass();
				if (fileType == 0)
				{
					char[] name = new char[8];
					char[] comment = new char[24];
					for (int k = 0; k < 8; k++)
					{
						name[k] = lookup[(char)lbxFile[512 + (i*32) + k]] ? (char)lbxFile[512 + (i*32) + k] : ' ';
					}
					for (int k = 0; k < 24; k++)
					{
						comment[k] = lookup[(char) lbxFile[520 + (i*32) + k]] ? (char) lbxFile[520 + (i*32) + k] : ' ';
					}
					newFile.comment = new string(comment);
					newFile.fileName = new string(name);
				}
				else
				{
					newFile.fileName = "File " + (i + 1);
				}
				int currentPos = 8 + (i*4);
				newFile.startPos = lbxFile[currentPos + 0] + (uint)(lbxFile[currentPos + 1] * 256) + (uint)(lbxFile[currentPos + 2] * 65536) +
								   (uint)(lbxFile[currentPos + 3] * 16777216);
				newFile.endPos = lbxFile[currentPos + 4] + (uint)(lbxFile[currentPos + 5] * 256) + (uint)(lbxFile[currentPos + 6] * 65536) +
								   (uint)(lbxFile[currentPos + 7] * 16777216);
				fileList.Items.Add(newFile.fileName + " " + newFile.comment);
				files[i] = newFile;
			}
		}

		private void fileList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				FileClass file = files[fileList.SelectedIndex];
				int length = (int)(file.endPos - file.startPos);
				if (fileType == 0)
				{
					TryLoadImage(file);
				}
				else if (fileType == 5)
				{
					char[] letters = new char[length];
					for (uint i = file.startPos; i < file.endPos; i++)
					{
						letters[i - file.startPos] = lookup[lbxFile[i]] ? (char) lbxFile[i] : '-';
					}
					string text = new string(letters);
					textBox2.Text = text;
				}
			}
			catch(Exception exception)
			{
				MessageBox.Show("Failed to load this file: " + exception.Message);
			}
		}

		private void TryLoadImage(FileClass file)
		{
			//This is relative to file.startPos, in the format of "Starting Index, count of bytes"
			//0, 2 is image width
			//2, 2 is image height
			//4, 2 is always 0
			//6, 2 is frames
			//8, 2 is frame delay
			//10, 2 is parameters
			//12, 2 is unknown, maybe internal palette (would be different from MoO 2's format if this is the case)?
			//14, ((frames + 1) * 4) is frameoffsets
			//if internalPalette is true, then ((frames + 1) * 4 + 14), 4 with first two for color shift (position in palette), then the next two is amount of colors (if 12, 30, then it is between 12 and 42 in 256 colors for palette)
			//if internalPalette is true, then ((frames + 1) * 4 + 18), (4 * number of colors) is color value (ARGB, with each byte needing to be multiplied by 4)
			//((frames + 1) * 4 + 14) or ((frames + 1) * 4 + 18) + (4 * number of colors) henceforth referred as start of image, 2 is number of pixels in sequence.  
			//If 0, then read next 2 for vertical offset and reset x to 0 (have no pixel data), otherwise next 2 for horizontal offset (x + offset)
			//Pixel data starts at file.startPos + frameOffsetX + 4
			//Read pixel data (each pixel have a value between 0 - 255), so 1 byte per pixel.  If number of pixels is odd value, discard the 2nd byte after each pixel
			//Read the image header
			int width = lbxFile[file.startPos] + (lbxFile[file.startPos + 1]*256);
			int height = lbxFile[file.startPos + 2] + (lbxFile[file.startPos + 3]*256);
			int frames = lbxFile[file.startPos + 6] + (lbxFile[file.startPos + 7]*256);
			frameDelay = lbxFile[file.startPos + 8] + (lbxFile[file.startPos + 9] * 256);
			widthLabel.Text = "Width: " + width;
			heightLabel.Text = "Height: " + height;
			frameLabel.Text = "Frames: " + frames;
			if (width % 4 != 0)
			{
				width += 4 - (width%4);
			}
			if (frames > 1)
			{
				framesBar.Enabled = true;
				framesBar.SetRange(0, frames - 1);
			}
			else
			{
				framesBar.Enabled = false;
			}
			int colorPaletteOffsetStart = lbxFile[file.startPos + 14] + lbxFile[file.startPos + 15]*256;

			uint[] frameOffsets = new uint[frames + 1];

			for (int i = 0; i < frameOffsets.Length; i++)
			{
				frameOffsets[i] = lbxFile[file.startPos + 18 + (i*4)] +
				                  lbxFile[file.startPos + 18 + (i*4) + 1]*256U +
				                  lbxFile[file.startPos + 18 + (i*4) + 2]*65536U +
				                  lbxFile[file.startPos + 18 + (i*4) + 3]*16777216U;
			}
			numOfInternalColors = 0;
			internalPalette = new byte[0];
			if (colorPaletteOffsetStart > 0)
			{
				uint colorOffset = lbxFile[file.startPos + colorPaletteOffsetStart] +
				                   lbxFile[file.startPos + colorPaletteOffsetStart + 1]*256U;
				internalPaletteOffset = lbxFile[file.startPos + colorPaletteOffsetStart + 2] +
							    lbxFile[file.startPos + colorPaletteOffsetStart + 3] * 256;
				numOfInternalColors = lbxFile[file.startPos + colorPaletteOffsetStart + 4] +
				              lbxFile[file.startPos + colorPaletteOffsetStart + 5]*256;
				internalPalette = new byte[numOfInternalColors * 3];
				for (int i = 0; i < (numOfInternalColors * 3); i++)
				{
					internalPalette[i] = lbxFile[file.startPos + colorOffset + i];
				}
			}

			//Handle internal palettes

			bitmaps = new Bitmap[frames];
			for (int i = 0; i < bitmaps.Length; i++)
			{
				//Each frame must start with 01 80, otherwise skip
				//Next two bytes are x offset, then next two are number of pixels.  If x offset is 0, then next two bytes are y offset
				bitmaps[i] = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
				ColorPalette colorPalette = bitmaps[i].Palette;
				for (int j = 0; j < 256; j++)
				{
					if (j >= internalPaletteOffset && j < numOfInternalColors + internalPaletteOffset)
					{
						//Load the internal palette first before using external one
						colorPalette.Entries[j] = Color.FromArgb(internalPalette[(j - internalPaletteOffset) * 3] * 4, internalPalette[((j - internalPaletteOffset) * 3) + 1] * 4,
																 internalPalette[((j - internalPaletteOffset) * 3) + 2] * 4);
					}
					else
					{
						colorPalette.Entries[j] = Color.FromArgb(externalPalette[j*3]*4, externalPalette[(j*3) + 1]*4,
						                                         externalPalette[(j*3) + 2]*4);
					}
				}
				bitmaps[i].Palette = colorPalette;
				BitmapData image = bitmaps[i].LockBits(new Rectangle(0, 0, bitmaps[i].Size.Width, bitmaps[i].Size.Height),
				                                       ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
				IntPtr ptr = image.Scan0;
				int bytes = Math.Abs(image.Stride)*image.Height;
				byte[] rgbValues = new byte[bytes];

				for (int k = 0; k < i; k++)
				{
					//This is not the first frame, frames basically overwrite the base frame with whatever exists
					BitmapData baseImage = bitmaps[k].LockBits(new Rectangle(0, 0, bitmaps[k].Size.Width, bitmaps[k].Size.Height),
						                                        ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
					IntPtr basePtr = baseImage.Scan0;
					int baseBytes = Math.Abs(image.Stride)*image.Height;
					System.Runtime.InteropServices.Marshal.Copy(basePtr, rgbValues, 0, baseBytes);
					bitmaps[k].UnlockBits(baseImage);
				}
				
				//Load in the image
				uint BitmapStart = frameOffsets[i] + file.startPos;
				uint BitmapEnd = frameOffsets[i + 1] + file.startPos;
				uint BitmapSize = BitmapEnd - BitmapStart;

				uint BitmapIndex = BitmapStart + 1;
				int x = 0;
				int y = height;
				uint next_ctl = 0;
				int long_data = 0;
				uint n_r = 0;
				uint last_pos = 0;
				int RLE_val = 255;

				while ((x < width) && (BitmapIndex < BitmapEnd))
				{
					y = 0;
					if (lbxFile[BitmapIndex] == 0xFF)
					{
						BitmapIndex++;
						RLE_val = 256;
					}
					else
					{
						long_data = lbxFile[BitmapIndex + 2];
						next_ctl = BitmapIndex + lbxFile[BitmapIndex + 1] + 2;

						if (lbxFile[BitmapIndex] == 0)
						{
							RLE_val = 256;
						}
						else if (lbxFile[BitmapIndex] == 0x80)
						{
							RLE_val = 0xE0;
						}
						else
						{
							//See what we have so far
							break;
						}

						y = lbxFile[BitmapIndex + 3];
						BitmapIndex += 4;

						n_r = BitmapIndex;
						while (n_r < next_ctl)
						{
							while ((n_r < BitmapIndex + long_data) && x < width)
							{
								if (lbxFile[n_r] >= RLE_val)
								{
									last_pos = n_r + 1;
									int RleLength = (lbxFile[n_r] - RLE_val) + 1;
									if (RleLength + y > height)
									{
										throw new Exception();
									}
									int RleCounter = 0;
									while ((RleCounter < RleLength) && y < height)
									{
										if ((x < width) && y < height && x >= 0 && y >= 0)
										{
											rgbValues[(width * y) + x] = lbxFile[last_pos];
										}
										else
										{
											throw new Exception();
										}
										y++;
										RleCounter++;
									}
									n_r += 2;
								}
								else //Regular single pixel
								{
									if ((x < width) && y < height && x >= 0 && y >= 0)
									{
										rgbValues[(width * y) + x] = lbxFile[n_r];
									}
									else
									{
										throw new Exception();
									}
									n_r++;
									y++;
								}
							}

							if (n_r < next_ctl)
							{
								y += lbxFile[n_r + 1];
								BitmapIndex = n_r + 2;
								long_data = lbxFile[n_r];

								n_r += 2;
								if (n_r >= next_ctl)
								{
									throw new Exception();
								}
							}
						}
						BitmapIndex = next_ctl; //Jump to next line
					}
					x++;
				}
				System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
				bitmaps[i].UnlockBits(image);
			}
			pictureBox1.Image = bitmaps[0];
			pictureBox1.Refresh();
		}

		private void framesBar_ValueChanged(object sender, EventArgs e)
		{
			pictureBox1.Image = bitmaps[framesBar.Value];
			pictureBox1.Refresh();
		}

		private void saveBMPButton_Click(object sender, EventArgs e)
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
							foreach (FileClass file in files)
							{
								baseWriter.WriteLine("FileName:" + file.fileName + " Comment:" + file.comment);
								TryLoadImage(file);
								using (
									TextWriter writer = new StreamWriter(Path.Combine(folderBrowser.SelectedPath, file.fileName + "HDR.TXT"), false)
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
											writer.WriteLine("R:" + internalPalette[i*3]*4 + " G:" + internalPalette[i*3 + 1]*4 + " B:" +
											                 internalPalette[i*3 + 2]*4);
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
									bitmaps[i].Save(Path.Combine(folderBrowser.SelectedPath, file.fileName + i + ".BMP"));
								}
							}
						}
					}
					catch(Exception exception)
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
					char[] splitter = new[] {':'};
					string line = reader.ReadLine();
					string LBXFileName = line.Split(splitter)[1];
					line = reader.ReadLine();
					int numOfFilesToLoad = int.Parse(line.Split(splitter)[1]);
					string[] fileNames = new string[numOfFilesToLoad];
					string[] fileComments = new string[numOfFilesToLoad];
					for (int i = 0; i < numOfFilesToLoad; i++)
					{
						string[] components = reader.ReadLine().Split(new[] {':', ' '});
						fileNames[i] = string.Format("{0,-8}", components[1]);
						fileComments[i] = string.Format("{0,-24}", components[3]);
					}
					//Load in file data
					byte[][] filesData = new byte[numOfFilesToLoad][];
					for (int i = 0; i < numOfFilesToLoad; i++)
					{
						filesData[i] = LoadInFile(fileNames[i], importPath);
					}
					uint length = 512U + (32U*(uint)numOfFilesToLoad);
					uint[] offsets = new uint[numOfFilesToLoad + 1];
					for (int i = 0; i < numOfFilesToLoad; i++)
					{
						offsets[i] = length;
						length += (uint)filesData[i].Length;
					}
					offsets[offsets.Length - 1] = length;

					byte[] finalLBXStream = new byte[length];
					finalLBXStream[0] = (byte) (numOfFilesToLoad%256);
					finalLBXStream[1] = (byte) (numOfFilesToLoad/256);
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
						for (int k = 0; k < 8; k ++)
						{
							finalLBXStream[512 + (i*32) + k] = (byte)fileNames[i][k];
						}
						for (int k = 0; k < 24; k++)
						{
							finalLBXStream[520 + (i*32) + k] = (byte) fileComments[i][k];
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

				numOfFrames = int.Parse(reader.ReadLine().Split(new[] {':'})[1]);
				frameDelay = int.Parse(reader.ReadLine().Split(new[] {':'})[1]);
				hasColorPalette = string.Compare(reader.ReadLine().Split(new[] {':'})[1], "True") == 0;
				if (hasColorPalette)
				{
					char[] splitter = new[] {':'};
					numOfColors = int.Parse(reader.ReadLine().Split(new[] {':'})[1]);
					paletteOffset = int.Parse(reader.ReadLine().Split(new[] { ':' })[1]);

					colorPalette = new byte[numOfColors * 3];
					for (int i = 0; i < numOfColors; i++)
					{
						string[] components = reader.ReadLine().Split(new[] {' '});
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

				uint actualLength = 18U + (uint)numOfFrames*4U;
				if (hasColorPalette)
				{
					actualLength += 16U + (uint) (numOfColors*3);
				}

				actualLength++; //Add 1 for start of frames (01)
				for (int i = 0; i < numOfFrames; i++)
				{
					actualLength += (uint)compressedFrames[i].Length;
				}

				byte[] finalFile = new byte[actualLength];

				//Load the file into one big array

				//Set up the header of the file
				finalFile[0] = (byte) (width%256);
				finalFile[1] = (byte) (width/256);
				finalFile[2] = (byte) (height%256);
				finalFile[3] = (byte) (height/256);
				finalFile[6] = (byte) (numOfFrames);
				finalFile[8] = (byte) (frameDelay%256);
				finalFile[9] = (byte) (frameDelay/256);

				int frameStartOffset = numOfFrames*4 + 19;
				if (hasColorPalette)
				{
					int colorOffset = numOfFrames*4 + 18;
					finalFile[15] = (byte) (colorOffset/16777216);
					finalFile[14] = (byte) ((colorOffset%16777216)/65536);
					finalFile[13] = (byte) ((colorOffset%65536)/256);
					finalFile[12] = (byte) (colorOffset%256);
					frameStartOffset += 8 + numOfColors*3;
				}

				int[] frameOffsets = new int[numOfFrames + 1];
				//Set up frame offsets
				for (int i = 0; i < numOfFrames; i++ )
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
					int startPos = 18 + (numOfFrames*4) + 4;
					int colorStartPos = startPos + 8;
					finalFile[startPos + 3] = (byte)(paletteOffset / 256);
					finalFile[startPos + 2] = (byte)(paletteOffset % 256);
					finalFile[startPos + 1] = (byte)(colorStartPos / 256);
					finalFile[startPos] = (byte)(colorStartPos % 256);

					finalFile[startPos + 4] = (byte)(numOfColors%256);
					finalFile[startPos + 5] = (byte) (numOfColors/256);

					for (int i = 0; i < colorPalette.Length; i++)
					{
						finalFile[colorStartPos + i] = colorPalette[i];
					}
				}

				//Now copy the image's contents
				for (int i = 0; i < numOfFrames; i++ )
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
								compressedFrame[compressedIter + 4 + k] = (byte) (0xE0 + y - 1);
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
		}
	}
}
