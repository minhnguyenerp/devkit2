# 🚀 Getting Started with Svelte using DevKit2

[← Go Back](https://github.com/minhnguyenerp/devkit2/blob/main/README.md)

## 🧩 Step 1: Create a New Project

1. Open **DevKit2**
2. Click **New Project From Template**
3. Select the template: **NodeJs, VSCode**
4. Enter a project name (e.g., `Svelte Test`)
5. Choose the project location (create a new folder if needed)
6. Click **OK**

After the project is created:

* Go to **My Projects**
* Double-click your project to open it in **VSCode**

---

## ⚙️ Step 2: Initialize Svelte & Install Dependencies

In VSCode:

1. Open the Terminal:

   * Go to **Terminal → New Terminal**

2. Run the following commands:

```bash
npx.cmd sv create
```

```bash
PS C:\Projects\Experiments\SvelteTest> npx.cmd sv create
Need to install the following packages:
sv@0.15.1
Ok to proceed? (y)

HINT: Run "sv --help" to get the full list of commands, add-ons, and examples to one-shot and skip interactive prompts.
┌  Welcome to the Svelte CLI! (v0.15.1)
│
◇  Where would you like your project to be created?
│  ./
│
◇  Which template would you like?
│  SvelteKit minimal
│
◇  Add type checking with TypeScript?
│  Yes, using TypeScript syntax
│
◇  What would you like to add to your project? (use arrow keys / space bar)
│  prettier
│
◆  Project created
│
◆  Successfully setup add-ons: prettier
│
◇  Which package manager do you want to install dependencies with?
│  npm
│
│  To skip prompts next time, run:
●  npx sv@0.15.1 create --template minimal --types ts --add prettier --install npm ./
│
◆  Successfully installed dependencies with npm
│
◇  What's next? ───────────────────────────────╮
│                                              │
│  📁 Project steps                            │
│                                              │
│    1: npm run dev -- --open                  │
│    1: npm run dev -- --open                  │
│                                              │
│  To close the dev server, hit Ctrl-C         │
│                                              │
│  Stuck? Visit us at https://svelte.dev/chat  │
│                                              │
├──────────────────────────────────────────────╯
│
└  You're all set!

npm notice
npm notice New minor version of npm available! 11.11.0 -> 11.12.1
npm notice Changelog: https://github.com/npm/cli/releases/tag/v11.12.1
npm notice To update run: npm install -g npm@11.12.1
npm notice
PS C:\Projects\Experiments\SvelteTest>
```

```bash
npm.cmd install
```

```bash
npm.cmd run dev
```

```bash
PS C:\Projects\Experiments\SvelteTest> npm.cmd run dev

> sveltetest@0.0.1 dev
> vite dev

4:11:37 PM [vite] (client) Forced re-optimization of dependencies

  VITE v8.0.8  ready in 8002 ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
  ➜  press h + enter to show help
```

---

## 🎉 You're Ready!

Your Svelte project is now set up and running. Open you browser and navigate to `http://localhost:5173` to see the result

👉 You can start coding and exploring Svelte right away!

---

[← Go Back](https://github.com/minhnguyenerp/devkit2/blob/main/README.md)
