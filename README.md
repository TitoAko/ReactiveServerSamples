[![.NET CI](https://github.com/TitoAko/ReactiveServerSamples/actions/workflows/dotnet.yml/badge.svg)](https://github.com/TitoAko/ReactiveServerSamples/actions/workflows/dotnet.yml)

# 💬 CorePunk Chat / Server Samples — 2025 Edition

**Author:** Emil Pirkl  
**GitHub:** <https://github.com/TitoAko/ReactiveServerSamples>

A modular **.NET 8** client–server chat and MMO backend playground designed to showcase modern networking, async/await, test-driven refactor, clean architecture and Docker workflow.

---

## 📋 Table of Contents
1. 📜 [Project Overview](#project-overview)  
2. ✨ [Highlights](#highlights)  
3. 🧱 [Architecture](#architecture)  
4. 🛠️ [Technology Stack](#technology-stack)  
5. 🚀 [Getting Started](#getting-started)  
   • 💻 [CLI flow](#cli-flow)  
   • 🐳 [Docker Compose demo](#docker-compose-demo)  
6. 🧪 [Testing & Coverage](#testing--coverage)  
7. 🛣️ [Roadmap](#roadmap)  
8. 🤝 [Contributing](#contributing)  
9. 📜 [License](#license)  

---

## 📜 Project Overview
This repo **will** contain three proof-of-concept sub-systems:

| Module | Status | What it does |
|--------|--------|--------------|
| **UDP / TCP Chat** | ✅ implemented | Minimal real-time chat with pluggable transport, lazy connect, graceful shutdown. |
| **Distributed Store Prototype** | ⏳ *planned* | CRUD console app against DynamoDB **or** Azure Cosmos DB (free tier). |
| **Payment Flow Design** | ⏳ *planned* | Markdown + diagrams outlining idempotent Stripe integration. |

---

## ✨ Highlights

| Feture / Aspect | Benefit |
|------|--------------------------|
| **Pluggable transport** | `ICommunicator` switched by enum → UDP *and* TCP fully implemented & integration-tested. |
| **Async-first API** | `StartAsync`, `SendMessageAsync`, `MessageReceived` events with CancellationToken everywhere. |
| **Clean DTO** | Single `Message` record + `IMessageType` converter (no reflection). |
| **CI / Coverage** | GitHub Actions build + test badge, Coverlet collector (currently ~72 %). |
| **Docker-ready** | Multi-stage Dockerfiles; `docker compose up --build` spins server + 2 clients automatically. |
| **Front-end ready** | Same ICommunicator API can back a WPF MVVM UI, a Blazor component, or a React/SignalR client without touching CoreLibrary. |
| **Extensible** | Roadmap items for WebSockets, health-checks, UI front-end. |

---

## 🧱 Architecture

```text
              +------------------------+
              |      ServerApp         |
              |------------------------|
              | ServerAppInitializer   |
              | ChatServer             |--+ 1..*
              +------------------------+  |  ClientConnection
                       ▲                  |  (model)
                       | Event            |
        +--------------+------------------+
        |  ICommunicator   (UDP / TCP)    |
        +--------------+------------------+
                       ▲
              +------------------------+
              |      ClientApp         |
              |------------------------|
              | ChatClient             |
              | Input / Output         |
              +------------------------+
```
Shared library layout
```mathematica
CoreLibrary
 ├─ Communication/
 │    ├─ UdpCommunication/
 │    │     ├─ UdpSender / UdpReceiver / UdpCommunicator
 │    └─ TcpCommunication/
 │          ├─ TcpSender / TcpReceiver / TcpCommunicator
 ├─ Messaging/    Message, IMessageType, TextMessage, converter
 ├─ Utilities/    Configuration, AppLock, Port helpers
 ├─ Factories/    CommunicatorFactory
 └─ IO/           InputHandler, OutputHandler
```
## 🛠️ Technology Stack

| Layer / Concern      | Tech                                                  |
|----------------------|-------------------------------------------------------|
| Language / Runtime   | **C# 10**, **.NET 8**                                 |
| Networking           | `System.Net.Sockets` (UDP & TCP)                      |
| Multithreading       | `async/await`, `CancellationToken` everywhere         |
| Tests & Coverage     | xUnit, FluentAssertions, Coverlet                     |
| Containers           | Docker Desktop, multi-stage Dockerfile, Compose       |
| CI / Coverage badge  | GitHub Actions (`.github/workflows/dotnet.yml`)       |

---

## 🚀 Getting Started
### 💻 CLI flow
```bash
# 1  build everything
dotnet build

# 2  start server (default UDP:9000)
dotnet run --project ServerApp                # binds 0.0.0.0:9000

# 3  start a client
dotnet run --project ClientApp 127.0.0.1 9000
# (add --name Alice if you want a deterministic nickname)
```
### 🐳 Docker Compose demo
```bash
docker compose up --build      # one server + two clients
# Logs show [Server], [chat-client-1], [chat-client-2]
# Ctrl-C stops and cleans up
```
## 🧪 Testing & Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

| Bucket            | Key tests                                        |
|-------------------|--------------------------------------------------|
| **Utilities**     | Configuration unique IDs, AppLock                |
| **Messaging**     | JSON round-trip, type converter                  |
| **Factory**       | Transport selection, guard clauses               |
| **UDP integration** | single-chat, exit, order-preservation           |
| **TCP integration** | round-trip chat, dispose smoke-test             |

Target coverage: **80 %**
```
```
&nbsp;(CI badge auto-updates)
```
```
## 🛣️ Roadmap

| Status | Item                                   | Notes (next)                  |
|:------:|----------------------------------------|-------------------------------|
| ✅     | UDP & TCP transport parity             | Integration-tested            |
| ⬜     | Distributed Store Prototype            | DynamoDB / Cosmos CRUD        |
| ⬜     | Payment Flow Design doc                | Stripe idempotency, webhooks  |
| ✅     | Coverage → **80 %+**                   | edge-case unit tests          |
| ⬜     | Docker **health-checks**               | `healthcheck:` blocks         |
| ⬜     | Centralised **ILogger** pipeline       | console + JSON sink           |
| ⬜     | WebSocket / SignalR gateway _(stretch)_ | optional front-end hook       |

```
```
<details>
<summary>🔮 Future ideas (post-MVP)</summary>

| Idea                                 | Rationale / value-add                 |
|--------------------------------------|---------------------------------------|
| **Observability** via Prometheus     | scrape UDP/TCP packet/sec, GC stats   |
| **gRPC micro-service demo**          | show modern binary RPC vs. REST       |
| **Kubernetes Helm chart**            | production-grade deployment example   |
| **Auth** with JWT & IdentityServer   | secure endpoints for real accounts    |
| **Front-end samples**  (XAML, WinForms, React/TS) | prove UI layers can swap in via same API|
</details>

---


## 🤝 Contributing
Pull requests are welcome — open an issue first to discuss large change-sets.

## 📜 License
MIT — free to use, modify, and share.

