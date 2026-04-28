using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

namespace UnityPdfViewer
{
    /// <summary>
    /// Defines how the path passed to <see cref="PdfViewerUI.LoadPDF(string)"/> is resolved.
    /// </summary>
    public enum PdfPathMode
    {
        /// <summary>
        /// The path is used as-is (must be a full absolute path, e.g. "C:/Docs/file.pdf").
        /// </summary>
        Absolute,

        /// <summary>
        /// The path is appended to the project root folder in the Editor,
        /// or to the folder that contains the built executable in a Player build.
        /// Example: "Docs/file.pdf" -> "&lt;projectRoot&gt;/Docs/file.pdf" in Editor,
        /// or "&lt;buildFolder&gt;/Docs/file.pdf" in a built game.
        /// Use this for PDFs that live OUTSIDE the build (e.g. a folder next to the .exe).
        /// </summary>
        RelativeToProjectFolder,

        /// <summary>
        /// The path is appended to <see cref="Application.streamingAssetsPath"/>.
        /// Example: "presence.pdf" -> "&lt;projectRoot&gt;/Assets/StreamingAssets/presence.pdf" in Editor,
        /// or "&lt;buildFolder&gt;/&lt;gameName&gt;_Data/StreamingAssets/presence.pdf" in a built standalone game.
        /// Use this for PDFs that are BUNDLED with the build inside the StreamingAssets folder
        /// (the standard Unity way to ship read-only data with a game).
        /// NOTE: on Android, streamingAssetsPath points inside the APK and direct file access
        /// will not work — use UnityWebRequest to copy the file to a writable location first.
        /// </summary>
        RelativeToStreamingAssets
    }

    public class PdfViewerUI : MonoBehaviour
    {
        [Header("UI References")]
        public RawImage pdfImage;      // display PDF page
        public TMP_Text pageIndicator;     // show current page
        public Button nextButton;      // next page button
        public Button previousButton;  // previous page button

        [Range(72, 300)] public int renderDPI = 150; // PDF render DPI

        [Header("Path Configuration")]
        [Tooltip(
            "Controls how 'Pdf Path' (and any string passed to LoadPDF) is turned into a real file path.\n" +
            "Pick the mode first, then write 'Pdf Path' accordingly.\n" +
            "\n" +
            "• Absolute\n" +
            "    The string IS the full file path. Pass the same kind of value you'd type in Explorer.\n" +
            "    Example: \"C:/Docs/manual.pdf\"  or  \"E:/Builds/Game/manual.pdf\"\n" +
            "\n" +
            "• RelativeToProjectFolder\n" +
            "    Base = the project root in the Editor, OR the folder containing the .exe in a Player build.\n" +
            "    Use this for PDFs that live OUTSIDE the build (drop a folder next to the .exe at deploy time).\n" +
            "    Example: \"Docs/manual.pdf\"\n" +
            "        Editor -> <projectRoot>/Docs/manual.pdf\n" +
            "        Build  -> <buildFolder>/Docs/manual.pdf\n" +
            "    NOTE: this mode is asymmetric for files INSIDE the build, because Editor uses 'Assets/...' " +
            "and a standalone Player uses '<gameName>_Data/...'. For files inside the build, use " +
            "RelativeToStreamingAssets instead.\n" +
            "\n" +
            "• RelativeToStreamingAssets  (recommended for PDFs shipped with the game)\n" +
            "    Base = Application.streamingAssetsPath. Resolves identically in Editor and standalone build.\n" +
            "    Drop your PDF in '<projectRoot>/Assets/StreamingAssets/' and Unity copies it next to the build " +
            "into '<gameName>_Data/StreamingAssets/' automatically.\n" +
            "    Example: \"presence.pdf\"  or  \"manuals/presence.pdf\"\n" +
            "        Editor -> <projectRoot>/Assets/StreamingAssets/presence.pdf\n" +
            "        Build  -> <buildFolder>/<gameName>_Data/StreamingAssets/presence.pdf")]
        public PdfPathMode pathMode = PdfPathMode.Absolute;

        [Tooltip(
            "Path of the PDF to load. How this string is interpreted depends on 'Path Mode' above.\n" +
            "  - Absolute                  -> full disk path, e.g. \"C:/Docs/manual.pdf\"\n" +
            "  - RelativeToProjectFolder   -> e.g. \"Docs/manual.pdf\" (next to .exe / project root)\n" +
            "  - RelativeToStreamingAssets -> e.g. \"presence.pdf\" (file shipped in Assets/StreamingAssets)\n" +
            "\n" +
            "This is the path used by the parameterless LoadPDF() overload. " +
            "You can still override it at runtime by calling LoadPDF(string) or LoadPDF(string, PdfPathMode).")]
        public string pdfPath = "SamplePDF.pdf";

        [HideInInspector]
        public PdfNavigator navigator;

        // Last resolved absolute path actually used to load a PDF. Cached for diagnostics.
        private string _resolvedPath;

        protected void Start()
        {
            nextButton?.onClick.AddListener(NextPage);
            previousButton?.onClick.AddListener(PreviousPage);
        }

        /// <summary>
        /// Loads the PDF using <see cref="pdfPath"/> and <see cref="pathMode"/> as configured in the Inspector.
        /// </summary>
        public void LoadPDF()
        {
            LoadPDF(pdfPath, pathMode);
        }

        /// <summary>
        /// Loads a PDF from disk, using the <see cref="pathMode"/> selected in the Inspector.
        /// </summary>
        public void LoadPDF(string path)
        {
            LoadPDF(path, pathMode);
        }

        /// <summary>
        /// Loads a PDF from disk, explicitly choosing how to resolve the path
        /// (overrides the <see cref="pathMode"/> set in the Inspector for this single call).
        /// </summary>
        public void LoadPDF(string path, PdfPathMode mode)
        {
            if (navigator != null)
            {
                navigator.Dispose();
                navigator = null;
            }

            _resolvedPath = ResolvePath(path, mode);

            Texture2D[] pages = PdfLoader.LoadPdfAsTextures(_resolvedPath, renderDPI);
            navigator = new PdfNavigator(pages);

            UpdateUI();
        }

        /// <summary>
        /// Resolves a user-supplied path according to the selected <see cref="PdfPathMode"/>.
        /// </summary>
        public static string ResolvePath(string path, PdfPathMode mode)
        {
            if (string.IsNullOrEmpty(path)) return path;

            switch (mode)
            {
                case PdfPathMode.RelativeToProjectFolder:
                    // Application.dataPath:
                    //   Editor  -> "<projectRoot>/Assets"
                    //   Player  -> "<buildFolder>/<gameName>_Data"
                    // GetDirectoryName gives the project root in the Editor and the build folder in Player.
                    string projectBase = Path.GetDirectoryName(Application.dataPath);
                    return Path.GetFullPath(Path.Combine(projectBase, path));

                case PdfPathMode.RelativeToStreamingAssets:
                    // Application.streamingAssetsPath:
                    //   Editor   -> "<projectRoot>/Assets/StreamingAssets"
                    //   Standalone Player -> "<buildFolder>/<gameName>_Data/StreamingAssets"
                    //   Android  -> path inside the APK (direct File IO will not work; use UnityWebRequest)
                    return Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, path));

                case PdfPathMode.Absolute:
                default:
                    return Path.GetFullPath(path);
            }
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
