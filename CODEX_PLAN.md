# CODEX_PLAN — WinForms.WinUI.Modernization / PlantOps

## Instructions for the implementing agent

Implement the demo described below in the repository `WinForms.WinUI.Modernization`. Work through the development sequence, build both solutions, validate the acceptance criteria, and deliver runnable code plus documentation. This file is the development brief; do not stop after producing another plan.

Repository description: **Incrementally modernising a .NET WinForms application with embedded WinUI 3 components using XAML Islands.**

The demo accompanies a technical article for software development managers whose mature WinForms applications remain valuable but need selected UI improvements. Assume the application has already moved to modern .NET; runtime migration is outside this example.

The following plan preserves the agreed architecture and requirements from the “WinForms WinUI3 Upgrade Ideas” conversation.

---

# Codex Project Plan — WinForms + WinUI 3 Incremental Modernization

## Objective

Create a small demonstration repository showing how an existing modern-.NET Windows Forms application can be incrementally modernised by replacing one area of its UI with a WinUI 3 `UserControl`.

The application should represent a fictional industrial equipment/service management system called **PlantOps**.

The key point of the project is **not** to demonstrate a full migration from WinForms to WinUI.

Instead, it should demonstrate this architectural proposition:

> A mature WinForms application can retain its existing shell, navigation, business logic, event handling and existing forms while selectively introducing WinUI 3 for parts of the UI that benefit from modernization.

The example must remain deliberately small and easy to understand so it can accompany a technical article.

---

## Repository Structure

Use a single repository:

```text
WinForms.WinUI.Modernization
│
├── README.md
│
├── Before/
│   ├── PlantOps.Before.sln
│   │
│   ├── PlantOps/
│   └── PlantOps.Core/
│
├── After/
│   ├── PlantOps.After.sln
│   │
│   ├── PlantOps/
│   ├── PlantOps.Core/
│   └── PlantOps.WinUI/
│
└── docs/
    ├── architecture.md
    └── images/
```

The **Before** and **After** applications should be completely runnable independently.

Do not make the After solution reference projects physically located inside the Before directory.

Some duplication is acceptable because clarity is more important than eliminating repeated code.

The reader should be able to clone the repository and run either solution without having to understand Git history or branches.

---

# Technical Baseline

Use:

```text
.NET 10
C#
Windows Forms
WinUI 3
Windows App SDK
x64 initially
Windows 11
Visual Studio 2026/current supported Visual Studio
```

Use the current supported Windows App SDK release compatible with .NET 10.

Keep external dependencies to an absolute minimum.

Do not introduce:

- Entity Framework
- databases
- dependency injection frameworks
- MVVM frameworks
- logging frameworks
- third-party control libraries
- unnecessary abstractions

This repository exists to explain **WinForms/WinUI integration**, not application architecture generally.

---

# Application Scenario

PlantOps is a fictional internal application used by a manufacturing company to manage industrial machinery.

Imagine that the real application has existed for many years and contains substantial functionality, but the demo will implement only enough UI and domain behaviour to demonstrate the modernization technique.

The main window should resemble a traditional line-of-business Windows application.

Example:

```text
┌───────────────────────────────────────────────────────────────┐
│ PlantOps — Equipment Management                              │
├───────────────┬───────────────────────────────────────────────┤
│               │                                               │
│ Equipment     │                                               │
│ Work Orders   │        Current application content            │
│ Maintenance   │                                               │
│ Reports       │                                               │
│ Settings      │                                               │
│               │                                               │
└───────────────┴───────────────────────────────────────────────┘
```

The left-hand navigation should remain WinForms in both versions.

The main application shell should remain WinForms in both versions.

---

# Shared Domain Model

Create a deliberately tiny domain layer.

For example:

```csharp
public sealed class Equipment
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string SerialNumber { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public EquipmentStatus Status { get; init; }

    public int HealthPercentage { get; init; }

    public DateTime LastServiceDate { get; init; }

    public DateTime NextServiceDate { get; init; }

    public double TemperatureCelsius { get; init; }

    public int RuntimeHours { get; init; }

    public int ActiveAlerts { get; init; }

    public string? CurrentWarning { get; init; }
}
```

