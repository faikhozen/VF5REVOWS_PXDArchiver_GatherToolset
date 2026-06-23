# =====================================================
# VF5REVO GMD Conflict Checker v0.67
# Developed by: Fai Khozen
# Description: Analyzes gathered .gmd files for conflicts
# when multiple mods contain the same character files.
# Used in conjunction with PXDArchiver for VF5REVO
# WorldStage modding.
# =====================================================

# PowerShell script to check for .gmd file conflicts with color coding
param([string]$HistoryFile)

Write-Host ""
Write-Host "====================================================" -ForegroundColor Cyan
Write-Host "VF5REVO GMD Conflict Checker v0.67" -ForegroundColor Cyan
Write-Host "Developed by: Fai Khozen" -ForegroundColor Cyan
Write-Host "====================================================" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $HistoryFile)) {
    Write-Host "No GMD conflicts detected." -ForegroundColor Green
    exit 0
}

$content = @(Get-Content $HistoryFile)
$fileMap = @{}
$filenameMap = @{}

# Parse the history file (format: /path/to/file.gmd-modname)
foreach ($line in $content) {
    $lastDash = $line.LastIndexOf('-')
    if ($lastDash -gt 0) {
        $filepath = $line.Substring(0, $lastDash)
        $modname = $line.Substring($lastDash + 1)
        
        # Store full path info
        if (-not $fileMap.ContainsKey($filepath)) {
            $fileMap[$filepath] = @()
        }
        $fileMap[$filepath] += $modname
        
        # Also track by filename only to detect overwrites
        $filename = $filepath.Split('/')[-1]
        if (-not $filenameMap.ContainsKey($filename)) {
            $filenameMap[$filename] = @()
        }
        $filenameMap[$filename] += @{path=$filepath; mod=$modname}
    }
}

# Find conflicts based on filename (same .gmd name in different mods)
$conflicts = @{}
foreach ($filename in $filenameMap.Keys) {
    $mods = @($filenameMap[$filename] | ForEach-Object { $_.mod } | Get-Unique)
    if ($mods.Count -gt 1) {
        $conflicts[$filename] = $filenameMap[$filename]
    }
}

$hasConflicts = $conflicts.Count -gt 0

# Display conflicts in RED
if ($hasConflicts) {
    Write-Host ""
    Write-Host "========== GMD CONFLICTS DETECTED ==========" -ForegroundColor Red
    Write-Host "The following files will overwrite each other:" -ForegroundColor Red
    Write-Host ""
    
    foreach ($filename in ($conflicts.Keys | Sort-Object)) {
        Write-Host ("File: " + $filename) -ForegroundColor Red
        Write-Host "  Found in mods:" -ForegroundColor Red
        
        # Get the list of conflicts for this file, in order
        $conflictList = $conflicts[$filename]
        $lastMod = $conflictList[-1].mod
        
        # Display each mod with marking for the last one
        $seenMods = @()
        foreach ($entry in $conflictList) {
            if ($seenMods -notcontains $entry.mod) {
                $seenMods += $entry.mod
                $path = $entry.path
                
                if ($entry.mod -eq $lastMod) {
                    Write-Host ("    - CURRENTLY SET AS: " + $entry.mod + " (" + $path + ")") -ForegroundColor Yellow -BackgroundColor Red
                } else {
                    Write-Host ("    - " + $entry.mod + " (" + $path + ")") -ForegroundColor Red
                }
            }
        }
        Write-Host ""
    }
    
    Write-Host "============================================" -ForegroundColor Red
    Write-Host ""
} else {
    Write-Host ("All " + $filenameMap.Count + " .gmd files are unique - no conflicts detected.") -ForegroundColor Green
    Write-Host ""
}

