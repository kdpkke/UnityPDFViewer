using System;
using UnityEngine;

public class PdfNavigator
{
    public Texture2D[] Pages { get; private set; } // all PDF pages
    public int CurrentPage { get; private set; }   // current page index
    public int TotalPages => Pages?.Length ?? 0;   // total number of pages

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
}