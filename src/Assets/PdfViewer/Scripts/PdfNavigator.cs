using System;
using UnityEngine;

namespace UnityPdfViewer
{
    public class PdfNavigator : IDisposable
    {
        public Texture2D[] Pages { get; private set; } // all PDF pages
        public int CurrentPage { get; private set; }   // current page index
        public int TotalPages => Pages?.Length ?? 0;   // total number of pages

        private bool isDisposed = false; // To prevent multiple dispose calls
        
        public PdfNavigator(Texture2D[] pages)
        {
            Pages = pages ?? Array.Empty<Texture2D>();
            CurrentPage = 0;
        }

        // Go to next page
        public void Next()
        {
            if (Pages.Length == 0) return;
            CurrentPage = Mathf.Min(CurrentPage + 1, Pages.Length - 1);
        }

        // Go to previous page
        public void Previous()
        {
            if (Pages.Length == 0) return;
            CurrentPage = Mathf.Max(CurrentPage - 1, 0);
        }

        // Go to specific page (1-based)
        public void GoTo(int pageNumber)
        {
            if (Pages.Length == 0) return;
            CurrentPage = Mathf.Clamp(pageNumber - 1, 0, Pages.Length - 1);
        }
        
        public void Dispose()
        {
            // Prevent disposing more than once
            if (isDisposed) return;

            if (Pages != null)
            {
                foreach (var page in Pages)
                {
                    if (page != null)
                    {
                        // This is the crucial part for releasing texture memory in Unity
                        UnityEngine.Object.Destroy(page);
                    }
                }
                Pages = null; // Clear the array reference
            }

            isDisposed = true;
        }
    }
}