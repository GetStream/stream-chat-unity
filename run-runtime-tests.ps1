<#
.SYNOPSIS
    Build and run StreamChat runtime tests in a standalone IL2CPP player.

.DESCRIPTION
    Two-step process:
    1. Configure the project: sets scripting backend to IL2CPP, activates
       StandaloneWindows64, and enables the STREAM_TESTS_ENABLED define.
    2. Build a test player and execute it. Unity writes NUnit XML results
       to the specified file.

.PARAMETER UnityPath
    Full path to Unity.exe. If omitted, the script searches Unity Hub's
    default install locations for the version in ProjectSettings/ProjectVersion.txt.

.PARAMETER ScriptingBackend
    "IL2CPP" (default) or "Mono". Passed indirectly: IL2CPP uses
    PrepareStandaloneIL2CPPTests; Mono skips the backend switch and
    only enables the test define.

.PARAMETER ResultsFile
    Path for the NUnit XML results file. Default: TestResults/RuntimeTests.xml

.PARAMETER SkipPrepare
    Skip step 1 (useful when re-running after a previous prepare).

.EXAMPLE
    .\run-runtime-tests.ps1
    .\run-runtime-tests.ps1 -ScriptingBackend Mono
    .\run-runtime-tests.ps1 -UnityPath "C:\Program Files\Unity\Hub\Editor\6000.0.63f1\Editor\Unity.exe"
#>
param(
    [string]$UnityPath = "",
    [ValidateSet("IL2CPP", "Mono")]
    [string]$ScriptingBackend = "IL2CPP",
    [string]$ResultsFile = "TestResults\RuntimeTests.xml",
    [switch]$SkipPrepare
)

$ErrorActionPreference = "Stop"
$projectPath = $PSScriptRoot

# --- Locate Unity -----------------------------------------------------------
if (-not $UnityPath) {
    $versionLine = (Get-Content "$projectPath\ProjectSettings\ProjectVersion.txt" |
        Select-String "m_EditorVersion:").Line -replace "m_EditorVersion:\s*", ""

    $candidates = @(
        "C:\Program Files\Unity Editor Instalations\$versionLine\Editor\Unity.exe",
        "C:\Program Files\Unity\Hub\Editor\$versionLine\Editor\Unity.exe",
        "D:\Program Files\Unity\Hub\Editor\$versionLine\Editor\Unity.exe",
        "C:\Unity\$versionLine\Editor\Unity.exe"
    )

    foreach ($c in $candidates) {
        if (Test-Path -LiteralPath $c) {
            $UnityPath = $c
            break
        }
    }

    if (-not $UnityPath) {
        Write-Host "Could not auto-detect Unity $versionLine at:" -ForegroundColor Yellow
        $candidates | ForEach-Object { Write-Host "  $_" }
        Write-Host ""
        $UnityPath = Read-Host "Enter full path to Unity.exe"
    }
}

if (-not (Test-Path -LiteralPath $UnityPath)) {
    Write-Error "Unity.exe not found at: $UnityPath"
    exit 1
}

Write-Host "Unity:   $UnityPath"
Write-Host "Project: $projectPath"
Write-Host "Backend: $ScriptingBackend"
Write-Host "Results: $ResultsFile"
Write-Host ""

$resultsDir = Split-Path $ResultsFile -Parent
if ($resultsDir -and -not (Test-Path $resultsDir)) {
    New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null
}

$resultsFullPath = Join-Path $projectPath $ResultsFile

function Invoke-Unity {
    param([string[]]$Arguments)

    $proc = Start-Process -FilePath $UnityPath -ArgumentList $Arguments `
        -Wait -NoNewWindow -PassThru
    return $proc.ExitCode
}

# --- Step 1: Configure project ----------------------------------------------
if (-not $SkipPrepare) {
    if ($ScriptingBackend -eq "IL2CPP") {
        $prepareMethod = "StreamChat.EditorTools.StreamEditorTools.PrepareStandaloneIL2CPPTests"
    } else {
        $prepareMethod = "StreamChat.EditorTools.StreamEditorTools.EnableStreamTestsEnabledCompilerFlag"
    }

    Write-Host "=== Step 1: Preparing project ($prepareMethod) ==="
    $prepareLog = Join-Path $projectPath "TestResults\prepare.log"

    $exitCode = Invoke-Unity @(
        "-batchmode", "-nographics", "-quit",
        "-projectPath", $projectPath,
        "-executeMethod", $prepareMethod,
        "-logFile", $prepareLog
    )

    if ($exitCode -ne 0) {
        Write-Error "Prepare step failed (exit code $exitCode). See $prepareLog"
        exit $exitCode
    }
    Write-Host "Prepare complete."
    Write-Host ""
}

# --- Step 2: Build test player and run tests ---------------------------------
Write-Host "=== Step 2: Building and running runtime tests ==="
$testLog = Join-Path $projectPath "TestResults\test-run.log"

$exitCode = Invoke-Unity @(
    "-batchmode", "-nographics",
    "-projectPath", $projectPath,
    "-runTests",
    "-testPlatform", "StandaloneWindows64",
    "-testResults", $resultsFullPath,
    "-logFile", $testLog
)

# --- Report ------------------------------------------------------------------
Write-Host ""
if (Test-Path -LiteralPath $resultsFullPath) {
    [xml]$xml = Get-Content $resultsFullPath
    $suite = $xml."test-run"
    if ($suite) {
        Write-Host ("Results: total={0} passed={1} failed={2} skipped={3}" -f `
            $suite.total, $suite.passed, $suite.failed, $suite.skipped)
    }
} else {
    Write-Host "No results file produced. Check $testLog for details."
}

if ($exitCode -eq 0) {
    Write-Host "ALL TESTS PASSED" -ForegroundColor Green
} else {
    Write-Host "TESTS FAILED (exit code $exitCode)" -ForegroundColor Red
    Write-Host "Logs: $testLog"
}

exit $exitCode
