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
        public string pdfFileName = "document.pdf"; // PDF in StreamingAssets
        [Range(72, 300)] public int renderDPI = 150; // PDF render DPI

        private PdfNavigator navigator; // page navigation
        private string pdfPath;         // full path to PDF

        private void Start()
        {
            pdfPath = Path.Combine(Application.streamingAssetsPath, pdfFileName);

            // load PDF pages
            Texture2D[] pages = PdfLoader.LoadPdfAsTextures(pdfPath, renderDPI);
            navigator = new PdfNavigator(pages);

            UpdateUI(); // display first page

            // attach buttons
            nextButton?.onClick.AddListener(NextPage);
            previousButton?.onClick.AddListener(PreviousPage);
        }

        private void NextPage()
        {
            navigator.Next();
            UpdateUI();
        }

        private void PreviousPage()
        {
            navigator.Previous();
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