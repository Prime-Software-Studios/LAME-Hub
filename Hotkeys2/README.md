# Hotkeys2
A small .NET Library for Global Hotkey binding. Built upon [@mrousavy](https://github.com/mrousavy)'s original .NET Framework library [Hotkeys](https://github.com/mrousavy/Hotkeys) and reimagined for .NET 6

## Usage

See the `Hotkeys.Example` project for a more complicated example demonstrating the `HotkeyService` and how to handle some edge cases that come up when implementing a KeyDown listener to let a user choose their own hotkeys.

Simple Example
```cs
using Hotkeys;
using System.Windows;
using System.Windows.Input;

//...

Hotkey key = new(
    key: Key.S,
    window: this,
    modifiers: (ModifierKeys.Control | ModifierKeys.Alt),
    description: "Heavy Duty Save Hotkey",
    action: hotkey => MessageBox.Show($"{hotkey.Name} was pressed!"));

//...

key.Dispose();
```
