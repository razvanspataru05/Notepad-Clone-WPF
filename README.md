# Notepad-Clone-WPF
A Notepad++ inspired text editor build with C# WPF using MVVM architecture. Features tabbed editing, file system explorer, find/replace functionalities and persistent state between sessions.

## Features
- Tabbed text editing with unsaved changed indicator
- File System Explorer which uses lazy loading
- Find & Replace functionalities (current tab / all tabs)
- Persistent tree state and view settings between sessions
- Directory operations: copy (path / folder), paste, new file
- Keyboard shortcuts for file operations

## Architecture
- **MVVM** pattern with RelayCommand
- Code-behind used only for UI operations
- Callback pattern for ViewModel-UI communication

## Technologies
- C# / .NET
- WPF
- MahApps.Metro.IconPacks

## How to run
1. Clone the repository
2. Restore NuGet packages after opening the '.sln' file
3. Build and run
