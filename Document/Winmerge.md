# DevKit2 - WinMerge Notes about Context Menu (Windows 11)

[Go Back](https://github.com/minhnguyenerp/devkit2/blob/main/README.md)

## Quick Setup via WinMerge UI (Recommended)

The easiest way to enable the Windows 11 context menu integration is directly from the WinMerge application:

1. Open **WinMerge**
2. Go to:

   ```
   Edit → Options → Shell Integration
   ```
3. Click:

   ```
   Register Shell Extension for Windows 11
   ```
4. Check:

   ```
   Add to context menu
   ```

![WinMerge Options](https://raw.githubusercontent.com/minhnguyenerp/devkit2/main/Document/Images/WinMergeOptions.png)

### Notes

* The **Register** button installs the MSIX-based context menu extension.
* The **Add to context menu** checkbox enables or disables the menu via internal configuration.
* Unchecking the box will immediately hide the context menu.

---

## Overview

WinMerge uses the **modern Windows 11 context menu system** based on **MSIX + packaged COM extension**, instead of the legacy `regsvr32` shell extension.

There are **two separate layers** involved:

1. **MSIX Package Registration**
2. **WinMerge Internal Enable Flag (Registry)**

Both must be correctly configured for the context menu to appear.

---

## 1. MSIX Context Menu Extension

WinMerge registers its Windows 11 context menu using:

```powershell
Add-AppxPackage WinMergeContextMenuPackage.msix -ExternalLocation "<WinMerge folder>"
```

### Key Components

* `WinMergeContextMenuPackage.msix` → registers extension with Windows
* `WinMergeContextMenu.dll` → handles context menu logic
* `WinMergeU.exe` → actual application

### Important

The MSIX package **does NOT control visibility of the menu**.
It only tells Windows:

> “Call this handler when user right-clicks.”

---

## 2. Context Menu Enable Flag (Registry)

The actual visibility of the menu is controlled by WinMerge itself.

### Registry Location

```text
HKEY_CURRENT_USER\Software\Thingamahoochie\WinMerge
```

### Key

```text
ContextMenuEnabled (DWORD)
```

### Values

| Value | Meaning               |
| ----- | --------------------- |
| `1`   | Context menu enabled  |
| `0`   | Context menu disabled |

---

## 3. How It Works Internally

When you right-click in Explorer:

1. Windows calls the registered handler (`WinMergeContextMenu.dll`)
2. The DLL reads:

```text
HKCU\Software\Thingamahoochie\WinMerge\ContextMenuEnabled
```

3. If the value is:

* `1` → menu is shown
* `0` → menu is hidden

---

## 4. Enable / Disable Context Menu

### Enable

```bat
reg add "HKCU\Software\Thingamahoochie\WinMerge" ^
 /v ContextMenuEnabled ^
 /t REG_DWORD ^
 /d 1 ^
 /f
```

### Disable

```bat
reg add "HKCU\Software\Thingamahoochie\WinMerge" ^
 /v ContextMenuEnabled ^
 /t REG_DWORD ^
 /d 0 ^
 /f
```

---

## 5. Full Setup (Manual)

To fully enable WinMerge context menu:

### Step 1 — Install MSIX Extension

```powershell
Add-AppxPackage "WinMergeContextMenuPackage.msix" -ExternalLocation "<WinMerge folder>"
```

### Step 2 — Enable via Registry

```bat
reg add "HKCU\Software\Thingamahoochie\WinMerge" /v ContextMenuEnabled /t REG_DWORD /d 1 /f
```

### Step 3 — Restart Explorer

```powershell
taskkill /f /im explorer.exe
start explorer.exe
```

---

## 6. Common Issues

### Context menu does not appear

Possible causes:

* `ContextMenuEnabled = 0`
* MSIX not installed
* Wrong `ExternalLocation`
* Missing `WinMergeContextMenu.dll`
* Explorer not restarted

---

### After updating WinMerge

If you install a new version in a **different folder**, the old MSIX may still point to the old location.

Fix:

```powershell
Get-AppxPackage WinMerge | Remove-AppxPackage
Add-AppxPackage "WinMergeContextMenuPackage.msix" -ExternalLocation "<new path>"
```

---

## 7. Key Insight

> MSIX registers the extension
> Registry controls whether it is shown

Both are required.

---

## 8. Recommendation

For stability:

* Use a **fixed installation path** (e.g. `C:\Apps\WinMerge`)
* Avoid versioned folders for MSIX external location
* Always keep:

  * `WinMergeU.exe`
  * `WinMergeContextMenu.dll`
    in the same directory

---

## 9. Summary

| Component                       | Responsibility                      |
| ------------------------------- | ----------------------------------- |
| MSIX package                    | Registers context menu with Windows |
| DLL (`WinMergeContextMenu.dll`) | Implements menu logic               |
| Registry (`ContextMenuEnabled`) | Enables / disables menu             |

---

If you are building your own application:

> This pattern (MSIX + registry toggle) is a good reference design for Windows 11 context menu integration.

---

[Go Back](https://github.com/minhnguyenerp/devkit2/blob/main/README.md)