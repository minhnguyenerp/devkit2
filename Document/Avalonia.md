# 🧰 Getting Started with Avalonia using DevKit2

## 🔹 Step 1: Install .NET SDK

1. Open **DevKit2**
2. Go to the **Programs** tab
3. Locate **DotnetSDK**
4. Select the desired version
5. Click **Install**

![DotNet SDK](https://raw.githubusercontent.com/minhnguyenerp/devkit2/main/Document/Images/devkit2_install_dotnetsdk.png)

👉 After installation, the portable .NET SDK environment will be available.

---

## 🔹 Step 2: Launch the DotnetSDK Environment

1. Go to the **Manual Launch** tab
2. Double-click **DotnetSDK**

![Run DotNet SDK](https://raw.githubusercontent.com/minhnguyenerp/devkit2/main/Document/Images/devkit2_run_DotnetSDK.png)

👉 A **Command Prompt** window will open
👉 This environment is pre-configured with:

* `PATH`
* `DOTNET_ROOT`
* other necessary portable environment variables

---

## 🔹 Step 3: Install Avalonia Templates and create project

In the command prompt, run:

```bash
dotnet new install Avalonia.Templates
```

![Install Avalonia Template](https://raw.githubusercontent.com/minhnguyenerp/devkit2/main/Document/Images/install_avalonia_template.png)

👉 Purpose:

* Installs Avalonia project templates
* This step is required **only once**

⚠️ Note:

* If you see warnings like *"No project was found..."*, you can safely ignore them

After installing the templates, you can create a new project anytime using:

```bash
dotnet new avalonia.mvvm -o "C:\Projects\MyApp"
```

![Create mvvm](https://raw.githubusercontent.com/minhnguyenerp/devkit2/main/Document/Images/avalonia_create_mvvm.png)

Run the application

```bash
cd /d C:\Projects\MyApp
dotnet run
```

👉 The Avalonia application will launch

![Avalonia window](https://raw.githubusercontent.com/minhnguyenerp/devkit2/main/Document/Images/avalonia_window.png)

---

## 🔹 Step 4: Create a DevKit2 Project for VS Code

1. Go to the **My Project** tab in **DevKit2**
2. Click **New Project**
3. Set the following:

   * **Project Name**: `My App`
   * **Application**: `VSCode`
   * Select environments:
     * ✅ **DotnetSDK**
     * ✅ **Git**
4. Click **OK**

![Avalonia vscode](https://raw.githubusercontent.com/minhnguyenerp/devkit2/main/Document/Images/devkit2_project_avalonia_vscode.png)

Then configure the application profile:

* **Working Directory**: `C:\Projects\MyApp`
* **Startup File**: `C:\Projects\MyApp`

![Application profile](https://raw.githubusercontent.com/minhnguyenerp/devkit2/main/Document/Images/devkit2_project_avalonia_vscode_working_dir.png)

👉 After creating the project:

* Simply **double-click** the project in DevKit2
* VS Code will open your Avalonia project

---

## 🔹 Step 6: Run and Develop the Application

Inside **VS Code**:

* Open the integrated terminal
* Run:

```bash
dotnet build
dotnet run
```

👉 The Avalonia application will start

---

[Go Back](https://github.com/minhnguyenerp/devkit2/blob/main/README.md)