<#
  A simple script for counting strings at the hole .net project, start counting only from  directory with sln/slnx file
  By: TheMakarik
#>
using namespace System.IO

Set-Variable -Name SolutionExtensions -Value @('.sln', '.slnx') -Option Constant -Scope Local
Set-Variable -Name CountableExtensions -Value @('.slnx', '.sln', '.cs', '.fs', '.vb', '.xaml', '.axaml', '.ps1', '.psm1', '.csproj', '.lua') -Option Constant -Scope Local
Set-Variable -Name ExcludeFolders -Value @('bin', 'obj', 'node_modules', '.git', '.vs', 'packages', '_ReSharper') -Option Constant -Scope Local


$solution = Get-ChildItem -Path $PSScriptRoot |
    Where-Object {
        $extension = [Path]::GetExtension($_.Name)
        $extension -in $SolutionExtensions
    } |
    Select-Object -ExpandProperty Name

if ($solution.Count -eq 0) {
    Write-Host 'Cannot find any solution files, please put this script to the folder with solution and restart' -ForegroundColor DarkRed
    exit -1
}

Write-Host "`nFound solution: $solution" -ForegroundColor Green
Write-Host "Counting lines of code in project..." -ForegroundColor Cyan
Write-Host ("=" * 60)


$statistics = @{}
$totalFiles = 0
$totalLines = 0

Get-ChildItem -Path $PSScriptRoot -Recurse -File |
    Where-Object {
        $extension = [Path]::GetExtension($_.Name)
        $extension -in $CountableExtensions -and 
        ($ExcludeFolders | Where-Object { $_.FullName -match $_ }).Count -eq 0
    } |
    ForEach-Object {
        $extension = [Path]::GetExtension($_.Name)
        $fileName = $_.FullName
        
        try {
            $lineCount = (Get-Content $fileName -ReadCount 0).Count
            
            if (-not $statistics.ContainsKey($extension)) {
                $statistics[$extension] = @{
                    Files = 0
                    Lines = 0
                }
            }
            
            $statistics[$extension].Files++
            $statistics[$extension].Lines += $lineCount
            
            $totalFiles++
            $totalLines += $lineCount
            
            if ($_.Length -gt 1MB) {
                Write-Host "  Processed large file: $([Path]::GetFileName($fileName)) ($lineCount lines)" -ForegroundColor DarkGray
            }
        }
        catch {
            Write-Host "  Error reading file: $fileName" -ForegroundColor Red
        }
    }

Write-Host "`n" + ("=" * 60)
Write-Host "CODE STATISTICS" -ForegroundColor Yellow
Write-Host ("=" * 60)

foreach ($ext in $CountableExtensions) {
    if ($statistics.ContainsKey($ext)) {
        $files = $statistics[$ext].Files
        $lines = $statistics[$ext].Lines
        
        $percentage = if ($totalLines -gt 0) { [math]::Round(($lines / $totalLines) * 100, 1) } else { 0 }
        
        Write-Host ($ext.PadRight(10)) -NoNewline
        Write-Host ("Files: " + $files.ToString().PadLeft(5)) -NoNewline -ForegroundColor Gray
        Write-Host (" | Lines: " + $lines.ToString("N0").PadLeft(10)) -NoNewline -ForegroundColor Green
        Write-Host (" | " + $percentage.ToString().PadLeft(5) + "%") -ForegroundColor Cyan
    }
}

Write-Host ("=" * 60)
Write-Host "TOTAL:".PadRight(10) -NoNewline
Write-Host ("Files: " + $totalFiles.ToString().PadLeft(5)) -NoNewline -ForegroundColor Cyan
Write-Host (" | Lines: " + $totalLines.ToString("N0").PadLeft(10)) -ForegroundColor Cyan
Write-Host ("=" * 60)

Write-Host "`nPROJECT INFORMATION" -ForegroundColor Yellow
Write-Host ("=" * 60)

$projectFiles = Get-ChildItem -Path $PSScriptRoot -Recurse -Filter "*.csproj" -File
$solutionFiles = Get-ChildItem -Path $PSScriptRoot -Recurse -Filter "*.sln" -File

Write-Host "Solution files: $($solutionFiles.Count)" -ForegroundColor White
Write-Host "Project files: $($projectFiles.Count)" -ForegroundColor White

$hasCode = $false
foreach ($ext in $CountableExtensions) {
    if ($statistics.ContainsKey($ext)) {
        $hasCode = $true
        break
    }
}

if ($hasCode) {
    Write-Host "`nCODE BY LANGUAGE:" -ForegroundColor Cyan
    foreach ($ext in $CountableExtensions) {
        if ($statistics.ContainsKey($ext)) {
            $lines = $statistics[$ext].Lines
            Write-Host ("  " + $ext.PadRight(5) + ": " + $lines.ToString("N0").PadLeft(8) + " lines") -ForegroundColor Gray
        }
    }
}