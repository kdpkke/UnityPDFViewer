# Unity PDF Viewer

[![License: MIT](https://img.shields.io/badge/license-MIT-blue?style=flat-square)](LICENSE)

A Unity library to render [PDF files](https://en.wikipedia.org/wiki/PDF) inside your Unity scene.  

This Unity library is built on top of:
* [PDFtoImage](https://github.com/sungaila/PDFtoImage)  

---

## Getting started

1. Place your **PDF file(s)** inside the Unity `StreamingAssets` folder.  
2. In your scene, create a **GameObject** for each PDF file you want to display.  
3. Attach the **`PdfViewerUI`** component to the GameObject.  

### Mandatory setup
- **RawImage**: assign a `RawImage` UI element where the PDF pages will be rendered.  
- **PDF File Name**: set the name of your file (e.g. `document.pdf`) in the `PDF File Name` property of the component.  

### Optional setup
- **Navigation buttons**: assign `Next` and `Previous` `Button` components to allow page navigation.  
- **Page indicator**: assign a `TextMeshProUGUI` text component. It will automatically display the format: `currentPage / totalPages` as the user browses the PDF.  

---

## Example

1. Create a `Canvas` with a `RawImage`.  
2. Create an empty `GameObject` named `PdfViewer`.  
3. Attach `PdfViewerUI` to `PdfViewer`.  
4. Drag the `RawImage` from the Canvas into the **Raw Image** field of the component.  
5. Set **PDF File Name** to `document.pdf`.  
6. (Optional) Add navigation buttons and link them to the component fields
7. If you want to call the available public methods in code:  

 ```csharp
 pdfviewerUI = GetComponent<PDFViewerUI>();
 // Then you can use:
 pdfviewerUI.NextPage();
 pdfviewerUI.PreviousPage();
 pdfviewerUI.GoToPage(3); // Go to page 3
 ```
 
Press play → the PDF is loaded and displayed in the UI.

---

### Unity project installation
1. Open your project and navigate to `Window` → `Package Manager`.
2. Click on the `+` button (top-left corner) and select `Install package from git URL...`.
3. Enter the following URL and confirm with the `Install` button:
```
https://github.com/kdpkke/UnityPDFViewer.git?path=etc/UnityPackage
```

## License
This project is licensed under the MIT License – see the [LICENSE](https://github.com/kdpkke/UnityPDFViewer/blob/main/LICENSE) file for details.

