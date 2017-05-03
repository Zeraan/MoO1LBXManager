using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace LBXManager
{
	internal class InternalFileClass
	{
		private int width;
		private int height;
		private int frames;
		private int frameDelay;
		private int colorPaletteOffsetStart;
		private uint[] frameOffsets;

		private int numOfInternalColors;
		private byte[] internalPalette;
		private int internalPaletteOffset;

		private Bitmap[] bitmaps;

		#region Properties
		public int Width => width;
		public int Height => height;
		public int Frames => frames;

		public uint StartPos { get; set; }
		public uint EndPos { get; set; }
		public string FileName { get; set; }
		public string Comment { get; set; }
		#endregion

		/// <summary>
		/// Convert from bytes to string if this is a text file
		/// </summary>
		/// <param name="lbxFile">LBXFile containing the original bytes</param>
		/// <returns>The parsed text</returns>
		public string GetText(LBXFile lbxFile)
		{
			var bytes = lbxFile.Bytes;
			int length = (int)(EndPos - StartPos);
			char[] letters = new char[length];
			for (uint i = StartPos; i < EndPos; i++)
			{
				letters[i - StartPos] = LBXFile.Lookup[bytes[i]] ? (char)bytes[i] : '-';
			}
			string text = new string(letters);
			return text;
		}

		/// <summary>
		/// Parse the image's attributes, frames, and image bytes
		/// </summary>
		/// <param name="lbxFile">LBXFile containing the original bytes</param>
		public Bitmap[] GetBitmaps(LBXFile lbxFile)
		{
			if (bitmaps != null && bitmaps.Length > 0)
			{
				return bitmaps;
			}

			var bytes = lbxFile.Bytes;

			//Parse the dimensions of the image file
			width = bytes[StartPos] + (bytes[StartPos + 1] * 256);
			height = bytes[StartPos + 2] + (bytes[StartPos + 3] * 256);
			frames = bytes[StartPos + 6] + (bytes[StartPos + 7] * 256);
			frameDelay = bytes[StartPos + 8] + (bytes[StartPos + 9] * 256);

			//Make sure the width is nibble aligned, this is an issue with Windows Bitmap and this works around that
			if (width % 4 != 0)
			{
				width += 4 - (width % 4);
			}

			//Find where each frame starts in the sequence of bytes
			frameOffsets = new uint[frames + 1];
			for (int i = 0; i < frameOffsets.Length; i++)
			{
				frameOffsets[i] = bytes[StartPos + 18 + (i * 4)] +
									bytes[StartPos + 18 + (i * 4) + 1] * 256U +
									bytes[StartPos + 18 + (i * 4) + 2] * 65536U +
									bytes[StartPos + 18 + (i * 4) + 3] * 16777216U;
			}

			numOfInternalColors = 0;
			internalPalette = new byte[0];

			//Load in custom color palette if any
			colorPaletteOffsetStart = bytes[StartPos + 14] + bytes[StartPos + 15] * 256;
			if (colorPaletteOffsetStart > 0)
			{
				uint colorOffset = bytes[StartPos + colorPaletteOffsetStart] +
									bytes[StartPos + colorPaletteOffsetStart + 1] * 256U;
				internalPaletteOffset = bytes[StartPos + colorPaletteOffsetStart + 2] +
								bytes[StartPos + colorPaletteOffsetStart + 3] * 256;
				numOfInternalColors = bytes[StartPos + colorPaletteOffsetStart + 4] +
								bytes[StartPos + colorPaletteOffsetStart + 5] * 256;
				internalPalette = new byte[numOfInternalColors * 3];
				for (int i = 0; i < (numOfInternalColors * 3); i++)
				{
					internalPalette[i] = bytes[StartPos + colorOffset + i];
				}
			}

			return ParseBytesIntoBitmaps(lbxFile);
		}

		/// <summary>
		/// With attributes extracted, we can generate bitmaps from raw bytes
		/// </summary>
		/// <param name="lbxFile"></param>
		/// <returns></returns>
		private Bitmap[] ParseBytesIntoBitmaps(LBXFile lbxFile)
		{
			var bytes = lbxFile.Bytes;

			bitmaps = new Bitmap[frames];
			for (int i = 0; i < bitmaps.Length; i++)
			{
				//Each frame must start with 01 80, otherwise skip
				//Next two bytes are x offset, then next two are number of pixels.  If x offset is 0, then next two bytes are y offset
				bitmaps[i] = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

				//Set up the 256 color palette.  If internal palette specified in the file contains less than 256 colors, then fill the rest with external palette 
				//i.e. 100 colors are defined internally, those are first 100 colors, and the last 156 colors are from last 156 colors of external palette
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
						colorPalette.Entries[j] = Color.FromArgb(LBXFile.ExternalPalette[j * 3] * 4, LBXFile.ExternalPalette[(j * 3) + 1] * 4,
																 LBXFile.ExternalPalette[(j * 3) + 2] * 4);
					}
				}
				//Set the bitmap's palette with the recently generated palette and set up dimensions of bitmap
				bitmaps[i].Palette = colorPalette;
				BitmapData image = bitmaps[i].LockBits(new Rectangle(0, 0, bitmaps[i].Size.Width, bitmaps[i].Size.Height),
													   ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
				IntPtr ptr = image.Scan0;
				int amountOfBytes = Math.Abs(image.Stride) * image.Height;
				byte[] rgbValues = new byte[amountOfBytes];

				for (int k = 0; k < i; k++)
				{
					//This is not the first frame, animated frames works by only storing pixels that actually changed, and overlaying it over previous frame, saving space
					BitmapData baseImage = bitmaps[k].LockBits(new Rectangle(0, 0, bitmaps[k].Size.Width, bitmaps[k].Size.Height),
																ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
					IntPtr basePtr = baseImage.Scan0;
					int baseBytes = Math.Abs(image.Stride) * image.Height;
					System.Runtime.InteropServices.Marshal.Copy(basePtr, rgbValues, 0, baseBytes);
					bitmaps[k].UnlockBits(baseImage);
				}

				//Load in the image
				uint BitmapStart = frameOffsets[i] + StartPos;
				uint BitmapEnd = frameOffsets[i + 1] + StartPos;
				uint BitmapSize = BitmapEnd - BitmapStart;

				uint BitmapIndex = BitmapStart + 1;
				int x = 0;
				int y = height;
				uint next_ctl = 0;
				int long_data = 0;
				uint n_r = 0; //Pointer to which byte we're reading
				uint last_pos = 0;
				int RLE_val = 255;

				//How the whole thing works: In order to save space, the algorithm is pretty simple to understand, but pretty complicated in practice.  Basically, it draws
				//vertically, with first part of a sequence saying how many pixels to draw, and second part is what color to draw the pixels.  So if it says "10 78" it means
				//to draw 10 pixels of the 78th color from the 256-color palette.  If the iteration reaches the vertical limit (height of the image that's defined in header of the file)
				//it moves to next column and resets index to 0.  Sometimes there's no sequence, just single pixels.  Those have different flags telling which is which
				while ((x < width) && (BitmapIndex < BitmapEnd))
				{
					y = 0;
					if (bytes[BitmapIndex] == 0xFF)
					{
						//End of current column of pixels
						BitmapIndex++;
						RLE_val = 256;
					}
					else
					{
						long_data = bytes[BitmapIndex + 2];
						next_ctl = BitmapIndex + bytes[BitmapIndex + 1] + 2;

						if (bytes[BitmapIndex] == 0)
						{
							RLE_val = 256;
						}
						else if (bytes[BitmapIndex] == 0x80)
						{
							RLE_val = 0xE0;
						}
						else
						{
							//See what we have so far
							break;
						}

						y = bytes[BitmapIndex + 3];
						BitmapIndex += 4;

						n_r = BitmapIndex;
						while (n_r < next_ctl)
						{
							while ((n_r < BitmapIndex + long_data) && x < width)
							{
								if (bytes[n_r] >= RLE_val)
								{
									last_pos = n_r + 1;
									int RleLength = (bytes[n_r] - RLE_val) + 1;
									if (RleLength + y > height)
									{
										throw new Exception();
									}
									int RleCounter = 0;
									while ((RleCounter < RleLength) && y < height)
									{
										if ((x < width) && y < height && x >= 0 && y >= 0)
										{
											rgbValues[(width * y) + x] = bytes[last_pos];
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
										rgbValues[(width * y) + x] = bytes[n_r];
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
								y += bytes[n_r + 1];
								BitmapIndex = n_r + 2;
								long_data = bytes[n_r];

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
				System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, amountOfBytes);
				bitmaps[i].UnlockBits(image);
			}
			
			return bitmaps;
		}
	}
}
