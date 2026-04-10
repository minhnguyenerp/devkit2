# 🧰 Getting Started with Avalonia using DevKit2

[Go Back](https://github.com/minhnguyenerp/devkit2/blob/main/README.md)

## 🧩 Step 1: Create a New Project

1. Open **DevKit2**
2. Click **New Project From Template**
3. Select the template: **Avalonia, VSCode**
4. Enter a project name (e.g., `Avalonia Test`)
5. Choose the project location (create new folder if needed)
6. Click **OK**

After creating the project:

* Go to **My Projects**
* Double-click your project to open it in **VSCode**

---

## ⚙️ Step 2: Initialize Avalonia project and install needed packages

In VSCode:

1. Open the Terminal:

   * Menu **Terminal → New Terminal**

2. Run the following commands:

```bash
dotnet new install Avalonia.Templates
dotnet new avalonia.mvvm
```
And Avalonia created and happy Avalonia coding.

---

## ▶️ Step 3: Run the Application

In the Terminal, run:

```bash
dotnet run
```

👉 You should see the avalonia window showing up

---

## 📦 Build the Executable (Release)

To build the application:

```bash
dotnet build -c Release
```

---

## ✅ Summary

You have:

* Created an Avalonia project using DevKit2
* Run and compiled the application

---

```bash
Note 04/10/2026:
warning NU1903: Package 'Tmds.DBus.Protocol' 0.90.3 has a known high severity vulnerability, https://github.com/advisories/GHSA-xrw6-gwf8-vvr9
dotnet nuget why Tmds.DBus.Protocol
  [net10.0]
  └── Avalonia.Desktop (v12.0.0)
      └── Avalonia.X11 (v12.0.0)
          └── Avalonia.FreeDesktop (v12.0.0)
              └── Tmds.DBus.Protocol (v0.90.3)

Add <PackageReference Include="Tmds.DBus.Protocol" Version="0.92.0" /> to the .csproj file
dotnet nuget why Tmds.DBus.Protocol
  [net10.0]
  ├── Avalonia.Desktop (v12.0.0)
  │   └── Avalonia.X11 (v12.0.0)
  │       └── Avalonia.FreeDesktop (v12.0.0)
  │           └── Tmds.DBus.Protocol (v0.92.0)
  └── Tmds.DBus.Protocol (v0.92.0)
```

---

[Go Back](https://github.com/minhnguyenerp/devkit2/blob/main/README.md)
