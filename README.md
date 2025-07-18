# 💬 CorePunk Chat System

A modular, event-driven client-server chat application built with **.NET 8**, featuring **UDP/TCP support**, **Rx.NET observables**, and clean architectural principles (including **SOLID** and **interface-driven design**).

This project is designed to showcase real-world software engineering skills in areas such as networking, multithreading, configurability, and extensibility.

---

## 📦 Features

- ✅ Configurable communication layer (UDP or TCP via config or environment)
- ✅ Rx.NET-based observables for real-time message/event streaming
- ✅ CancellationToken support for safe async operations and shutdown
- ✅ Interfaces for communication (`ICommunicator`, `IClient`, etc.)
- ✅ `CommunicatorFactory` for dynamic instantiation via reflection
- ✅ Testable core modules with separation of concerns
- ✅ Optional Docker support and cross-platform readiness

---

## 🧱 Architecture Overview

```text
                   +------------------------+
                   |     ServerApp          |
                   |------------------------|
                   | - ServerAppInitializer |
                   | - ChatServer           |
                   +------------------------+
                           |
                           v
               +------------------------+
               |  ICommunicator (UDP/TCP) |
               +------------------------+
                           ^
                           |
                   +------------------------+
                   |     ClientApp          |
                   |------------------------|
                   | - ClientAppInitializer |
                   | - ChatClient           |
                   +------------------------+
```

Shared Libraries:
- CoreLibrary.Interfaces (`IClient`, `ICommunicator`, `IMessageProcessor`)
- CoreLibrary.Messaging (`Message`, `MessageTypes`)
- CoreLibrary.Communication (`UdpCommunicator`, `TcpCommunicator`)

---

## ⚙️ How to Run

🖥️ Local (Visual Studio or CLI)

Build the solution.

Run ServerApp first.

Run one or more instances of ClientApp.

Enter a username and start chatting.

## 🐳 Docker (Planned)
🛠️ Docker support is in progress.

Planned structure:
---
docker-compose up --build

Will run both ServerApp and one or more clients in separate containers.

📁 Folder Structure
```text
CoreLibrary/
  └─ Communication/          # UdpCommunicator, TcpCommunicator
  └─ Handlers/               # ClientHandler, UserManager
  └─ Interfaces/             # IClient, ICommunicator, IMessageProcessor, etc.
  └─ Messaging/              # Message types and helpers
  └─ Utilities/              # LoggingService, AppLock, Config, Auth

ClientApp/
  └─ ClientAppInitializer.cs
  └─ ChatClient.cs
  └─ InputHandler.cs
  └─ OutputHandler.cs

ServerApp/
  └─ ServerAppInitializer.cs
  └─ ChatServer.cs

Factories/
  └─ CommunicatorFactory.cs
  ```
## 🧪 Testing (Planned)
 Unit tests for ClientHandler, Message, AppLock, UdpReceiver

 Integration test for ChatServer and broadcast handling

 Mocked communicator for client testing

## 🔄 TODO & Improvements
 Finish TCP implementation

 Add unit tests to CoreLibrary

 Add XML documentation across interfaces

 Finalize Docker + Docker Compose support

 Include WebSocket support (optional extension)

 Add UI front-end (optional)

## 📜 License
MIT — free to use, share, and modify.