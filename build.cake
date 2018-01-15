#addin "nuget:?package=Cake.Git&version=0.16.1"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var configuration = Argument("configuration", "Debug");
var revision = EnvironmentVariable("BUILD_NUMBER") ?? Argument("revision", "9999");
var target = Argument("target", "Default");


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define git commit id
var commitId = "SNAPSHOT";

// Define product name and version
var product = "Htc.Vita.External.DX9.Redist";
var companyName = "HTC";
var version = "9.0.201006";
var semanticVersion = string.Format("{0}.{1}", version, revision);
var ciVersion = string.Format("{0}.{1}", version, "0");
var nugetTags = new [] {"htc", "vita", "external", "dx9", "redist"};
var projectUrl = "https://github.com/ViveportSoftware/vita_external_dx9_redist/";
var description = "HTC Vita external package: DX9 redistributable";

// Define copyright
var copyright = string.Format("Copyright © 2018 - {0}", DateTime.Now.Year);

// Define timestamp for signing
var lastSignTimestamp = DateTime.Now;
var signIntervalInMilli = 1000 * 5;

// Define path
var targetFileName = "directx_Jun2010_redist.exe";
var targetFileUrl = "https://download.microsoft.com/download/8/4/A/84A35BF1-DAFE-4AE8-82AF-AD2AE20B6B14/directx_Jun2010_redist.exe";

// Define directories.
var distDir = Directory("./dist");
var tempDir = Directory("./temp");
var nugetDir = distDir + Directory(configuration) + Directory("nuget");
var tempOutputDir = tempDir + Directory(configuration);


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Fetch-Git-Commit-ID")
    .ContinueOnError()
    .Does(() =>
{
    var lastCommit = GitLogTip(MakeAbsolute(Directory(".")));
    commitId = lastCommit.Sha;
});

Task("Display-Config")
    .IsDependentOn("Fetch-Git-Commit-ID")
    .Does(() =>
{
    Information("Build target: {0}", target);
    Information("Build configuration: {0}", configuration);
    Information("Build commitId: {0}", commitId);
    if ("Release".Equals(configuration))
    {
        Information("Build version: {0}", semanticVersion);
    }
    else
    {
        Information("Build version: {0}-CI{1}", ciVersion, revision);
    }
});

Task("Clean-Workspace")
    .IsDependentOn("Display-Config")
    .Does(() =>
{
    CleanDirectory(distDir);
    CleanDirectory(tempDir);
});

Task("Download-Dependent-Binaries")
    .IsDependentOn("Clean-Workspace")
    .Does(() =>
{

    var targetFile = tempOutputDir + File(targetFileName);
    var binaryDir = tempOutputDir + Directory("binary");
    var args = string.Format("x {0} -o{1}", targetFile, binaryDir);

    CreateDirectory(tempOutputDir);
    CreateDirectory(binaryDir);
    DownloadFile(targetFileUrl, targetFile);

    using(var process = StartAndReturnProcess("7z", new ProcessSettings{ Arguments = args }))
    {
        process.WaitForExit();
        // This should output 0 as valid arguments supplied
        Information("Exit code: {0}", process.GetExitCode());
    }
});

