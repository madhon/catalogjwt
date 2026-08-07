# scripts/generate-jwt-ecdsa.ps1
# Requires: OpenSSL on PATH, or use the .NET fallback below.

$ErrorActionPreference = "Stop"
$outDir = Join-Path $PSScriptRoot "keys"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

$privatePath = Join-Path $outDir "private.pem"
$publicPath  = Join-Path $outDir "public.pem"

openssl ecparam -name prime256v1 -genkey -noout -out $privatePath
openssl ec -in $privatePath -pubout -out $publicPath

function ConvertTo-JsonPemLine([string]$Path) {
    # Normalize newlines, trim trailing whitespace, escape as \n for JSON/user-secrets
    $pem = (Get-Content -Path $Path -Raw).Trim() -replace "`r`n", "`n" -replace "`r", "`n"
    return ($pem -replace "`n", "\n")
}

$privateLine = ConvertTo-JsonPemLine $privatePath
$publicLine  = ConvertTo-JsonPemLine $publicPath

Write-Host ""
Write-Host "=== Auth: jwt:PrivateKeyPem (single line) ===" -ForegroundColor Cyan
Write-Host $privateLine
Write-Host ""
Write-Host "=== API/Gateway: PublicKeyPem (single line) ===" -ForegroundColor Cyan
Write-Host $publicLine
Write-Host ""
Write-Host "=== appsettings.json fragment ===" -ForegroundColor Green
Write-Host @"
"privateKeyPem": "$privateLine",
"publicKeyPem": "$publicLine"
"@
Write-Host ""
Write-Host "=== user-secrets commands (Auth) ===" -ForegroundColor Green
Write-Host "dotnet user-secrets set `"jwt:PrivateKeyPem`" `"$privateLine`" --project Catalog.Auth"
Write-Host "dotnet user-secrets set `"jwt:PublicKeyPem`" `"$publicLine`" --project Catalog.Auth"
Write-Host ""
Write-Host "PEM files also written to: $outDir" -ForegroundColor DarkGray