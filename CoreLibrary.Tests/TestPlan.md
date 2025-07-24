# ✅ CoreLibrary Test Coverage Plan

This document tracks unit- and integration-test coverage goals for the networking-chat system.

---

## 🎯 Target Coverage: ≥ 80 % of core logic

Focus is placed on

* Observable-event correctness  
* Lifecycle integrity (e.g. sockets disposed safely)  
* Key logical units (AppLock, handlers, communicators)

---

## ✅ Progress Table

| ✓ | Module            | Target class / area       | Key tests (examples)                                  | Status |
|---|-------------------|---------------------------|-------------------------------------------------------|--------|
| ✓ | Configuration     | `Configuration`           | Unique ClientId, default fall-backs                   | Done   |
| ✓ | App-lock          | `AppLock`                | Mutex prevents 2nd instance                           | Done   |
| ✓ | Messaging         | `Message`, type converter | JSON round-trip, enum serialization                   | Done   |
| ✓ | Factory           | `CommunicatorFactory`     | Transport selection, bad-enum guard                  | Done   |
| ✓ | UDP send/receive  | `UdpSender/Receiver`      | 60 kB cap, order preservation, exit message           | Done   |
| ✓ | TCP parity        | `TcpSender/Receiver`      | Round-trip chat, dispose smoke-test                   | Done   |
| ⏳ | Edge-cases        | All transports            | Zero-length payload, half-close, cancellation         | **Planned** |
| ⏳ | Distributed store | TBD                       | Dynamo/Cosmos CRUD integration tests                  | **Planned** |

---

## 📘 Notes
* Coverage currently ≈ 72 % (see CI badge). Goal is 80 %+ after adding the _Edge-cases_ bucket.  
* Future modules (Distributed-Store prototype, Payment flow) will add their own test suites.

---

## 🔄 Last updated
`24 / 07 / 2025`