Possible status enum:

```csharp
public enum EquipmentStatus
{
    Operational,
    MaintenanceRequired,
    Offline
}
```

Create an in-memory service such as:

```csharp
EquipmentRepository
```

returning perhaps five machines.

Example equipment:

```text
CNC Mill 04
Laser Cutter 02
Hydraulic Press 07
Coordinate Measuring Machine 01
Packaging Line 03
```

No persistence is required.

---

# BEFORE APPLICATION

The Before solution represents the application prior to UI modernization.

It should look competent and functional rather than deliberately ugly.

## Main Form

Create:

```text
MainForm
```

with:

```text
left navigation panel
Equipment ListBox/ListView
content panel
status strip
menu strip if appropriate
```

Selecting **Equipment** displays an equipment management screen.

The user should be able to select machines from a WinForms `ListView`, `ListBox` or `DataGridView`.

Selecting equipment updates the existing WinForms equipment detail panel.

---

# Existing WinForms Equipment UI

Implement something approximately like:

```text
Equipment Details

Equipment:       CNC Mill 04
Serial Number:   MX-4821
Location:        Production Hall A
Status:          Operational

Last service:    12 June 2026
Next service:    12 December 2026

Health:          82%
Temperature:     64°C
Runtime:         1284 hours
Alerts:          2

Spindle vibration above normal range

[ Schedule Maintenance ]
```

Use ordinary WinForms controls:

```text
Panel
Label
TextBox or read-only labels
ProgressBar
Button
GroupBox
TableLayoutPanel
```

The important point is that this UI **already works**.

Do not make it intentionally broken.

---

# Existing Application Behaviour

Create an existing WinForms form:

```text
ScheduleMaintenanceForm
```

When the user clicks:

```text
Schedule Maintenance
```

the existing application opens that form.

The form can contain:

```text
Equipment
Maintenance Type
Requested Date
Engineer Notes

[Schedule] [Cancel]
```

No persistence is necessary.

When the user clicks Schedule, simply show:

```text
Maintenance scheduled for CNC Mill 04
```

or update an in-memory object.

This form represents an existing piece of PlantOps functionality that will **remain WinForms** after modernization.

This is important to the demonstration.

---

# Existing Event Flow

The Before application should have a clearly identifiable existing application-level mechanism for requesting maintenance.

Avoid excessive architecture.

For example:

```csharp
EquipmentDetailsControl.MaintenanceRequested
```

could be raised by the existing WinForms equipment screen.

`MainForm` subscribes to the event and opens:

```csharp
ScheduleMaintenanceForm
```

This allows the After version to demonstrate that the new WinUI component can participate in the **same existing application workflow**.

---

# AFTER APPLICATION

The After solution should initially contain essentially the same application.

The surrounding application must visibly remain WinForms.

Do not redesign the entire shell.

The modernization should affect only the equipment information area.

The equipment list remains WinForms.

Navigation remains WinForms.

MainForm remains WinForms.

ScheduleMaintenanceForm remains WinForms.

The domain model remains unchanged.

The main difference is:

```text
EquipmentDetailsControl
```

is replaced by a hosted WinUI component.

---

# WinUI Project

Create a separate project:

```text
PlantOps.WinUI
```

containing a WinUI 3 `UserControl`:

```text
EquipmentStatusPanel
```

The WinForms application should host this component using the appropriate WinUI 3/XAML Islands infrastructure for the Windows App SDK version being used.

Keep the WinUI hosting infrastructure explicit enough that readers can understand it.

Do not hide everything behind a giant framework abstraction.

However, it is acceptable to introduce a small reusable helper such as:

```text
WinUIHostControl
```

if this removes distracting HWND plumbing from `MainForm`.

If such a helper is created, its purpose should be obvious.

---

# EquipmentStatusPanel

Create a visually modern composite WinUI control.

Something approximately like:

