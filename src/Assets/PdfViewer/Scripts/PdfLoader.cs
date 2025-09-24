using System;
using System.Collections.Generic;
using System.IO;
using SkiaSharp;
using UnityEngine;
using PDFtoImage;

namespace UnityPdfViewer
{
    public static class PdfLoader
    {
        // Load PDF from path and convert each page to Texture2D
        public static Texture2D[] LoadPdfAsTextures(string pdfPath, int dpi)
        {
            if (!File.Exists(pdfPath))
            {
                Debug.LogError($"PDF file not found: {pdfPath}");
                return Array.Empty<Texture2D>();
            }

            List<Texture2D> textures = new List<Texture2D>();

            try
            {
                using (var fileStream = File.OpenRead(pdfPath))
                {
                    // Convert PDF pages to Skia bitmaps
                    IEnumerable<SKBitmap> bitmaps = Conversion.ToImages(fileStream, false, null, new RenderOptions { Dpi = dpi });

                    foreach (var skBitmap in bitmaps)
                    {
                        textures.Add(ConvertSKBitmapToTexture2D(skBitmap)); // convert to Unity Texture2D
                        skBitmap.Dispose(); // free Skia memory
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error converting PDF: {e.Message}");
            }

            return textures.ToArray();
        }

        // Convert SKBitmap to Unity Texture2D
        private static Texture2D ConvertSKBitmapToTexture2D(SKBitmap skBitmap)
        {
            if (skBitmap == null) return null;

            Texture2D texture = new Texture2D(skBitmap.Width, skBitmap.Height, TextureFormat.RGBA32, false);

            var pixels = skBitmap.Pixels;
            Color32[] unityPixels = new Color32[pixels.Length];

            // Copy colors from SKColor to Color32
            for (int i = 0; i < pixels.Length; i++)
            {
                var c = pixels[i];
                unityPixels[i] = new Color32(c.Red, c.Green, c.Blue, c.Alpha);
            }

            // Flip vertically to match Unity's coordinate system
            Color32[] flippedPixels = new Color32[unityPixels.Length];
            int width = skBitmap.Width;
            int height = skBitmap.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int sourceIndex = y * width + x;
                    int destIndex = (height - 1 - y) * width + x;
                    flippedPixels[destIndex] = unityPixels[sourceIndex];
                }
            }

            texture.SetPixels32(flippedPixels);
            texture.Apply(); // upload pixels to GPU

            return texture;
        }
    }
}
