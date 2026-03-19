# 🎵 Soundboard
 
A WPF-based soundboard application for Windows, built with C# and .NET 8. Supports customizable sound button grids, hotkey bindings, and persistent storage via Entity Framework Core.

## Requirements
 
- **Windows 10 or later**
- **Visual Studio 2022** (Community edition is fine)
  - Workload: **.NET desktop development**
- **.NET 8.0 Windows SDK**
 
## Getting the Project Setup and Running

### 1. Clone and Open

Afer cloning the repository, open 'Soundboard.sln' in Visual Studio 2022.

### 2. Restore NuGet Packages

Visual Studio should restore packages automatically on build. If not, you can trigger it manually in 2 ways:

 * Right-click the Solution → Restore NuGet Packages
 * Or via the CLI using the command:
 
```bash
dotnet restore
```

### 3. Apply database migrations

Open the **Package Manager Console** in Visual Studio (`Tools → NuGet Package Manager → Package Manager Console`) and run:
```powershell
Update-Database
```

Or via the CLI from the `Soundboard.Domain.DataAccess` project directory:
```bash
dotnet ef database update
```

This will create a local `soundboard.db` SQLite file and apply all migrations automatically. No database installation or configuration required.

### 4. Run the Project!

Press **Soundboard** button beside the green play icon or hit **F5** to launch the program!
If you want a faster startup, use the **Start without Debugging** or hit **Ctrl + F5**

### Additional notes

If you want a shortcut to open the application without using Visual Studio, first build the app in release mode (Change the dropdown to the left of Start Soundboard from debug to Release)
Next, the app will need to compile once, launch the application in either way and then close it.
Then navigate to `Soundboard\bin\Release\net8.0-windows\Soundboard.exe`
Right click the file, and select `Create shortcut`

## Database Migrations

This project uses **Entity Framework Core** with code-first migrations. Any time the database schema changes, a new migration is included with the relevant code changes.
 
If you pull changes that include new migrations, run:
 
```bash
dotnet ef database update
```
 
To check which migrations have or haven't been applied to your local database:
 
```bash
dotnet ef migrations list
```

## Currently Working on/Future Plans

* All the todos  [IN PROGRESS]
* Update the UI to look good [IN PROGRESS]
* Update Audio service to default to speakers only if no virtual audio devices are set up (Need to double check if this is already handled)
* Update the UI to look good
* Update Audio service to default to speakers only if no virtual audio devices are set up
* A general cleanup of comments, code and files (More services?)
* Allow user to pick out a virtual audio device (Should have the app be able to handle as much as possible without having to juggle another application)

### Additional Notes

This is notes for later, when I finally make this readme useful

Packages: AutoFac + DI net version, NAudio
NAudio.Sdl2
EFCore.SQLite,Design,Tools

To play through mic, need a virtual audio device. Will need a way for users to pick that out
/select audio devices besides picking thru 3rd party application (Voicemeeter)