Task("Build-NuGet-Package-Apr2005")
    .IsDependentOn("Download-Dependent-Binaries")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Apr2005",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Apr2005_d3dx9_25_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Apr2005_d3dx9_25_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Apr2006")
    .IsDependentOn("Build-NuGet-Package-Apr2005")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Apr2006",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Apr2006_d3dx9_30_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Apr2006_d3dx9_30_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Apr2006_MDX1_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Apr2006_MDX1_x86_Archive.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Apr2006_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Apr2006_XACT_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Apr2006_xinput_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Apr2006_xinput_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Apr2007")
    .IsDependentOn("Build-NuGet-Package-Apr2006")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Apr2007",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "APR2007_d3dx9_33_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "APR2007_d3dx9_33_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "APR2007_d3dx10_33_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "APR2007_d3dx10_33_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "APR2007_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "APR2007_XACT_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "APR2007_xinput_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "APR2007_xinput_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Aug2005")
    .IsDependentOn("Build-NuGet-Package-Apr2007")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Aug2005",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Aug2005_d3dx9_27_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2005_d3dx9_27_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Aug2006")
    .IsDependentOn("Build-NuGet-Package-Aug2005")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Aug2006",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "AUG2006_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "AUG2006_XACT_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "AUG2006_xinput_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "AUG2006_xinput_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Aug2007")
    .IsDependentOn("Build-NuGet-Package-Aug2006")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Aug2007",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "AUG2007_d3dx9_35_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "AUG2007_d3dx9_35_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "AUG2007_d3dx10_35_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "AUG2007_d3dx10_35_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "AUG2007_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "AUG2007_XACT_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Aug2008")
    .IsDependentOn("Build-NuGet-Package-Aug2007")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Aug2008",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Aug2008_d3dx9_39_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2008_d3dx9_39_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2008_d3dx10_39_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2008_d3dx10_39_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2008_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2008_XACT_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2008_XAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2008_XAudio_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Aug2009")
    .IsDependentOn("Build-NuGet-Package-Aug2008")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Aug2009",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Aug2009_D3DCompiler_42_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_D3DCompiler_42_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_d3dcsx_42_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_d3dcsx_42_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_d3dx9_42_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_d3dx9_42_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_d3dx10_42_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_d3dx10_42_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_d3dx11_42_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_d3dx11_42_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_XACT_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_XAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Aug2009_XAudio_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Dec2005")
    .IsDependentOn("Build-NuGet-Package-Aug2009")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Dec2005",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Dec2005_d3dx9_28_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Dec2005_d3dx9_28_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Dec2006")
    .IsDependentOn("Build-NuGet-Package-Dec2005")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Dec2006",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "DEC2006_d3dx9_32_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "DEC2006_d3dx9_32_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "DEC2006_d3dx10_00_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "DEC2006_d3dx10_00_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "DEC2006_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "DEC2006_XACT_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-DxSetup")
    .IsDependentOn("Build-NuGet-Package-Dec2006")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".DxSetup",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "DSETUP.dll",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "dsetup32.dll",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "dxdllreg_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "DXSETUP.exe",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "dxupdate.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Feb2005")
    .IsDependentOn("Build-NuGet-Package-DxSetup")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Feb2005",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Feb2005_d3dx9_24_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Feb2005_d3dx9_24_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Feb2006")
    .IsDependentOn("Build-NuGet-Package-Feb2005")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Feb2006",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Feb2006_d3dx9_29_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Feb2006_d3dx9_29_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Feb2006_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Feb2006_XACT_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Feb2007")
    .IsDependentOn("Build-NuGet-Package-Feb2006")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Feb2007",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "FEB2007_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "FEB2007_XACT_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Feb2010")
    .IsDependentOn("Build-NuGet-Package-Feb2007")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Feb2010",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Feb2010_X3DAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Feb2010_X3DAudio_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Feb2010_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Feb2010_XACT_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Feb2010_XAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Feb2010_XAudio_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Jun2005")
    .IsDependentOn("Build-NuGet-Package-Feb2010")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Jun2005",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Jun2005_d3dx9_26_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2005_d3dx9_26_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Jun2006")
    .IsDependentOn("Build-NuGet-Package-Jun2005")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Jun2006",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "JUN2006_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2006_XACT_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Jun2007")
    .IsDependentOn("Build-NuGet-Package-Jun2006")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Jun2007",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "JUN2007_d3dx9_34_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2007_d3dx9_34_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2007_d3dx10_34_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2007_d3dx10_34_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2007_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2007_XACT_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Jun2008")
    .IsDependentOn("Build-NuGet-Package-Jun2007")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Jun2008",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "JUN2008_d3dx9_38_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2008_d3dx9_38_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2008_d3dx10_38_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2008_d3dx10_38_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2008_X3DAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2008_X3DAudio_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2008_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2008_XACT_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2008_XAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "JUN2008_XAudio_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Jun2010")
    .IsDependentOn("Build-NuGet-Package-Jun2008")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Jun2010",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Jun2010_D3DCompiler_43_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_D3DCompiler_43_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_d3dcsx_43_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_d3dcsx_43_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_d3dx9_43_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_d3dx9_43_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_d3dx10_43_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_d3dx10_43_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_d3dx11_43_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_d3dx11_43_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_XACT_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_XAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Jun2010_XAudio_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Mar2008")
    .IsDependentOn("Build-NuGet-Package-Jun2010")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Mar2008",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Mar2008_d3dx9_37_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2008_d3dx9_37_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2008_d3dx10_37_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2008_d3dx10_37_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2008_X3DAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2008_X3DAudio_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2008_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2008_XACT_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2008_XAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2008_XAudio_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Mar2009")
    .IsDependentOn("Build-NuGet-Package-Mar2008")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Mar2009",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Mar2009_d3dx9_41_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2009_d3dx9_41_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2009_d3dx10_41_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2009_d3dx10_41_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2009_X3DAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2009_X3DAudio_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2009_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2009_XACT_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2009_XAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Mar2009_XAudio_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Nov2007")
    .IsDependentOn("Build-NuGet-Package-Mar2009")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Nov2007",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Nov2007_d3dx9_36_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Nov2007_d3dx9_36_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Nov2007_d3dx10_36_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Nov2007_d3dx10_36_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "NOV2007_X3DAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "NOV2007_X3DAudio_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "NOV2007_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "NOV2007_XACT_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Nov2008")
    .IsDependentOn("Build-NuGet-Package-Nov2007")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Nov2008",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Nov2008_d3dx9_40_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Nov2008_d3dx9_40_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Nov2008_d3dx10_40_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Nov2008_d3dx10_40_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Nov2008_X3DAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Nov2008_X3DAudio_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Nov2008_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Nov2008_XACT_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Nov2008_XAudio_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Nov2008_XAudio_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Oct2005")
    .IsDependentOn("Build-NuGet-Package-Nov2008")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Oct2005",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "Oct2005_xinput_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "Oct2005_xinput_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});

