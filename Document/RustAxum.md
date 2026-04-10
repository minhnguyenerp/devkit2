# 🚀 Getting Started with Rust + Axum using DevKit2

[Go Back](https://github.com/minhnguyenerp/devkit2/blob/main/README.md)

## 🧩 Step 1: Create a New Project

1. Open **DevKit2**
2. Click **New Project From Template**
3. Select the template: **RustGcc, VSCode** or **RustMsvc, VSCode**
4. Enter a project name (e.g., `Rust Axum Test`)
5. Choose the project location (create new folder if needed)
6. Click **OK**

![Create Go Gin Project](https://raw.githubusercontent.com/minhnguyenerp/devkit2/main/Document/Images/rust/rust-axum-new-project.png)

After creating the project:

* Go to **My Projects**
* Double-click your project to open it in **VSCode**

---

## ⚙️ Step 2: Initialize Rust project and install needed packages

In VSCode:

1. Open the Terminal:

   * Menu **Terminal → New Terminal**

2. Run the following commands:

```bash
cargo init
cargo add axum
cargo add tokio --features full
cargo add serde --features derive
cargo add serde_json
```

---

## 💻 Step 3: Write Code

Open file `src/main.rs` and add the following code:

```rust
use axum::{routing::get, Router, Json, serve};
use serde::Serialize;
use std::net::SocketAddr;

#[derive(Serialize)]
struct Message {
    message: String,
}

async fn hello_handler() -> Json<Message> {
    Json(Message {
        message: "Hello, Axum!".to_string(),
    })
}

#[tokio::main]
async fn main() {
    let app = Router::new().route("/", get(hello_handler));

    let addr = SocketAddr::from(([127, 0, 0, 1], 3000));
    println!("Axum listening on http://{}", addr);

    let listener = tokio::net::TcpListener::bind(addr).await.unwrap();
    serve(listener, app).await.unwrap();
}
```

👉 What this does:

* Starts a web server using Axum
* When accessing `/`, it returns a JSON response: `"Hello, Axum!"`

---

## ▶️ Step 4: Run the Application

In the Terminal, run:

```bash
cargo run
```

Then open your browser and go to:

```
http://localhost:3000/
```

👉 You should see:

```json
{"message":"Hello, Axum!"}
```

---

## 📦 Build the Executable (Release)

To build the application:

```bash
cargo build --release
```

---

## ✅ Summary

You have:

* Created a Rust project using DevKit2
* Installed the Axum framework
* Built a simple API
* Run and compiled the application

---

[Go Back](https://github.com/minhnguyenerp/devkit2/blob/main/README.md)