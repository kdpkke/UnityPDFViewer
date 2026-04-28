using UnityEngine;
using UnityPdfViewer;

public class LevelManagerPDFViewerSample : MonoBehaviour
{
    [Tooltip(
        "PdfViewerUI in the scene that will display the PDF.\n" +
        "Configure the PDF path and Path Mode directly on the PdfViewerUI component — " +
        "this script just triggers the load on Start().")]
    public PdfViewerUI pdfInScene;

    void Start()
    {
        if (pdfInScene)
            pdfInScene.LoadPDF(); // uses pdfPath + pathMode configured on PdfViewerUI
    }
}
