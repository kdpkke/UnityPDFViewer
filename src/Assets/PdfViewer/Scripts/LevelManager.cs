using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPdfViewer;

public class LevelManager : MonoBehaviour
{
    public PdfViewerUI pdfInScene;
    public string pdfAbsoluteFullPath = "SamplePDF.pdf";
    // Start is called before the first frame update
    void Start()
    {
        if (pdfInScene)
            pdfInScene.LoadPDF(pdfAbsoluteFullPath);
    }
}
