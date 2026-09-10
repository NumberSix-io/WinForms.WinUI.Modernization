param([switch] $RunHostChecks)
$ErrorActionPreference = 'Stop'
$repositoryPath = Split-Path $PSScriptRoot -Parent
Push-Location $repositoryPath
try {
    foreach ($variant in @('Before', 'After')) {
        $solutionPath = Join-Path $variant "PlantOps.$variant.sln"
        & dotnet restore $solutionPath -p:Platform=x64 --locked-mode
        if ($LASTEXITCODE -ne 0) { throw "$variant restore failed" }
        & dotnet build $solutionPath --no-restore -c Release -p:Platform=x64
        if ($LASTEXITCODE -ne 0) { throw "$variant build failed" }
    }
    if ($RunHostChecks) {
        New-Item -ItemType Directory -Force artifacts | Out-Null
        $reportPath = Join-Path $repositoryPath 'artifacts\host-verification.txt'
        $executablePath = Join-Path $repositoryPath 'After\PlantOps\bin\x64\Release\net10.0-windows10.0.26100.0\win-x64\PlantOps.exe'
        $checkProcess = Start-Process -FilePath $executablePath -ArgumentList '--verify-host', ('"' + $reportPath + '"') -PassThru
        if (-not $checkProcess.WaitForExit(30000)) {
            Stop-Process -Id $checkProcess.Id
            throw 'Host verification timed out after 30 seconds'
        }
        if (Test-Path -LiteralPath $reportPath) { Get-Content -LiteralPath $reportPath }
        if ($checkProcess.ExitCode -ne 0) { throw "Host verification failed with exit code $($checkProcess.ExitCode)" }
    }
}
finally { Pop-Location }
