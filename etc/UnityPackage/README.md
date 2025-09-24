# Unity PDF Viewer

[![License: MIT](https://img.shields.io/badge/license-MIT-blue?style=flat-square)](LICENSE)

A Unity library to render [PDF files](https://en.wikipedia.org/wiki/PDF) inside your Unity scene.  

This Unity library is built on top of:
* [PDFtoImage](https://github.com/sungaila/PDFtoImage)  

---

## ☕ Like this project?

This project is developed and maintained in my spare time. If it has saved you time or helped you build something cool, please consider buying me a coffee to say thanks!

Your support keeps me caffeinated and motivated to work on new features and bug fixes.

<a href="https://paypal.me/matteomuratore1">
  <img src="https://img.shields.io/badge/PayPal-00457C?style=for-the-badge&logo=paypal&logoColor=white" alt="Donate with PayPal" />
</a>

---

## Getting started

1. Place your **PDF file(s)** inside the Unity `StreamingAssets` folder.  
2. In your scene, create a **GameObject** for each PDF file you want to display.  
3. Attach the **`PdfViewerUI`** component to the GameObject.  

### Mandatory setup
- **RawImage**: assign a `RawImage` UI element where the PDF pages will be rendered.  
- **PDF File Name**: set the name of your PDF file (e.g. `document.pdf`) when calling the `LoadPDF()` method (it will take the correct path from the StreamingAssets folder automatically).

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
 pdfViewerUI = GetComponent<PDFViewerUI>();
 pdfViewerUI.LoadPDF(pdfFileName);
 
 // Then you can use:
 pdfViewerUI.NextPage();
 pdfViewerUI.PreviousPage();
 pdfViewerUI.GoToPage(3); // Go to page 3
 ```
 
Press play → the PDF is loaded and displayed in the UI.

---

## License
This project is licensed under the MIT License – see the [LICENSE](https://github.com/kdpkke/UnityPDFViewer/blob/main/LICENSE) file for details.