```text
┌───────────────────────────────────────────────────┐
│ CNC Mill 04                         ● Operational │
│ MX-4821                                           │
│                                                   │
│ Equipment health                                  │
│ ████████████████████░░░░░   82%                  │
│                                                   │
│ Next scheduled maintenance                        │
│ 12 December 2026                                  │
│                                                   │
│ Temperature      Runtime        Active Alerts      │
│    64 °C         1,284 h             2             │
│                                                   │
│ ⚠ Spindle vibration above normal range            │
│                                                   │
│              Schedule maintenance                 │
└───────────────────────────────────────────────────┘
```

Use standard WinUI controls where appropriate.

Possible candidates include:

```text
Grid
StackPanel
Border
TextBlock
Button
ProgressBar
InfoBar
FontIcon
TeachingTip only if genuinely useful
```

Do not try to demonstrate every WinUI control.

The UI should look cleaner and more modern than the original WinForms panel, but the difference should remain believable.

---

# Passing Existing Application State Into WinUI

The existing WinForms equipment list should continue controlling the selected equipment.

Flow:

```text
WinForms ListView
       │
       │ Equipment selected
       ▼
MainForm
       │
       │ selected Equipment
       ▼
WinUI EquipmentStatusPanel
```

Expose a simple method or property on the WinUI control such as:

```csharp
public void SetEquipment(Equipment equipment)
```

or:

```csharp
public Equipment? Equipment
```

Choose whichever works most naturally with WinUI interop.

The important architectural message is:

> Existing WinForms code still owns application navigation and application state.

The WinUI component receives the domain information it needs.

Do not create a separate duplicate WinUI application state model.

---

# Passing Events Back From WinUI to WinForms

This is one of the most important parts of the demo.

The WinUI control should expose an event such as:

```csharp
public event EventHandler<MaintenanceRequestedEventArgs>? MaintenanceRequested;
```

When the user clicks the WinUI:

```text
Schedule maintenance
```

button, the control raises the event.

The WinForms host subscribes to it.

The event should ultimately execute the **existing WinForms maintenance workflow**.

Flow:

```text
WinUI Button
     │
     │ Click
     ▼
EquipmentStatusPanel
     │
     │ MaintenanceRequested
     ▼
WinForms host
     │
     │ existing handler
     ▼
ScheduleMaintenanceForm
```

The existing `ScheduleMaintenanceForm` should open exactly as it did in the Before application.

This is the central demonstration of the repository.

The new WinUI UI is not a separate application.

It participates in the existing WinForms application.

---

# Make the Existing Code Reusable

Try to structure the code so that the event handler itself survives the modernization.

For example:

```csharp
private void RequestMaintenance(Equipment equipment)
{
    using var dialog = new ScheduleMaintenanceForm(equipment);
    dialog.ShowDialog(this);
}
```

Before:

```csharp
_winFormsEquipmentControl.MaintenanceRequested += ...
```

After:

```csharp
_winUIEquipmentControl.MaintenanceRequested += ...
```

Both eventually call:

```csharp
RequestMaintenance(equipment);
```

That gives us an excellent code comparison for the article.

It visually proves that existing application behaviour does not need to be rewritten just because its UI initiator changed.

---

# WinUI Hosting Infrastructure

Implement the correct hosting mechanism for WinUI 3 inside WinForms using the current Windows App SDK.

The implementation should explicitly address:

```text
Windows App SDK initialization
WinUI/XAML initialization
creation of DesktopWindowXamlSource
attaching the XAML island to a WinForms HWND
creating the EquipmentStatusPanel
sizing the island when the WinForms host resizes
focus/input
threading requirements
disposing/closing DesktopWindowXamlSource
application shutdown
```

The implementation should be safe and deterministic.

Comment the important interop sections heavily.

This is educational code.

Readers should be able to understand **why** unusual interop calls exist.

---

# Resizing

The WinUI component must behave like an ordinary WinForms child control from the user's perspective.

When the WinForms form resizes:

```text
WinForms host panel
       ↓
XAML Island HWND
       ↓
WinUI component
```

all three should resize correctly.

Avoid hard-coded dimensions except initial defaults.

This should be one of the article's implementation sections because it demonstrates that the WinUI control is genuinely embedded rather than simply appearing in a separate window.

