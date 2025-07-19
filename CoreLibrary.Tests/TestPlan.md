# ✅ CoreLibrary Test Coverage Plan

This document tracks unit and integration test coverage goals for the networking chat system.

---

## 🎯 Target Coverage: ~80% of core logic

Focus is placed on:
- Observable event correctness
- Lifecycle integrity (e.g., UdpClient not disposed unexpectedly)
- Key logical units (AppLock, handlers, communicators)

---

## ✅ Progress Table

| ✅ | Module Name           | Target Class        | Key Tests                                                           | Status       |
|----|------------------------|----------------------|----------------------------------------------------------------------|--------------|
| ✅ | Client Events          | `ClientHandler`      | Event triggers: `OnConnect`, `OnDisconnect`, `OnMessageReceived`    | ✅ Done         |
| ✅ | AppLock Check          | `AppLock`            | Prevents multiple instances on same config                          | ✅ Done         |
| ✅ | Input Parsing          | `InputHandler`       | Interprets raw console input into commands or messages              | 🚧 Needs Review |
| ✅ | Packet Trigger         | `UdpReceiver`        | StartObservables, deserialization, and observable emission          | ✅ Done         |
| ⬜ | Disposal Guard (UDP)   | `UdpCommunicator`    | Ensure client not disposed during/after send/receive                | 🚧 Planned       |
| ⬜ | Config Loader          | `Configuration`      | Loads from file, fallback to defaults                               | Optional          |
| ⬜ | Message Format         | `Message`            | Structure, sender, message type correctness                         | Optional          |

---

## 📘 Notes

- Edge cases (e.g., disposed UdpClient) may require integration-style tests with mocked behavior or exposed state.
- Coverage % is not tracked via tooling for now — we aim for practical and visible test verification.
- `TcpClient` will follow same structure once its support is implemented.

---

## 🔄 Last Updated
`{{ 18/07/2025 }}`
