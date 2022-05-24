$sut_path = "$PSScriptRoot/../Queued.WebApi"
$api_ready_status_url = "http://localhost:5000/work"
$api_ready_status_max_attempts = 10

# =============================================================================
# Functions
# =============================================================================

function Get-ApiStatus {
    try {
        $response = Invoke-RestMethod -Uri $api_ready_status_url
        if ($response -eq "")
        {
            return "ok";
        }
    }
    catch [System.Net.Http.HttpRequestException] {
        return "retry";
    }
    return "failed"
}

function Get-ApiReadyStatus {
    $counter = 0
    $status = Get-ApiStatus
    while ($status -eq "retry" -and $counter++ -lt $api_ready_status_max_attempts) {
        Write-Output "waiting api ready status..."
        Start-Sleep -Milliseconds 500
        $status = Get-ApiStatus
    }
    return $status
}

# =============================================================================
# Main
# =============================================================================

# build api
Write-Output "building api..."
$build = Start-Process -FilePath "dotnet" -ArgumentList "build" -WorkingDirectory "$sut_path" -PassThru -RedirectStandardOutput "__build.log" -Wait

# evaluate success/failure
if($build.ExitCode -eq 0)
{
    Write-Output "build success..."
    # Success
}
else
{
    # Failure
    Write-Error "Could not build api"
    return
}

# start api
Write-Output "starting api..."
$sut_pid = Start-Process -FilePath "dotnet" -ArgumentList "run" -WorkingDirectory "$sut_path" -PassThru -RedirectStandardOutput "__output.log"

# wait ready status
Write-Output "waiting api ready status..."
$status = Get-ApiReadyStatus
if ($status -eq "failed")
{
    Write-Error "Could not start api"
    return
}

# wait key press to close api, so the api has time to process works
Write-Output "api is ready, go test it!"
Write-Host -NoNewLine 'Press any key to continue...';
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
Stop-Process $sut_pid.Id 
