// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "RocksmithToolkitLib.Ogg.OggFile.#WriteOgg(System.String)",
    Justification = "The inner stream may be disposed twice, but without putting it in a using block it may not be disposed at all.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "RocksmithToolkitLib.Ogg.OggFile.#VerifyHeaders()",
    Justification = "The inner stream may be disposed twice, but without putting it in a using block it may not be disposed at all.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "RocksmithToolkitLib.Sng.SngFileWriter.#WriteRocksmithVocalsFile(RocksmithToolkitLib.Xml.Vocals,System.String,MiscUtil.Conversion.EndianBitConverter)",
    Justification = "The inner stream may be disposed twice, but without putting it in a using block it may not be disposed at all.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "RocksmithToolkitLib.Sng.SngFileWriter.#WriteRocksmithSngFile(RocksmithToolkitLib.Xml.Song,System.String,MiscUtil.Conversion.EndianBitConverter)",
    Justification = "The inner stream may be disposed twice, but without putting it in a using block it may not be disposed at all.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "RocksmithToolkitLib.DLCPackage.SoundBankGenerator.#GenerateSoundBank(System.String,System.IO.Stream,System.IO.Stream)",
    Justification = "The inner stream may be disposed twice, but without putting it in a using block it may not be disposed at all.")]
