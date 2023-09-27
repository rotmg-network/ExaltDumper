# ExaltDumper

### Built on top of Il2CppDumper
---
Unity Il2Cpp automatic reverse engineering and dumping tool, specifically tailored for the game Realm of the Mad God.

## Features

* Complete DLL restore (except code) - can be used to extract `MonoBehaviour` and `MonoScript`
* Supports ELF, ELF64, Mach-O, PE, NSO and WASM format
* Supports Unity 5.3 - 2022.2
* Supports generating IDA, Ghidra and Binary Ninja scripts to help with analyzing Il2Cpp files
* Supports generating structure header file
* Supports bypassing some simple PE protection
* Supports bypassing the custom protection in place for Realm of the Mad God (**work-in-progress**)

## Usage

Run `ExaltDumper.exe` and choose the game DLL file then the `global-metadata.dat` file, then wait for information to be dumped

The program will then generate all the output files in current working directory

### Command-line

```
ExaltDumper.exe <executable-file> <global-metadata> <output-directory>
```

### Outputs

#### DummyDll

Folder, containing all restored dll files

Use [dnSpy](https://github.com/0xd4d/dnSpy), [ILSpy](https://github.com/icsharpcode/ILSpy) or other .Net decompiler tools to view

Can be used to extract Unity `MonoBehaviour` and `MonoScript`, for [UtinyRipper](https://github.com/mafaca/UtinyRipper), [UABE](https://7daystodie.com/forums/showthread.php?22675-Unity-Assets-Bundle-Extractor)

#### ida.py

For IDA

#### ida_with_struct.py

For IDA, read il2cpp.h file and apply structure information in IDA

#### il2cpp.h

structure information header file

#### ghidra.py

For Ghidra

#### Il2CppBinaryNinja

For BinaryNinja

#### ghidra_wasm.py

For Ghidra, works with [ghidra-wasm-plugin](https://github.com/nneonneo/ghidra-wasm-plugin)

#### script.json

For ida.py, ghidra.py and Il2CppBinaryNinja

#### stringliteral.json

Contains all stringLiteral values and their addresses in the binary

### Configuration

All the configuration options are located in `config.json`

Available options:

* `DumpMethod`, `DumpField`, `DumpProperty`, `DumpAttribute`, `DumpFieldOffset`, `DumpMethodOffset`, `DumpTypeDefIndex`
  * Whether to output these information to dump.cs

* `GenerateDummyDll`, `GenerateScript`
  * Whether to generate these things

* `DummyDllAddToken`
  * Whether to add token in DummyDll

* `RequireAnyKey`
  * Whether to press any key to exit at the end

* `ForceIl2CppVersion`, `ForceVersion`
  * If `ForceIl2CppVersion` is `true`, the program will use the version number specified in `ForceVersion` to choose parser for il2cpp binaries (does not affect the choice of metadata parser). This may be useful on some older il2cpp version (e.g. the program may need to use v16 parser on il2cpp v20 (Android) binaries in order to work properly)

* `ForceDump`
  * Force files to be treated as dumped

* `NoRedirectedPointer`
  * Treat pointers in dumped files as unredirected, This option needs to be `true` for files dumped from some devices

## Common errors

#### `ERROR: Metadata file supplied is not valid metadata file.`  

Make sure you choose the correct file. Sometimes games may obfuscate this file for content protection purposes and so on. 
Deobfuscating of such files is beyond the scope of this program, so please **DO NOT** file an issue regarding to deobfuscating.

#### `ERROR: Can't use auto mode to process file, try manual mode.`

Please note that the executable file for the PC is `GameAssembly.dll`

You can open a new issue and upload the results; I will try to solve it.

## Credits

- Perfare - [Original Il2CppDumper](https://github.com/Perfare/Il2CppDumper)
