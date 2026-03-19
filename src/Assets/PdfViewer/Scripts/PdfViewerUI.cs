using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

namespace UnityPdfViewer
{
    public class PdfViewerUI : MonoBehaviour
    {
        [Header("UI References")]
        public RawImage pdfImage;      // display PDF page
        public TMP_Text pageIndicator;     // show current page
        public Button nextButton;      // next page button
        public Button previousButton;  // previous page button

        [Header("PDF Settings")]
        [Range(72, 300)] public int renderDPI = 150;

        [Tooltip("If true, the path is relative to StreamingAssets (e.g. \"docs/manual.pdf\").\n" +
                 "If false, the path must be an absolute path (e.g. \"C:/PDFs/manual.pdf\").")]
        public bool useStreamingAssets = true;

        [HideInInspector]
        public PdfNavigator navigator;

        private string pdfPath;

        protected void Start()
        {
            nextButton?.onClick.AddListener(NextPage);
            previousButton?.onClick.AddListener(PreviousPage);
        }

        /// <summary>
        /// Loads a PDF file. The path is resolved based on <see cref="useStreamingAssets"/>:
        /// <list type="bullet">
        ///   <item><c>useStreamingAssets = true</c> (default): pass a relative path inside StreamingAssets (e.g. "manual.pdf" or "docs/manual.pdf").</item>
        ///   <item><c>useStreamingAssets = false</c>: pass an absolute file path (e.g. "C:/Users/me/Documents/manual.pdf").</item>
        /// </list>
        /// </summary>
        public void LoadPDF(string path)
        {
            if (navigator != null)
            {
                navigator.Dispose();
                navigator = null;
            }

            pdfPath = useStreamingAssets
                ? Path.Combine(Application.streamingAssetsPath, path)
                : path;

            Texture2D[] pages = PdfLoader.LoadPdfAsTextures(pdfPath, renderDPI);
            navigator = new PdfNavigator(pages);

            UpdateUI();
        }

        public void NextPage()
        {
            navigator.Next();
            UpdateUI();
        }

        public void PreviousPage()
        {
            navigator.Previous();
            UpdateUI();
        }
        
        public void GoToPage(int pageNumber)
        {
            navigator.GoTo(pageNumber);
            UpdateUI();
        }

        // update RawImage, page text, button states
        private void UpdateUI()
        {
            if (navigator.Pages.Length == 0 || pdfImage == null) return;

            pdfImage.texture = navigator.Pages[navigator.CurrentPage];

            if (pageIndicator != null)
                pageIndicator.text = $"Page {navigator.CurrentPage + 1} / {navigator.TotalPages}";

            if (nextButton != null)
                nextButton.interactable = navigator.CurrentPage < navigator.TotalPages - 1;

            if (previousButton != null)
                previousButton.interactable = navigator.CurrentPage > 0;
        }

        private void OnDestroy()
        {
            // free textures when object is destroyed
            if (navigator?.Pages != null)
            {
                foreach (var page in navigator.Pages)
                {
                    if (page != null) Destroy(page);
                }
            }
        }
    }
}