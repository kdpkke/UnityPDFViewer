using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private static ServiceLocator _instance;
    
    private PdfViewerUI _pdfViewerUI;

    public static ServiceLocator Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject serviceLocator = new GameObject("ServiceLocator");
                _instance = serviceLocator.AddComponent<ServiceLocator>();
                DontDestroyOnLoad(serviceLocator);
            }
            return _instance;
        }
    }

    public PdfViewerUI PdfViewerUI
    {
        get
        {
            if (_pdfViewerUI == null)
            {
                _pdfViewerUI = FindObjectOfType<PdfViewerUI>();
            }
            return _pdfViewerUI;
        }
    }
}