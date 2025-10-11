Write-Host "Starting lua scripts..." -ForegroundColor Green

$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$luaExePath = Join-Path $scriptDirectory "lua54.exe"
$testsPath = Join-Path $scriptDirectory "tests"
$luaunitPath = Join-Path $scriptDirectory "luaunit"

$modulesPath = Join-Path (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $scriptDirectory))) "SolidZip\src\SolidZip\modules"
$modulesPathLua = $modulesPath -replace '\\', '/'
$modulesSearchPath = "$modulesPathLua/?.lua;$modulesPathLua/logging/?.lua"


$luaunitPathLua = $luaunitPath -replace '\\', '/'

Get-ChildItem -Path $testsPath -Filter "*Tests.lua" | ForEach-Object {
    Write-Host "Running $($_.Name)..." -ForegroundColor Yellow
    & $luaExePath -e "package.path = package.path .. ';$luaunitPathLua/?.lua;$modulesSearchPath'" $_.FullName
}