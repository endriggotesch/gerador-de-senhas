$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$dist = Join-Path $projectRoot "dist"
$compiler = Join-Path $env:WINDIR "Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$source = Join-Path $projectRoot "PasswordGeneratorPortable.cs"
$output = Join-Path $dist "GeradorDeSenhas.exe"

if (-not (Test-Path $compiler)) {
  $compiler = Join-Path $env:WINDIR "Microsoft.NET\Framework\v4.0.30319\csc.exe"
}

if (-not (Test-Path $compiler)) {
  throw "Compilador do .NET Framework nao encontrado neste Windows."
}

New-Item -ItemType Directory -Force -Path $dist | Out-Null

& $compiler `
  /target:winexe `
  /platform:anycpu `
  /optimize+ `
  /win32manifest:app.manifest `
  /reference:System.dll `
  /reference:System.Drawing.dll `
  /reference:System.Windows.Forms.dll `
  /out:$output `
  $source

Write-Host "Executavel criado em: $output"
