# Soundboard
A C# WPF program that allows sounds to be stored and played through a microphone at the push of a button

## TODOS

* All the todos (duh)
* Update the UI to look good
* Update Audio service to default to speakers only if no virtual audio devices are set up (Need to double check if this is already handled)
* A general cleanup of comments, code and files (More services?)
* Allow user to pick out a virtual audio device (Should have the app be able to handle as much as possible without having to juggle another application)

### Notes

This is notes for later, when I finally make this readme useful

Packages: AutoFac + DI net version, NAudio
NAudio.Sdl2
EFCore.SQLite,Design,Tools

To play through mic, need a virtual audio device. Will need a way for users to pick that out
/select audio devices besides picking thru 3rd party application (Voicemeeter)

