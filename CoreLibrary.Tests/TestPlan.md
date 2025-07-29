# ✅ CoreLibrary Test Coverage Plan

This document tracks **unit** and **integration** test coverage goals for the ReactiveServerSamples networking chat system.

---

## 🎯 Target Coverage — ≥ 80 % of core logic

Focus areas ⁠(highest ROI for reliability):

- Observable-event correctness (`MessageReceived`, handler events)
- Lifecycle integrity (e.g. communicator Dispose ↔ socket state)
- Guard clauses on size / emptiness / cancellation edge cases
- App-level single-instance enforcement (`AppLock`)

Current **coverage** (Coverlet): **≈ 82 %** → tests crossed 80 %.

---

## ✅ Progress Table

| ✅ | Module&nbsp;Area           | Target Class / Helper | Key Tests (examples)                                         | Status |
|----|---------------------------|-----------------------|--------------------------------------------------------------|--------|
| ✅ | Client Events             | `ClientHandler`       | `OnConnect`, `OnDisconnect`, `OnMessageReceived`             | **Done** |
| ✅ | AppLock Re-entry Guard    | `AppLock`             | second instance denied, mutex disposal                       | **Done** |
| ✅ | Input Parsing             | `InputHandler`        | `/exit`, `/w`, generic text                                  | **Done** |
| ✅ | Config & Port Helpers     | `Configuration`       | unique *ClientId*, `IsEndpointBusy`, record `with` cloning   | **Done** |
| ✅ | Message Format            | `Message`             | JSON round-trip, converter recognises `TextMessage`          | **Done** |
| ✅ | Factory Smoke             | `CommunicatorFactory` | correct enum → transport; throws on bad enum                 | **Done** |
| ✅ | **UDP integration**       | `UdpCommunicator`     | single chat, `/exit`, order preservation                     | **Done** |
| ✅ | Disposal Guard (UDP)      | `UdpCommunicator`     | dispose while receiving                                      | **Done** |
| ✅ | **TCP integration**       | `TcpCommunicator`     | round-trip chat, start/stop smoke                            | **Done** |
| ✅ | **Edge cases (TCP/UDP)**  | Sender / Receiver     | 60 kB overflow, zero-length reject, half-close, token cancel | **Done** |

---

## 📘 Notes

*   **Zero-byte TCP writes** are treated as protocol errors and now raise `ArgumentException`; the test asserts the guard instead of waiting forever.  
*   New helpers (`PortFinder`, `TestConfig`, `TaskExtensions.TimeoutAfter`) keep tests concise and prevent flaky delays.  
*   Coverlet collector is wired to CI; the shield badge in README auto-updates after each push.  
*   When TCP feature work resumes (back-pressure, congestion metrics) we’ll mirror edge-case tests for those scenarios.

---

## 🔄 Last Updated
`25 / 07 / 2025`
