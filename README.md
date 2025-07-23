[![.NET CI](https://github.com/TitoAko/CorePunkChat/actions/workflows/dotnet.yml/badge.svg)](https://github.com/TitoAko/CorePunkChat/actions/workflows/dotnet.yml)
# 💬 CorePunk Chat System — 2025 edition

A modular, **.NET 8** client–server chat written to demonstrate modern
networking, async/await, test-driven refactor and clean design.

---

## ✨ Highlights

| Area | What you’ll find |
|------|------------------|
| **Pluggable transport** | UDP implemented, TCP stub in place – pick via `Configuration.TransportKind`. |
| **Async-first API** | Single `ICommunicator` interface (`StartAsync`, `SendMessageAsync`, `MessageReceived`). |
| **Clean DTO model** | One `Message` class + `MessageType` enum → no runtime reflection. |
| **Enum-switch factory** | `CommunicatorFactory` chooses transport without reflection overhead. |
| **Graceful shutdown** | Everywhere uses `CancellationToken` + `Dispose`. |
| **Solid tests** | 11 passing xUnit tests covering serialization, factory, utilities and UDP e2e. |
| **Docker-ready** | Multi-stage Dockerfiles (build ➜ runtime) for client & server. |
| **Extensible** | Add WebSockets/SignalR or a GUI without touching core logic. |

---

## 🏗️ Architecture (high level)

```text
          +------------------------+
          |     ServerApp          |
          |------------------------|
          | ServerAppInitializer   |
          | ChatServer             |--+
          +------------------------+  |
                   ▲                  |
                   | Event            |
          +------------------------+  | 1..*
          |   ClientConnection     |--+
          +------------------------+
                   ▲    ▲
                   |    | ICommunicator (UDP/TCP)
+------------------+    |
|     ClientApp     |   |
|-------------------|   |
| ChatClient        |---+
| Input / Output    |
+-------------------+
```

### Shared library outline

```
CoreLibrary
 ├─ Communication/
 │    ├─ UdpCommunication/
 │    │     ├─ UdpSender / UdpReceiver / UdpCommunicator
 │    └─ TcpCommunicator   (minimal stub; TODO)
 │
 ├─ Messaging/    Message, MessageType, Json converter
 ├─ Utilities/    Configuration, Port helpers, etc.
 ├─ Factories/    CommunicatorFactory
 └─ IO/           InputHandler, OutputHandler
```

Legacy folders (`Interfaces/`, `MessageTypes/`, reflection factory) have
been archived.

---

## ⏯️ Running locally

```bash
# 1  build everything
dotnet build

# 2  start server (default UDP 9000)
dotnet run --project ServerApp

# 3  in another terminal start a client
dotnet run --project ClientApp 127.0.0.1 9000
```

`ClientId` is random by default; pass `--name Alice` if you prefer.

---

## 🐳 Docker (quick demo)

```bash
docker compose up --build
# => server binds UDP 9000, two clients auto-connect and exchange chat lines
# Stop with Ctrl-C
```

```bash
docker compose up --build
```
---

## 🧪 Testing

*Project:* **CoreLibrary.Tests**  
*Framework:* xUnit + Coverlet

| Bucket | Tests |
|--------|-------|
| Utilities | `Configuration.IsEndpointBusy`, unique `ClientId` |
| Messaging | JSON round-trip, enum converter |
| Factory | Correct transport chosen, throws on bad enum |
| UDP Integration | single chat, exit, order preservation |

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Coverage target: **≥ 70 %** (badge script in CI).

---

## 🚧 Roadmap

| Status | Item                                 | Notes (next up)                    |
|--------|--------------------------------------|------------------------------------|
| ✅     | UDP transport (sender / receiver)    | End-to-end tested                  |
| ✅     | **TCP transport parity**             | Multi-container demo, CI passing   |
| ⬜     | Raise test coverage > 80 %           | Add handler edge-cases & TCP tests |
| ⬜     | Docker -compose healthchecks         | Auto-restart on failure            |
| ⬜     | Optional WebSocket gateway           | Stretch goal                       |


---

## 📝 License

MIT – free to use, modify, and share.
