param(
    [Parameter(Mandatory = $true, HelpMessage = "Enter commit message")]
    [string]$CommitMessage
)

# Settings
$TestLog = "test_output.log"
$CommitLog = "commit_output.log"
$PushLog = "push_output.log"

# Colors for output
$ErrorColor = "Red"
$SuccessColor = "Green"
$InfoColor = "Yellow"
$WarningColor = "Magenta"

function Write-Info($message) {
    Write-Host "[INFO] $message" -ForegroundColor $InfoColor
}

function Write-Success($message) {
    Write-Host "[SUCCESS] $message" -ForegroundColor $SuccessColor
}

function Write-ErrorMsg($message) {
    Write-Host "[ERROR] $message" -ForegroundColor $ErrorColor
}

function Write-Warning($message) {
    Write-Host "[WARNING] $message" -ForegroundColor $WarningColor
}

function Test-InternetConnection {
    try {
        $ping = Test-Connection -ComputerName "8.8.8.8" -Count 1 -Quiet -ErrorAction Stop
        return $ping
    }
    catch {
        return $false
    }
}

function Cleanup-Logs {
    $logs = @($TestLog, $CommitLog, $PushLog)
    foreach ($log in $logs) {
        if (Test-Path $log) {
            Remove-Item $log -Force
        }
    }
}

# Main Script
Write-Host "`n=== Git Automation Script (PowerShell) ===" -ForegroundColor Cyan
Write-Host "Commit message: '$CommitMessage'" -ForegroundColor Gray
Write-Host "`n" -NoNewline

# 1. Run tests
try {
    Write-Info "Running tests..."
    dotnet test *>&1 | Tee-Object -FilePath $TestLog
    
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMsg "Tests failed"
        Write-Host "`nError details:" -ForegroundColor Gray
        Get-Content $TestLog
        Cleanup-Logs
        exit 1
    }
    
    Write-Success "Tests passed successfully"
}
catch {
    Write-ErrorMsg "Error running tests: $_"
    Cleanup-Logs
    exit 1
}

# 2. Add files to git
try {
    Write-Info "Adding files to git..."
    git add .
    
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMsg "Failed to add files to git"
        Cleanup-Logs
        exit 1
    }
    
    Write-Success "Files added successfully"
}
catch {
    Write-ErrorMsg "Error adding files: $_"
    Cleanup-Logs
    exit 1
}

# 3. Check internet connection
Write-Info "Checking internet connection..."

$hasInternet = Test-InternetConnection

if (-not $hasInternet) {
    Write-Warning "No internet connection available"
    $canPush = $false
}
else {
    Write-Success "Internet connection is available"
    $canPush = $true
}

# 4. Create commit
try {
    Write-Info "Creating commit..."
    git commit -m "$CommitMessage" *>&1 | Tee-Object -FilePath $CommitLog
    
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMsg "Failed to create commit"
        Write-Host "`nError details:" -ForegroundColor Gray
        Get-Content $CommitLog
        Cleanup-Logs
        exit 1
    }
    
    Write-Success "Commit created successfully"
    
    # Show commit info
    $commitHash = git rev-parse --short HEAD
    Write-Host "    Commit hash: $commitHash" -ForegroundColor Gray
}
catch {
    Write-ErrorMsg "Error creating commit: $_"
    Cleanup-Logs
    exit 1
}

# 5. Push (if internet available)
if ($canPush) {
    try {
        Write-Info "Pushing changes to remote..."
        git push | Tee-Object -FilePath $PushLog
        
        if ($LASTEXITCODE -ne 0) {
            Write-ErrorMsg "Failed to push changes"
            Write-Host "`nError details:" -ForegroundColor Gray
            Get-Content $PushLog
            Cleanup-Logs
            exit 1
        }
        
        Write-Success "Changes pushed successfully"
        
        # Show branch info
        $branch = git branch --show-current
        Write-Host "    Branch: $branch" -ForegroundColor Gray
    }
    catch {
        Write-ErrorMsg "Error pushing changes: $_"
        Cleanup-Logs
        exit 1
    }
}
else {
    Write-Warning "Push skipped - no internet connection"
    Write-Host "`nNote: Commit is saved locally." -ForegroundColor Gray
    Write-Host "Run 'git push' manually when internet connection is restored." -ForegroundColor Gray
}

# 6. Cleanup log files
Cleanup-Logs

Write-Host "`nScript completed successfully" -ForegroundColor Cyan