Param(
    [string]$Script = "build.cake",
    [string]$Target = "Default",
    [string]$Configuration = "Release",
    [string]$Verbosity = "Verbose",
    [bool]$Experimental = $false
)

$TOOLS_DIR = Join-Path $PSScriptRoot "tools"
$NUGET_EXE = Join-Path $TOOLS_DIR "nuget.exe"
$CAKE_EXE = Join-Path $TOOLS_DIR "Cake/Cake.exe"
$PACKAGES_CONFIG = Join-Path $TOOLS_DIR "packages.config"
$APPVEYOR_YML = Join-Path $PSScriptRoot "appveyor.yml"

# Make sure tools folder exists
if ((Test-Path $PSScriptRoot) -and !(Test-Path $TOOLS_DIR)) {
    New-Item -path $TOOLS_DIR -name logfiles -itemtype directory
}

# Try find NuGet.exe in path if not exists
if (!(Test-Path $NUGET_EXE)) {
    "Trying to find nuget.exe in path"
    $NUGET_EXE_IN_PATH = &where.exe nuget.exe
    if ($NUGET_EXE_IN_PATH -ne $null -and (Test-Path $NUGET_EXE_IN_PATH)) {
        "Found $($NUGET_EXE_IN_PATH)"
        $NUGET_EXE = $NUGET_EXE_IN_PATH
    }
}

# Try download NuGet.exe if not exists
if (!(Test-Path $NUGET_EXE)) {
    Invoke-WebRequest -Uri http://nuget.org/nuget.exe -OutFile $NUGET_EXE
}

# Make sure NuGet exists where we expect it.
if (!(Test-Path $NUGET_EXE)) {
    Throw "Could not find NuGet.exe"
}

# Save nuget.exe path to environment to be available to child processed
$ENV:NUGET_EXE = $NUGET_EXE

# Restore tools from NuGet.
Push-Location
Set-Location $TOOLS_DIR

# Restore packages
if (Test-Path $PACKAGES_CONFIG)
{
    Invoke-Expression "&`"$NUGET_EXE`" install -ExcludeVersion"
}
# Install just Cake if missing config
else
{
    Invoke-Expression "&`"$NUGET_EXE`" install Cake -ExcludeVersion"
}
Pop-Location
if ($LASTEXITCODE -ne 0)
{
    exit $LASTEXITCODE
}

# Make sure that Cake has been installed.
if (!(Test-Path $CAKE_EXE)) {
    Throw "Could not find Cake.exe"
}

# If default cake script not present download
if(($Script -eq "build.cake") -and !(Test-Path $Script))
{
    Invoke-WebRequest -Uri https://raw.githubusercontent.com/cake-build/bootstrapper/master/res/scripts/build.cake -OutFile $Script
}
$e
# Start Cake
Invoke-Expression "&`"$CAKE_EXE`" `"$Script`" -target=`"$Target`" -configuration=`"$Configuration`" -verbosity=`"$Verbosity`" $(if($Experimental -eq $true){" -experimental"})"
exit $LASTEXITCODE