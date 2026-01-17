# UmBuster – Real-Time Filler Word Detector

## Purpose
UmBuster helps speakers reduce filler words (*um, okay, right, so*) by providing **real-time visual feedback** during practice, live sessions, or online meetings (Zoom/Teams).

---

## Core Features
- **Real-Time Speech Monitoring**
  - Captures audio from the microphone.
  - Transcribes speech locally using an offline speech-to-text engine.
  - Detects filler words from a customizable list.

- **Filler Word Alerts**
  - Visual only: a floating overlay increments a counter and highlights the detected filler word.

- **On-Demand Control**
  - Simple ON/OFF toggle to start or stop detection.

- **Customizable Filler List**
  - Default list: *um, uh, okay, right, so, like, you know*.
  - User can add/remove words via a small settings panel.

---

## UI Concept
- **Floating Overlay Window (always-on-top)**
  - Displays:
    - Current filler count.
    - Last detected filler word.
- **Settings Button**
  - Opens a small panel to edit the filler word list.

---

## Tech Stack
- **Frontend/UI:** WPF (.NET 8) or WinUI.
- **Audio Capture:** NAudio or Windows WASAPI.
- **Speech-to-Text:** Whisper.cpp or Vosk (local only).
- **Data:** In-memory.
- **Packaging:** MSIX installer for distribution.
