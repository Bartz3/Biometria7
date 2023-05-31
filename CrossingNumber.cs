using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace WpfApp
{
	// Zwraca double i bierze za parametry double mean, double std
	public enum MinutiaeType
	{
		Ending,      // 1 czarny piksel
		Bifurcation, // 3 czarne piksele po bokach
		Crossing,    // 4 czarne piksele po bokach
	}

	public static class CrossingNumber
	{
		public unsafe static Bitmap Apply(Bitmap bmp, out Dictionary<MinutiaeType, int> minutiaes)
		{
			minutiaes = new Dictionary<MinutiaeType, int>();
			int m1 = 0, m3 = 0, m4 = 0;
			minutiaes.Add(MinutiaeType.Ending, m1);
			minutiaes.Add(MinutiaeType.Bifurcation, m1);
			minutiaes.Add(MinutiaeType.Crossing, m4);

			var data = bmp.LockBits(ImageLockMode.ReadWrite);

			int stride = data.Stride;
			int height = data.Height;

			int offset = stride + 3;

			byte* ptr = (byte*)data.Scan0.ToPointer();

            int edgeThreshold = offset;

            for (int i = offset; i < stride * height - offset; i += 3)
            {
                if (ptr[i] != White)
                {
                    int sum = 0;
                    if (ptr[i - 3] != White) ++sum;
                    if (ptr[i + 3] != White) ++sum;
                    if (ptr[i - stride] != White) ++sum;
                    if (ptr[i + stride] != White) ++sum;

                    //int row = i / stride;
                    //int col = (i % stride) / 3;

                    //bool isOnEdge = row <= edgeThreshold || row >= (height - edgeThreshold - 1) ||
                    //                col <= edgeThreshold || col >= (stride / 3 - edgeThreshold - 1);

                    if (sum == 1 )
                    {
                        ptr[i] = 254;
                        minutiaes[MinutiaeType.Ending]++;
                        //bool isEnding = true;
                        //if (ptr[i - 3] != White || ptr[i + 3] != White || ptr[i - stride] != White || ptr[i + stride] != White)
                        //    isEnding = false;

                        //if (isEnding)
                        //{
                        //    ptr[i] = 254;
                        //    minutiaes[MinutiaeType.Ending]++;
                        //}
                    }
                    else if (sum == 3)
                    {
                        ptr[i + 1] = 254;
                        minutiaes[MinutiaeType.Bifurcation]++;
                    }
                    else if (sum == 4)
                    {
                        ptr[i + 2] = 156;
                        ptr[i + 1] = 32;
                        minutiaes[MinutiaeType.Crossing]++;
                    }
                }
            }


            bmp.UnlockBits(data);
			return bmp;
		}

		private const byte White = 255;
	}



}