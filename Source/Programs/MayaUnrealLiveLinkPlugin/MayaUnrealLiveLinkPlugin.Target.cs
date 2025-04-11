// MIT License

// Copyright (c) 2022 Autodesk, Inc.

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnrealBuildTool;
using System;
using System.IO;
using EpicGames.Core;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


[SupportedPlatforms("Win64", "Linux")]
public class MayaUnrealLiveLinkPluginTarget : TargetRules
{
	/// <summary>
	/// Finds the innermost parent directory with the provided name. Search is case insensitive.
	/// </summary>
	string InnermostParentDirectoryPathWithName(string ParentName, string CurrentPath)
	{
		DirectoryInfo ParentInfo = Directory.GetParent(CurrentPath);

		if (ParentInfo == null)
		{
			throw new DirectoryNotFoundException("Could not find parent folder '" + ParentName + "'");
		}

		// Case-insensitive check of the parent folder name.
		if (ParentInfo.Name.ToLower() == ParentName.ToLower())
		{
			return ParentInfo.ToString();
		}

		return InnermostParentDirectoryPathWithName(ParentName, ParentInfo.ToString());
	}

	/// <summary>
	/// Returns the path to this .cs file.
	/// </summary>
	string GetCallerFilePath([CallerFilePath] string CallerFilePath = "")
	{
		if (CallerFilePath.Length == 0)
		{
			throw new FileNotFoundException("Could not find the path of our .cs file");
		}

		return CallerFilePath;
	}

	public MayaUnrealLiveLinkPluginTarget(TargetInfo Target) : base(Target)
	{
		Init(Target, "");
	}

	public MayaUnrealLiveLinkPluginTarget(TargetInfo Target, string InMayaVersionString) : base(Target)
	{
		Init(Target, InMayaVersionString);
	}
	
