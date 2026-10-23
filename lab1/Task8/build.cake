var target        = Argument("target", "Pack");
var configuration = Argument("configuration", "Release");

var solution  = "./ZtmSolution.sln";
var toolProj  = "./ZtmGdansk/ZtmGdansk.csproj";
var artifacts = "./artifacts";

Task("Clean")
    .Does(() =>
{
    Information("Czyszczenie bin/, obj/ i {0}", artifacts);
    CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");
    CleanDirectory(artifacts);
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetBuild(solution, new DotNetBuildSettings
    {
        Configuration = configuration,
        NoRestore     = true
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetTest(solution, new DotNetTestSettings
    {
        Configuration = configuration,
        NoBuild       = true,
        NoRestore     = true
    });
});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{
    DotNetPack(toolProj, new DotNetPackSettings
    {
        Configuration   = configuration,
        NoBuild         = true,
        NoRestore       = true,
        OutputDirectory = artifacts
    });
});

RunTarget(target);