Task("Build-NuGet-Package-Oct2006")
    .IsDependentOn("Build-NuGet-Package-Oct2005")
    .Does(() =>
{
    CreateDirectory(nugetDir);
    var nugetPackVersion = semanticVersion;
    if (!"Release".Equals(configuration))
    {
        nugetPackVersion = string.Format("{0}-CI{1}", ciVersion, revision);
    }
    Information("Pack version: {0}", nugetPackVersion);
    var nuGetPackSettings = new NuGetPackSettings
    {
            Id = product + ".Oct2006",
            Version = nugetPackVersion,
            Authors = new[] {"HTC"},
            Description = description + " [CommitId: " + commitId + "]",
            Copyright = copyright,
            ProjectUrl = new Uri(projectUrl),
            Tags = nugetTags,
            RequireLicenseAcceptance= false,
            Files = new []
            {
                    new NuSpecContent
                    {
                            Source = "OCT2006_d3dx9_31_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "OCT2006_d3dx9_31_x86.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "OCT2006_XACT_x64.cab",
                            Target = "content"
                    },
                    new NuSpecContent
                    {
                            Source = "OCT2006_XACT_x86.cab",
                            Target = "content"
                    }
            },
            Properties = new Dictionary<string, string>
            {
                    {"Configuration", configuration}
            },
            BasePath = tempOutputDir + Directory("binary"),
            OutputDirectory = nugetDir
    };

    NuGetPack(nuGetPackSettings);
});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build-NuGet-Package-Oct2006");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
