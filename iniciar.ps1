param([switch]$SoloPreparar)

$ErrorActionPreference = 'Stop'
$previousEnvironment = $env:ASPNETCORE_ENVIRONMENT
$previousPassword = $env:Demo__Password
Push-Location $PSScriptRoot

try {
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    if ([string]::IsNullOrWhiteSpace($env:Demo__Password)) {
        $env:Demo__Password = 'CarritoDemo2026!'
    }

    dotnet tool restore
    if ($LASTEXITCODE -ne 0) { throw 'No se pudo restaurar dotnet-ef.' }

    dotnet build Shop.Api -c Release
    if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar la API.' }

    dotnet ef database update --project Shop.Api --configuration Release --no-build
    if ($LASTEXITCODE -ne 0) { throw 'No se pudo preparar la base. Revisa que SQL Server esté disponible y que la cuenta tenga permisos.' }

    dotnet run --project Shop.Api -c Release --no-build -- --seed-demo
    if ($LASTEXITCODE -ne 0) { throw 'No se pudieron cargar los datos de prueba.' }

    if (-not $SoloPreparar) {
        dotnet run --project Shop.Api -c Release --no-build
        if ($LASTEXITCODE -ne 0) { throw 'No se pudo iniciar la API.' }
    }
}
finally {
    $env:ASPNETCORE_ENVIRONMENT = $previousEnvironment
    $env:Demo__Password = $previousPassword
    Pop-Location
}