---

# Styling

Use WinUI's normal Fluent styling.

Do not introduce a custom design system.

Support at least:

```text
normal Windows scaling
high DPI
light theme
```

Dark theme support is optional.

If dark-theme support comes essentially for free, retain it, but don't complicate the article significantly just to demonstrate theming.

---

# Deliberate Scope Boundaries

Do **not** migrate:

```text
MainForm
navigation
equipment ListView
menus
status bar
ScheduleMaintenanceForm
business/domain model
application startup architecture unnecessarily
```

Those remaining WinForms components are part of the demonstration.

The repository should visually contain both technologies at the same time.

---

# Desired Architecture

Conceptually:

```text
                        PlantOps
                           │
                  Existing WinForms
                           │
              ┌────────────┴────────────┐
              │                         │
         Navigation                Equipment List
          WinForms                    WinForms
                                        │
                                        │ selection
                                        ▼
                             EquipmentStatusPanel
                                   WinUI 3
                                        │
                                        │
                         MaintenanceRequested event
                                        │
                                        ▼
                                  MainForm
                                   WinForms
                                        │
                                        ▼
                          ScheduleMaintenanceForm
                                   WinForms
                                        │
                                        ▼
                           Existing application logic
```

That diagram should potentially be included in `docs/architecture.md`.

---

# README

Create a high-quality README targeted at developers who arrive from the article.

Opening message should explain:

> PlantOps is a deliberately small example showing how an existing .NET Windows Forms application can introduce WinUI 3 incrementally rather than being completely rewritten.

Explain that the repository contains:

```text
Before
After
```

and that the interesting comparison is the equipment details UI.

Include:

```text
Prerequisites
How to run Before
How to run After
Architecture
Key modernization steps
Relevant source files
Known limitations
```

Do not turn README into the complete article.

The article will provide the narrative; README should make the repository easy to explore.

---

# Important Code Documentation

Use comments where interop behaviour would otherwise be obscure.

For example:

```csharp
// DesktopWindowXamlSource owns the XAML Island that allows
// this WinUI visual tree to be hosted inside the WinForms HWND.
```

Avoid excessive comments on ordinary C#.

Something like:

```csharp
// Set equipment name
nameTextBlock.Text = equipment.Name;
```

does not need a comment.

Interop code does.

---

# Code Quality

Code should be production-style despite being a demo.

Use:

```text
nullable reference types
file-scoped namespaces if appropriate
meaningful names
async APIs where actually required
proper disposal
correct event unsubscription
deterministic XAML Island cleanup
```

Avoid unnecessarily sophisticated patterns.

A development manager reading the source should think:

> "This is surprisingly contained."

That is an important outcome of the demonstration.

---

# Suggested Development Sequence

Codex should implement the project in this order:

1. Create the repository structure and both solutions.
2. Implement `PlantOps.Core`.
3. Build the complete Before WinForms application.
4. Confirm machine selection works.
5. Confirm the WinForms Schedule Maintenance workflow works.
6. Copy the appropriate baseline application into After.
7. Introduce Windows App SDK/WinUI dependencies.
8. Create `PlantOps.WinUI`.
9. Create `EquipmentStatusPanel`.
10. Prove a basic WinUI `TextBlock` can be hosted inside WinForms before adding application functionality.
11. Implement correct host resizing/lifetime.
12. Pass selected `Equipment` from WinForms into WinUI.
13. Populate the complete equipment panel.
14. Raise `MaintenanceRequested` from WinUI.
15. Connect it to the existing WinForms maintenance workflow.
16. Verify multiple equipment selections.
17. Test resizing and DPI behaviour.
18. Verify clean shutdown.
19. Add comments around interop.
20. Write README and architecture documentation.

Importantly, **do not jump directly to building the complete WinUI panel before proving that the minimal XAML Island host works correctly**.

---

# Acceptance Criteria

The project is complete when a reader can run the Before application, select **CNC Mill 04**, see its WinForms equipment panel, click **Schedule Maintenance**, and see the WinForms maintenance dialog.