	private void Init(TargetInfo Target, string InMayaVersionString)
	{
		Type = TargetType.Program; // 타겟 타입을 프로그램으로 설정합니다 (일반적인 DLL 플러그인이 아닌 독립 실행 가능한 프로그램 또는 라이브러리를 빌드하는 데 사용됩니다).
		IncludeOrderVersion = EngineIncludeOrderVersion.Latest;  // 엔진 헤더 파일 포함 순서를 최신 버전으로 설정합니다.
		bShouldCompileAsDLL = true;  // DLL 형태로 컴파일하도록 설정합니다.
		LinkType = TargetLinkType.Monolithic;  // 모든 코드를 하나의 DLL로 링크하도록 설정합니다.
		SolutionDirectory = "Programs/LiveLink";  // Visual Studio 솔루션 내에서 이 프로젝트가 속할 폴더를 지정합니다.
		string MllName = "MayaUnrealLiveLinkPlugin";  // 기본 플러그인 이름("MayaUnrealLiveLinkPlugin")을 설정합니다.
		LaunchModuleName = MllName + InMayaVersionString;  // 실행 모듈 이름을 기본 플러그인 이름에 마야 버전 문자열을 붙여 설정합니다. MayaUnrealLiveLinkPlugin2023

		ReadOnlyBuildVersion Version = ReadOnlyBuildVersion.Current;  // 현재 언리얼 엔진 빌드 버전을 가져옵니다.
		string UEVersion = Version.MajorVersion.ToString()+"_"+Version.MinorVersion.ToString(); // 언리얼 엔진 메이저 및 마이너 버전을 문자열 형태로 조합합니다.
		Name             = MllName + "_" + UEVersion;  // MayaUnrealLiveLinkPlugin_5_4

		// We only need minimal use of the engine for this plugin
		bBuildDeveloperTools = false;  // 개발자 툴 관련 기능을 빌드하지 않도록 설정합니다.
		bUseMallocProfiler = false;  // 메모리 할당 프로파일러를 사용하지 않도록 설정합니다.
		bBuildWithEditorOnlyData = true;  // 에디터 전용 데이터를 포함하여 빌드하도록 설정합니다.
		bCompileAgainstEngine = false;  // 엔진 코드를 직접적으로 컴파일하지 않도록 설정합니다.
		bCompileAgainstCoreUObject = true;  // CoreUObject 모듈에 대해 컴파일하도록 설정합니다 (언리얼 엔진의 핵심 오브젝트 시스템).
		bCompileAgainstApplicationCore = false;  // ApplicationCore 모듈에 대해 컴파일하지 않도록 설정합니다 (애플리케이션 레벨 기능을 제공).
		bCompileICU = false;  // ICU (International Components for Unicode) 라이브러리를 컴파일하지 않도록 설정합니다.
		bHasExports = true;  // DLL이 익스포트(외부에 공개)할 심볼을 가지고 있음을 나타냅니다.
		

		bBuildInSolutionByDefault = false;  // 기본적으로 솔루션에 포함하여 빌드하지 않도록 설정합니다.
		bUseAdaptiveUnityBuild = false;  // 어댑티브 유니티 빌드(UnityBuild)를 사용하지 않도록 설정합니다 (컴파일 시간 최적화 기술).

		// This .cs file must be inside the source folder of this Program. We later use this to find other key directories.
		string TargetFilePath = GetCallerFilePath();

		// We need to avoid failing to load DLL due to looking for EngineDir() in non-existent folders.
		// By having it build in the same directory as the engine, it will assume the engine is in the same directory
		// as the program, and because this folder always exists, it will not fail the check inside EngineDir().

		// Because this is a Program, we assume that this target file resides under a "Programs" folder.
		string ProgramsDir = InnermostParentDirectoryPathWithName("Programs", TargetFilePath);

		// We assume this Program resides under a Source folder.
		string SourceDir = InnermostParentDirectoryPathWithName("Source", ProgramsDir);

		// The program is assumed to reside inside the "Engine" folder.
		string EngineDir = InnermostParentDirectoryPathWithName("Engine", SourceDir);

		// The default Binaries path is assumed to be a sibling of "Source" folder.
		string DefaultBinDir = Path.GetFullPath(Path.Combine(SourceDir, "..", "Binaries", Platform.ToString()));

		// We assume that the engine exe resides in Engine/Binaries/[Platform]
		string EngineBinariesDir = Path.Combine(EngineDir, "Binaries", Platform.ToString(), "Maya", InMayaVersionString);

		// Now we calculate the relative path between the default output directory and the engine binaries,
		// 이제 기본 출력 디렉토리와 엔진 바이너리 간의 상대 경로를 계산합니다,
		// in order to force the output of this program to be in the same folder as the engine.
		// 이 프로그램의 출력을 엔진과 동일한 폴더에 강제하기 위해.
		ExeBinariesSubFolder = (new DirectoryReference(EngineBinariesDir)).MakeRelativeTo(new DirectoryReference(DefaultBinDir));

		// Setting this is necessary since we are creating the binaries outside of Restricted.
		bLegalToDistributeBinary = true;

		// We still need to copy the resources, so at this point we might as well copy the files where the default Binaries folder was meant to be.
		// MayaUnrealLiveLinkPlugin.xml will be unaware of how the files got there.

		// Add a post-build step that copies the output to a file with the .mll extension
		// .ml 확장자가 있는 파일에 출력을 복사하는 빌드 후 단계를 추가합니다
		string OutputName = Name; // MayaUnrealLiveLinkPlugin_<UEVersion> // MayaUnrealLiveLinkPlugin_5_4 인듯

		// Development 이 기본이라 여긴 패스일듯
			if (Target.Configuration != UnrealTargetConfiguration.Development)
			{
				OutputName = string.Format("{0}-{1}-{2}", OutputName, Target.Platform, Target.Configuration);
				MllName = string.Format("{0}-{1}-{2}", MllName, Target.Platform, Target.Configuration);
			}

		string PostBuildBinDir = Path.Combine(DefaultBinDir, "Maya", InMayaVersionString); // 빌드 후 복사할 대상 바이너리 디렉토리를 설정합니다.

		// 윈도우니 여기도 패스
			bool IsLinux = System.Environment.OSVersion.Platform.ToString() == "Unix";

			if (Target.Platform == UnrealTargetPlatform.Linux) {
				OutputName = "lib" + OutputName;
				MllName = "lib" + MllName;
			}

		// Fix Script with right version of UE
		DirectoryInfo SourcePath = Directory.GetParent(TargetFilePath);  // 현재 Target.cs 파일이 있는 폴더 경로를 가져옵니다.
		string text = File.ReadAllText(SourcePath.ToString()+"/MayaUnrealLiveLinkPluginUI.py.in");  // 템플릿 Python UI 스크립트(cpp 레퍼?) 파일을 읽어옵니다.
		text = text.Replace("@UNREAL5_ENGINE_VERSION@", UEVersion);  // 템플릿에서 언리얼 엔진 버전 플레이스홀더를 실제 버전으로 치환합니다.
		File.WriteAllText(SourcePath.ToString()+"/MayaUnrealLiveLinkPluginUI.py", text);  // 수정된 Python UI 스크립트 파일을 저장합니다.

		// 빌드 완료 후 실행할 명령들을 추가합니다.
		// Copy binaries
		PostBuildSteps.Add(string.Format("echo Copying {0} to {1}...", EngineBinariesDir, PostBuildBinDir)); 

		if (!IsLinux)
		{
			PostBuildSteps.Add(string.Format("xcopy /y /i /v \"{0}\\{1}.*\" \"{2}\\{3}.*\" 1>nul", EngineBinariesDir, OutputName, PostBuildBinDir, OutputName));
		}
		else
		{
			PostBuildSteps.Add(string.Format("mkdir -p \"{2}\" && rm -f {2}/{1}* && cp \"{0}\"/\"{1}\".* \"{2}\" 1>nul", EngineBinariesDir, OutputName, PostBuildBinDir));
		}

		string SourceFileName = Path.Combine(PostBuildBinDir, OutputName);
		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			// Rename dll as mll
			string OutputFileName = Path.Combine(PostBuildBinDir, MllName + "_" + UEVersion);

			PostBuildSteps.Add(string.Format("echo Renaming {0}.dll to {1}.mll...", SourceFileName, OutputFileName)); 
			PostBuildSteps.Add(string.Format("move /Y \"{0}.dll\" \"{1}.mll\" 1>nul", SourceFileName, OutputFileName));  // Windows에서는 복사된 DLL 파일의 확장자를 .mll로 변경합니다 (마야 플러그인 확장자).
		} else if (Target.Configuration != UnrealTargetConfiguration.Development)
		{
			// For Linux debug builds, create symbolic link when library name is different
			string OutputFileName = Path.Combine(PostBuildBinDir, "lib" + Name);

			PostBuildSteps.Add(string.Format("echo \"Creating symbolic links {1}.* to {0}.*...\"", SourceFileName, OutputFileName));
			PostBuildSteps.Add(string.Format("ln -sf \"{0}.{2}\" \"{1}.{2}\" 1>nul", SourceFileName, OutputFileName,"so"));
			PostBuildSteps.Add(string.Format("ln -sf \"{0}.{2}\" \"{1}.{2}\" 1>nul", SourceFileName, OutputFileName,"debug"));
			PostBuildSteps.Add(string.Format("ln -sf \"{0}.{2}\" \"{1}.{2}\" 1>nul", SourceFileName, OutputFileName,"sym"));
		}
	}
}
