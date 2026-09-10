# Validation record

Validated on **10 September 2026**, Windows 11 build **26200**, x64, .NET SDK **10.0.401**, .NET runtime **10.0.12**, Visual Studio **18.10.12201.205**, and Windows App SDK **2.4.0**. The interactive desktop reported **216 DPI (225% scaling)**. The earlier Before build was also compiled with SDK 10.0.400.

## Build and deployment checks

| Check | Result |
| --- | --- |
| Before Debug/x64 build | Passed, zero warnings/errors |
| After Debug/x64 build | Passed, zero warnings/errors |
| Independent Release/x64 builds via `build/Verify.ps1` | Passed, zero warnings/errors |
| Locked restore of each solution | Passed |
| After `--verify-host` integration checks | Passed; process exits with code 0 |
| Documented .NET + Windows App SDK self-contained folder publish | Passed |
| Same integration checks from published `artifacts/After/PlantOps.exe` | Passed; process exits with code 0 |
| Locked restore after publishing | Passed |
| Core equipment/repository files and maintenance form match across solutions | SHA-256 hashes identical |
| No cross-solution project references | Verified; each solution owns its project files |

`docs/host-verification.txt` records the host check results. Rerun `build/Verify.ps1 -RunHostChecks` to generate a fresh report under `artifacts/`. The script opens real windows and must run on an interactive Windows desktop. Normal application startup does not run these checks.

## Runtime checks performed

- Loaded a minimal **compiled XAML** control inside WinForms before implementing the composite panel. Its text was visible in the native child and accessibility tree. This temporary development control has since been removed.
- Ran both Before and After independently. All five equipment selections updated their respective panels. After's statuses, health, runtime, alert counts and warning visibility were checked, including the offline machine and machines with no warning.
- Clicked Schedule maintenance for CNC Mill 04 in both versions. Both opened the existing owned WinForms dialog. Scheduling displayed “Maintenance scheduled for CNC Mill 04”; After's status bar showed the completion message.
- Cancelled the Before dialog with Escape. After's integration check invoked the actual XAML button automation peer for each machine, checked the WinForms dialog's owner and machine name, clicked its Cancel button, and checked focus restoration.
- Used actual keyboard input on the published After app: clicked the WinForms list, pressed Tab and observed the WinUI button's focus ring; Shift+Tab returned to the list; Down selected Laser Cutter 02; Tab then Space opened its maintenance dialog; Escape cancelled; Tab returned to the Equipment navigation button. UI Automation's reported focused element was not reliable for this hybrid window, so visible focus and resulting actions were used as evidence.
- Maximized and restored After. The WinUI content remained embedded. The repeatable checks also resized the form to three distinct logical sizes, confirmed the island's native parent HWND, and compared actual child client dimensions with the WinForms host dimensions at 216 DPI.
- Forced host HWND recreation. A new panel was created, retained the selected equipment, and opened a single functioning maintenance dialog. Disposing the host cleared the source and panel references.
- Closed the normal application windows and observed process exit. The integration and published-app checks exited normally after closing their forms and shutting down XAML/dispatcher services.
- Saved comparable screenshots of CNC Mill 04 in [Before](images/before.jpg) and [After](images/after.jpg). These are actual window captures, not mockups.

## Checks still requiring another environment

These are explicit coverage limits, not claims of tests completed:

- Moving the live window between monitors with **different DPI settings**, and repeating at 100%, 150% and 200%. No system display settings were changed during validation.
- Deployment on a **clean Windows 11 machine** without development tooling or previously installed runtimes. The published app was tested locally, where other runtimes are installed.
- Narrator/screen-reader navigation, high contrast, touch/pen, and alternative keyboard layouts. Controls use standard WinForms/WinUI accessibility support, but these combinations were not audited.
- Windows 11 versions older than the local build, ARM64/x86, and dark theme. The latter architectures and theme are outside the chosen demo scope.

## Reader's quick acceptance check

1. Run Before, select CNC Mill 04, open maintenance, cancel, reopen and schedule.
2. Run After and repeat. Only equipment details and its initiating button should be WinUI; the dialog and application shell remain WinForms.
3. Select each machine, especially the long Coordinate Measuring Machine name and Laser Cutter with no warning.
4. Resize/maximize/restore; tab in both directions across the list/island boundary; activate with Space and cancel with Escape.
5. Close the application and confirm no PlantOps process remains.
