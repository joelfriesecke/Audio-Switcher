# Audio Switcher Plugin for Loupedeck

**Audio Switcher** is a simple yet powerful plugin for Loupedeck consoles. It allows you to toggle or cycle through your Windows default audio output devices with a single button press. Perfect for quickly switching between headphones and speakers during streams or calls.

![License](https://img.shields.io/badge/license-Proprietary-red.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![Loupedeck](https://img.shields.io/badge/Loupedeck-v5.0%2B-green.svg)

## Features

-   **One-Touch Switching**: Cycle through enabled audio playback devices.
-   **Visual Feedback**: The button icon updates to show the current active device status.
-   **Seamless Integration**: Works natively with Loupedeck CT, Live, Live S, and Razer Stream Controller.

## Installation

1.  Download the latest release (`.lplug4` file) from the [Releases Page](https://github.com/joelfriesecke/Audio-Switcher/releases).
2.  Double-click the downloaded file (`AudioSwitcher_1.0.lplug4`).
3.  Loupedeck software will open and install the plugin automatically.
4.  Restart the Loupedeck software if necessary.

## Usage

1.  Open the Loupedeck software.
2.  Locate **Audio Switcher** in the plugin list (right-hand sidebar).
3.  Drag the **Switch Audio Output** action onto a button or touch dial on your device layout.
4.  Press the button to cycle through your audio devices.

## Requirements

-   **OS**: Windows 10 or 11
-   **Software**: Loupedeck Software 5.0 or later
-   **Hardware**: Loupedeck CT, Live, Live S, Razer Stream Controller, or Loupedeck+

## Building from Source

To build this plugin yourself:

1.  Clone the repository.
    ```bash
    git clone https://github.com/joelfriesecke/Audio-Switcher.git
    cd Audio-Switcher/AudioOutputSwitcherPlugin
    ```
2.  Open the solution `AudioOutputSwitcherPlugin.sln` in Visual Studio 2022.
3.  Restore NuGet packages and build the solution in **Release** mode.
4.  The output `.dll` files will be in `bin/Release`.

## License

**Proprietary License**

Copyright © 2026 Joel Friesecke. All rights reserved.

This software is provided for personal, non-commercial use only.
You are **not** allowed to:
-   Sell this software.
-   Sublicense this software.
-   Distribute modified versions commercially.

See the [LICENSE](LICENSE) file for details.

---

*Note: This plugin is a community project and is not officially affiliated with Loupedeck.*
