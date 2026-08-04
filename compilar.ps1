param(
    [string]$Configuracion = "Release"
)

$ErrorActionPreference = "Stop"
$raiz = Split-Path -Parent $MyInvocation.MyCommand.Path
$publicar = Join-Path $raiz "publicar"

Write-Host "==> Compilando y publicando (single file, autocontenido)..."
dotnet publish (Join-Path $raiz "ClaammApp.UI\ClaammApp.UI.csproj") `
    -c $Configuracion `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -o $publicar

if ($LASTEXITCODE -ne 0) { throw "La publicacion fallo." }

Get-ChildItem -Path $publicar -Filter *.pdb | Remove-Item -Force
Remove-Item -Path (Join-Path $publicar "LatoFont") -Recurse -Force -ErrorAction SilentlyContinue
Get-ChildItem -Path $publicar | Select-Object Name, @{N='MB';E={[math]::Round($_.Length/1MB,1)}}

Write-Host ""
Write-Host "==> Verificando que 'publicar' este ignorado por git..."
git -C $raiz status --porcelain
Write-Host "==> Fin."