They can then run After and perform exactly the same workflow.

But this time:

```text
equipment navigation = WinForms
equipment selection = WinForms
equipment status panel = WinUI 3
Schedule Maintenance button = WinUI 3
maintenance dialog = existing WinForms
application logic = existing C#
```

The user should not perceive that the WinUI component is running through a separate application or process.

It should feel like a native part of PlantOps.

---

# Most Important Principle

> Modernise the part that needs modernising; don't rewrite the application.

This is incremental modernization, not a full migration. Replace one business-oriented UI surface while preserving the existing WinForms shell, navigation, application state, domain model, event handling and maintenance workflow.

The central demonstration is that a WinUI 3 UserControl can receive existing application data and raise an event that invokes existing WinForms behaviour. Visual improvements support that story; they are not the whole story.

---

## Implementation handoff: version verification and deployment

Before implementing the hosting layer, verify the selected Windows App SDK release and its supported .NET, Windows, Visual Studio, and C# XAML Islands requirements against current Microsoft documentation. The baseline above is the requested target, not a claim that an unspecified package version has been tested.

- Keep .NET 10, C#, Windows 11 and x64 as the intended baseline. Record exact SDK/package versions, target frameworks, Windows target/minimum versions and prerequisites in the README. Pin the selected dependency versions for reproducibility.
- Use WinUI 3 (`Microsoft.UI.Xaml`) hosting guidance for the selected Windows App SDK. Do not substitute UWP/WinUI 2 hosting code or assume APIs from older XAML Islands samples are interchangeable.
- Establish the required UI-thread, STA, dispatcher and XAML initialization sequence while retaining the WinForms top-level window and message loop. Explain any necessary startup changes.
- Supply the XAML metadata providers, generated metadata and resources required by the selected C# hosting approach. Check that both the minimal hosted control and the composite panel load successfully.
- Choose and document a straightforward supported packaging/deployment approach, including Windows App SDK runtime/bootstrap requirements where applicable. Provide reproducible build/run instructions for a reader's machine.
- If a verified compatibility constraint requires a baseline adjustment, document the constraint and the smallest viable change. Preserve the single-process, embedded WinUI 3 architecture and the independently runnable Before/After solutions.
- Reference the official documentation used in `docs/architecture.md`. Starting point from the discussion: [Host WinUI controls with XAML Islands](https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/host-controls-existing-desktop-apps).

## Implementation handoff: completion checks

Use these checks alongside the acceptance criteria above:

- Both solutions restore and build independently in the documented x64 configuration. Neither solution references projects in the other solution's directory.
- The Before version is a fully functional, plausible WinForms baseline. The After version uses the same equipment data and preserves the surrounding UI and maintenance workflow.
- Selecting each of the sample machines updates the displayed details correctly; maintenance requests always refer to the currently selected machine.
- Scheduling and cancelling the existing owned WinForms dialog work in both versions. Both UI controls route maintenance requests to the equivalent existing `RequestMaintenance` handler.
- The WinUI panel is visibly embedded in the WinForms content area, within the same process. It is not a separate top-level WinUI application.
- Resizing, normal Windows scaling and high-DPI operation work without clipping or a detached island. Check keyboard focus entering/leaving the island, button activation, and focus restoration after closing the maintenance dialog.
- Closing the application releases the island and subscriptions cleanly and leaves no process running. Handle host lifetime and any relevant HWND recreation deterministically.
- README and architecture documentation explain prerequisites, exact build/run steps, retained versus replaced components, data/event flow, interop lifetime, deployment choices and known limitations.
- Capture comparable Before/After screenshots in `docs/images/` when a working graphical environment is available.
- Report the actual build and runtime checks performed. If the environment prevents a check, state precisely what remains unverified; do not claim completion of tests that were not run.

## Suggested CLI handoff prompt

After placing this file in the repository root, give Codex this instruction:

> Read CODEX_PLAN.md and implement the PlantOps Before/After demo. Start with the complete WinForms baseline, then prove minimal WinUI 3 hosting before building the equipment status panel. Build and validate both solutions, preserve the existing maintenance workflow, and document the results.

