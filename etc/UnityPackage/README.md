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

1. In your scene, create a **GameObject** for each PDF file you want to display.
2. Attach the **`PdfViewerUI`** component to the GameObject.

### Mandatory setup
- **Raw Image**: assign a `RawImage` UI element where the PDF pages will be rendered.
- **Pdf Path** + **Path Mode**: configure where your PDF lives directly in the Inspector. See [Path resolution](#path-resolution) below for the available modes and what string to write.

### Optional setup
- **Navigation buttons**: assign `Next` and `Previous` `Button` components to allow page navigation.
- **Page indicator**: assign a `TextMeshProUGUI` text component. It will automatically display the format: `currentPage / totalPages` as the user browses the PDF.
- **Render DPI**: a slider (72–300) that controls the resolution at which each page is rasterized.

---

## Path resolution

Starting from `1.0.7`, the PDF path is configured **on the `PdfViewerUI` component itself** through two Inspector fields:

- **`Path Mode`** (enum `PdfPathMode`) — how the string is interpreted.
- **`Pdf Path`** (string) — the path itself, written according to the selected mode.

| Path Mode | Base used to resolve the path | What to write in `Pdf Path` |
|---|---|---|
| `Absolute` | none — the string is the full disk path | `"C:/Docs/manual.pdf"`, `"E:/Builds/Game/manual.pdf"` |
| `RelativeToProjectFolder` | project root in Editor / folder containing the `.exe` in a Player build | `"Docs/manual.pdf"` (resolves to `<projectRoot>/Docs/manual.pdf` in Editor or `<buildFolder>/Docs/manual.pdf` in build) |
| `RelativeToStreamingAssets` | `Application.streamingAssetsPath` | `"presence.pdf"` or `"manuals/presence.pdf"` (resolves to `<projectRoot>/Assets/StreamingAssets/...` in Editor or `<buildFolder>/<gameName>_Data/StreamingAssets/...` in standalone build) |

**Recommended for PDFs shipped with the game**: drop them into `Assets/StreamingAssets/` and use `RelativeToStreamingAssets`. Unity copies the folder next to the build automatically and the same `Pdf Path` works in Editor and in the standalone build.

> **Android note**: `Application.streamingAssetsPath` points inside the APK on Android, so direct file IO won't work. Use `UnityWebRequest` to copy the file to a writable location first and then load it via `Absolute` mode.

---

## Loading the PDF

`PdfViewerUI` exposes three overloads of `LoadPDF`:

```csharp
public void LoadPDF();                              // uses pdfPath + pathMode from the Inspector
public void LoadPDF(string path);                   // overrides pdfPath, uses pathMode from the Inspector
public void LoadPDF(string path, PdfPathMode mode); // overrides everything for this single call
```

### Example — load from StreamingAssets
1. Create a `Canvas` with a `RawImage`.
2. Create an empty `GameObject` named `PdfViewer` and attach `PdfViewerUI` to it.
3. Drag the `RawImage` into the **Raw Image** field of the component.
4. (Optional) Hook up `Next` / `Previous` buttons and a `TextMeshProUGUI` page indicator.
5. Set **Path Mode** = `RelativeToStreamingAssets` and **Pdf Path** = `presence.pdf`.
6. Drop your PDF in `Assets/StreamingAssets/presence.pdf`.
7. Trigger the load from any script:

```csharp
using UnityPdfViewer;

PdfViewerUI viewer = GetComponent<PdfViewerUI>();

// Uses the Pdf Path + Path Mode you set in the Inspector
viewer.LoadPDF();

// Or override at runtime
viewer.LoadPDF("manuals/intro.pdf");                                // same Path Mode
viewer.LoadPDF("C:/Docs/external.pdf", PdfPathMode.Absolute);       // override mode

// Page navigation
viewer.NextPage();
viewer.PreviousPage();
viewer.GoToPage(3);
```

Press play → the PDF is loaded and displayed in the UI.

---

## Sample scene

A ready-to-use sample is included in this package.

In the **Package Manager** window, select **Unity PDF Viewer** → open the **Samples** tab → click **Import** next to *Sample Scene*. Unity will copy a demo scene with a `PdfViewerUI` already wired up and a small `LevelManagerPDFViewerSample` script that calls `pdfInScene.LoadPDF()` on `Start()`. The sample ships with two example PDFs so you can try `RelativeToStreamingAssets` and `Absolute` modes immediately.

---

## License
This project is licensed under the MIT License – see the [LICENSE](https://github.com/kdpkke/UnityPDFViewer/blob/main/LICENSE) file for details.
