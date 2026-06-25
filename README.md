# Audio Switcher Plugin for Loupedeck & Logi

**Audio Switcher** is a simple yet powerful plugin for Loupedeck and Logitech (Logi Plugin
Service) devices. It lets you switch your Windows default audio **output** and **input** device
with a single button press or a dial — perfect for quickly moving between headphones, speakers,
or microphones during streams and calls.

![License](https://img.shields.io/badge/license-Proprietary-red.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![Loupedeck](https://img.shields.io/badge/Loupedeck%20%2F%20Logi-v6.0%2B-green.svg)

## Actions

The plugin contributes the following actions (group **Audio**):

| Action | Type | What it does |
| --- | --- | --- |
| **Audio Output** / **Audio Input** | Button | Pick one specific device in the action editor; pressing the button makes that device the default. |
| **Cycle Audio Output** / **Cycle Audio Input** | Button | One-touch switch to the *next* active device. The button label shows the currently active device. |
| **Audio Output (Dial)** / **Audio Input (Dial)** | Dial | Rotate to cycle through devices forwards/backwards. The dial value shows the currently active device. |

> **Note on the active-device label:** the Cycle and Dial actions update their label after *you*
> use them. If the default device is changed elsewhere (Windows sound settings or another app),
> the label refreshes on the next interaction rather than instantly.

## Installation

1. Download the latest release (`AudioSwitcher_1_1_1.lplug4`) from the
   [Releases page](https://github.com/joelfriesecke/audio-switcher/releases).
2. Double-click the downloaded file. The Loupedeck / Logi software installs the plugin
   automatically.
3. Restart the software if necessary.

## Usage

1. Open the Loupedeck / Logi software.
2. Locate **Audio Switcher** in the plugin list.
3. Drag one of the **Audio** actions onto a button or dial on your device layout.
4. Press the button (or turn the dial) to switch your audio device.

## Requirements

- **OS**: Windows 10 or 11
- **Software**: Logi Plugin Service / Loupedeck **6.0 or later**
- **Hardware**: Loupedeck CT, Live, Live S, Razer Stream Controller / Stream Controller X,
  or Logitech MX Creative Console

## Build

```powershell
.\build.ps1
```

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download) and the `logiplugintool`
command-line tool (installed with the Logi Plugin Service). The script builds, stages, packs
and verifies `AudioSwitcher_<version>.lplug4`.

## License

**Proprietary** — personal, non-commercial use only. See the [LICENSE](LICENSE) file for the
full End User License Agreement.

---

*This plugin is a community project and is not officially affiliated with Logitech or Loupedeck.*
