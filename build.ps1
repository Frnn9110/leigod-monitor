$ErrorActionPreference = 'Stop'
$src = Join-Path $PSScriptRoot 'monitor2.cs'
$out = Join-Path $PSScriptRoot 'LeigodMonitor.exe'
$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
& $csc /nologo /target:winexe /optimize+ /out:$out $src /reference:System.dll /reference:System.Core.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll
Write-Output $out
