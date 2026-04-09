# 🚀 Getting Started with Go + Gin using DevKit2

[Go Back](https://github.com/minhnguyenerp/devkit2/blob/main/README.md)

## 🧩 Step 1: Create a New Project

1. Open **DevKit2**
2. Click **New Project From Template**
3. Select the template: **Golang, VSCode**
4. Enter a project name (e.g., `Getting Started Go Gin`)
5. Choose the project location
6. Click **OK**

![Create Go Gin Project](https://raw.githubusercontent.com/minhnguyenerp/devkit2/main/Document/Images/go-gin-new-project.png)

After creating the project:

* Go to **My Projects**
* Double-click your project to open it in **VSCode**

---

## ⚙️ Step 2: Initialize Go Module and Install Gin

In VSCode:

1. Open the Terminal:

   * Menu **Terminal → New Terminal**

2. Run the following commands:

```bash
go mod init gogin
go get github.com/gin-gonic/gin
```

![Init Go Gin Project](https://raw.githubusercontent.com/minhnguyenerp/devkit2/main/Document/Images/go-gin-install-package.png)

👉 Explanation:

* `go mod init`: initializes a Go module
* `go get`: installs the Gin framework

---

## 💻 Step 3: Write Code

Create a file named `main.go` and add the following code:

```go
package main

import (
    "github.com/gin-gonic/gin"
    "net/http"
)

func main() {
    r := gin.Default()

    r.GET("/", func(c *gin.Context) {
        c.JSON(http.StatusOK, gin.H{
            "message": "Hello, Gin!",
        })
    })

    r.Run(":3000")
}
```

👉 What this does:

* Starts a web server using Gin
* When accessing `/`, it returns a JSON response: `"Hello, Gin!"`

---

## ▶️ Step 4: Run the Application

In the Terminal, run:

```bash
go run .
```

Then open your browser and go to:

```
http://localhost:3000/
```

👉 You should see:

```json
{"message":"Hello, Gin!"}
```

---

## 📦 Build the Executable (Release)

To build the application:

```bash
go build -ldflags "-s -w" -o bin/gogin.exe
```

👉 Explanation:

* `-ldflags "-s -w"`: reduces binary size
* `-o`: specifies output file name

---

## ✅ Summary

You have:

* Created a Go project using DevKit2
* Installed the Gin framework
* Built a simple API
* Run and compiled the application

---

[Go Back](https://github.com/minhnguyenerp/devkit2/blob/main/README.md)