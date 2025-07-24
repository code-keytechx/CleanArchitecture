# Project Analysis Report
Generated on: 7/24/2025, 1:55:30 PM
Project Path: /Users/quang.vuong/Documents/Development/CleanArchitecture
Repository Root: /Users/quang.vuong/Documents/Development/CleanArchitecture

## 📋 Project Overview
- **Name:** CleanArchitecture
- **Version:** unknown
- **Project Path:** /Users/quang.vuong/Documents/Development/CleanArchitecture
- **Repository Root:** /Users/quang.vuong/Documents/Development/CleanArchitecture
- **Primary Language:** JavaScript
- **Total Files:** 1175
- **Target Framework:** net9.0
- **SDK Version:** 9.0.100
- **Project Type:** Console Application
- **Output Type:** Exe
- **Modern .NET:** Yes (Core/.NET 5+)

## 🔤 Languages Detected
- **JavaScript:** 301 files
- **C#:** 154 files
- **JSON:** 72 files
- **TypeScript:** 22 files
- **HTML:** 15 files
- **CSS:** 12 files
- **YAML:** 10 files
- **Shell Script:** 10 files
- **PowerShell:** 7 files
- **SCSS:** 3 files

## 📚 Dependencies
## 🛠️ Development Dependencies
## ⚙️ Configuration Files
- .gitignore
- README.md
- LICENSE

## 🧪 Testing Information
### Test Folders (9)
| Folder Name | Path | Type | Files |
|-------------|------|------|-------|
| SentraUnitTests | SentraUnitTests | Unit | 2 |
| UnitTests | SentraUnitTests/UnitTests | Unit | 0 |
| testing | infra/core/testing | General Testing | 1 |
| tests | tests | General Testing | 0 |
| Application.FunctionalTests | tests/Application.FunctionalTests | General Testing | 14 |
| Application.UnitTests | tests/Application.UnitTests | Unit | 1 |
| Domain.UnitTests | tests/Domain.UnitTests | Unit | 1 |
| Infrastructure.IntegrationTests | tests/Infrastructure.IntegrationTests | E2E/Integration | 2 |
| Web.AcceptanceTests | tests/Web.AcceptanceTests | General Testing | 4 |

### Test Files Summary
- **Total Test Files:** 757

### Test Files by Directory
**src/Web/ClientApp/src/api-authorization** (1 files)
  - authorize.interceptor.spec.ts

**src/Web/ClientApp/src/app/counter** (1 files)
  - counter.component.spec.ts

**src/Web/ClientApp** (1 files)
  - tsconfig.spec.json

**src/Web/ClientApp-React/src** (1 files)
  - App.test.js

**tests/Application.FunctionalTests** (14 files)
  - Application.FunctionalTests.csproj
  - BaseTestFixture.cs
  - CustomWebApplicationFactory.cs
  - GlobalUsings.cs
  - ITestDatabase.cs
  - PostgreSQLTestDatabase.cs
  - PostgreSQLTestcontainersTestDatabase.cs
  - SqlTestDatabase.cs
  - SqlTestcontainersTestDatabase.cs
  - SqliteTestDatabase.cs
  - ... and 4 more files

**tests/Application.FunctionalTests/TodoItems/Commands** (4 files)
  - CreateTodoItemTests.cs
  - DeleteTodoItemTests.cs
  - UpdateTodoItemDetailTests.cs
  - UpdateTodoItemTests.cs

**tests/Application.FunctionalTests/TodoLists/Commands** (4 files)
  - CreateTodoListTests.cs
  - DeleteTodoListTests.cs
  - PurgeTodoListsTests.cs
  - UpdateTodoListTests.cs

**tests/Application.FunctionalTests/TodoLists/Queries** (1 files)
  - GetTodosTests.cs

**tests/Application.FunctionalTests/obj** (5 files)
  - Application.FunctionalTests.csproj.nuget.dgspec.json
  - Application.FunctionalTests.csproj.nuget.g.props
  - Application.FunctionalTests.csproj.nuget.g.targets
  - project.assets.json
  - project.nuget.cache

**tests/Application.FunctionalTests/obj/Debug/net9.0** (3 files)
  - Application.FunctionalTests.GlobalUsings.g.cs
  - Application.FunctionalTests.assets.cache
  - Application.FunctionalTests.csproj.FileListAbsolute.txt

**tests/Application.UnitTests** (1 files)
  - Application.UnitTests.csproj

**tests/Application.UnitTests/Common/Behaviours** (1 files)
  - RequestLoggerTests.cs

**tests/Application.UnitTests/Common/Exceptions** (1 files)
  - ValidationExceptionTests.cs

**tests/Application.UnitTests/Common/Mappings** (1 files)
  - MappingTests.cs

**tests/Application.UnitTests/obj** (5 files)
  - Application.UnitTests.csproj.nuget.dgspec.json
  - Application.UnitTests.csproj.nuget.g.props
  - Application.UnitTests.csproj.nuget.g.targets
  - project.assets.json
  - project.nuget.cache

**tests/Application.UnitTests/obj/Debug/net9.0** (3 files)
  - Application.UnitTests.GlobalUsings.g.cs
  - Application.UnitTests.assets.cache
  - Application.UnitTests.csproj.FileListAbsolute.txt

**tests/Domain.UnitTests** (1 files)
  - Domain.UnitTests.csproj

**tests/Domain.UnitTests/ValueObjects** (1 files)
  - ColourTests.cs

**tests/Domain.UnitTests/bin/Debug/net9.0** (31 files)
  - .msCoverageSourceRootsMapping_CleanArchitecture.Domain.UnitTests
  - CleanArchitecture.Domain.UnitTests.deps.json
  - CleanArchitecture.Domain.UnitTests.dll
  - CleanArchitecture.Domain.UnitTests.pdb
  - CleanArchitecture.Domain.UnitTests.runtimeconfig.json
  - CleanArchitecture.Domain.dll
  - CleanArchitecture.Domain.pdb
  - CoverletSourceRootsMapping_CleanArchitecture.Domain.UnitTests
  - FluentAssertions.dll
  - MediatR.Contracts.dll
  - ... and 21 more files

**tests/Domain.UnitTests/bin/Debug/net9.0/cs** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/de** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/es** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/fr** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/it** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/ja** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/ko** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/pl** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/pt-BR** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/ru** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/runtimes/win/lib/netstandard2.0** (1 files)
  - System.Security.Cryptography.ProtectedData.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/tr** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/zh-Hans** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/bin/Debug/net9.0/zh-Hant** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Domain.UnitTests/obj/Debug/net9.0** (14 files)
  - .NETCoreApp,Version=v9.0.AssemblyAttributes.cs
  - CleanArchitecture.Domain.UnitTests.dll
  - CleanArchitecture.Domain.UnitTests.pdb
  - Domain.U.77BC3ED9.Up2Date
  - Domain.UnitTests.AssemblyInfo.cs
  - Domain.UnitTests.AssemblyInfoInputs.cache
  - Domain.UnitTests.GeneratedMSBuildEditorConfig.editorconfig
  - Domain.UnitTests.GlobalUsings.g.cs
  - Domain.UnitTests.assets.cache
  - Domain.UnitTests.csproj.AssemblyReference.cache
  - ... and 4 more files

**tests/Domain.UnitTests/obj/Debug/net9.0/ref** (1 files)
  - CleanArchitecture.Domain.UnitTests.dll

**tests/Domain.UnitTests/obj/Debug/net9.0/refint** (1 files)
  - CleanArchitecture.Domain.UnitTests.dll

**tests/Domain.UnitTests/obj** (5 files)
  - Domain.UnitTests.csproj.nuget.dgspec.json
  - Domain.UnitTests.csproj.nuget.g.props
  - Domain.UnitTests.csproj.nuget.g.targets
  - project.assets.json
  - project.nuget.cache

**tests/Infrastructure.IntegrationTests** (2 files)
  - GlobalUsings.cs
  - Infrastructure.IntegrationTests.csproj

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0** (21 files)
  - CleanArchitecture.Infrastructure.IntegrationTests.deps.json
  - CleanArchitecture.Infrastructure.IntegrationTests.dll
  - CleanArchitecture.Infrastructure.IntegrationTests.pdb
  - CleanArchitecture.Infrastructure.IntegrationTests.runtimeconfig.json
  - Microsoft.TestPlatform.CommunicationUtilities.dll
  - Microsoft.TestPlatform.CoreUtilities.dll
  - Microsoft.TestPlatform.CrossPlatEngine.dll
  - Microsoft.TestPlatform.PlatformAbstractions.dll
  - Microsoft.TestPlatform.Utilities.dll
  - Microsoft.VisualStudio.CodeCoverage.Shim.dll
  - ... and 11 more files

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/cs** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/de** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/es** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/fr** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/it** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/ja** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/ko** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/pl** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/pt-BR** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/ru** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/tr** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/zh-Hans** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/bin/Debug/net9.0/zh-Hant** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Infrastructure.IntegrationTests/obj/Debug/net9.0** (14 files)
  - .NETCoreApp,Version=v9.0.AssemblyAttributes.cs
  - CleanArchitecture.Infrastructure.IntegrationTests.dll
  - CleanArchitecture.Infrastructure.IntegrationTests.pdb
  - Infrastr.0624B3B4.Up2Date
  - Infrastructure.IntegrationTests.AssemblyInfo.cs
  - Infrastructure.IntegrationTests.AssemblyInfoInputs.cache
  - Infrastructure.IntegrationTests.GeneratedMSBuildEditorConfig.editorconfig
  - Infrastructure.IntegrationTests.GlobalUsings.g.cs
  - Infrastructure.IntegrationTests.assets.cache
  - Infrastructure.IntegrationTests.csproj.AssemblyReference.cache
  - ... and 4 more files

**tests/Infrastructure.IntegrationTests/obj/Debug/net9.0/ref** (1 files)
  - CleanArchitecture.Infrastructure.IntegrationTests.dll

**tests/Infrastructure.IntegrationTests/obj/Debug/net9.0/refint** (1 files)
  - CleanArchitecture.Infrastructure.IntegrationTests.dll

**tests/Infrastructure.IntegrationTests/obj** (5 files)
  - Infrastructure.IntegrationTests.csproj.nuget.dgspec.json
  - Infrastructure.IntegrationTests.csproj.nuget.g.props
  - Infrastructure.IntegrationTests.csproj.nuget.g.targets
  - project.assets.json
  - project.nuget.cache

**tests/Web.AcceptanceTests** (4 files)
  - ConfigurationHelper.cs
  - GlobalUsings.cs
  - Web.AcceptanceTests.csproj
  - appsettings.json

**tests/Web.AcceptanceTests/Features** (2 files)
  - Login.feature
  - Login.feature.cs

**tests/Web.AcceptanceTests/Pages** (2 files)
  - BasePage.cs
  - LoginPage.cs

**tests/Web.AcceptanceTests/StepDefinitions** (1 files)
  - LoginStepDefinitions.cs

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/node** (1 files)
  - LICENSE

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/node/darwin-arm64** (1 files)
  - node

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package** (10 files)
  - README.md
  - ThirdPartyNotices.txt
  - api.json
  - browsers.json
  - cli.js
  - index.d.ts
  - index.js
  - index.mjs
  - package.json
  - protocol.yml

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/bin** (18 files)
  - PrintDeps.exe
  - README.md
  - install_media_pack.ps1
  - reinstall_chrome_beta_linux.sh
  - reinstall_chrome_beta_mac.sh
  - reinstall_chrome_beta_win.ps1
  - reinstall_chrome_stable_linux.sh
  - reinstall_chrome_stable_mac.sh
  - reinstall_chrome_stable_win.ps1
  - reinstall_msedge_beta_linux.sh
  - ... and 8 more files

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib** (8 files)
  - androidServerImpl.js
  - browserServerImpl.js
  - inProcessFactory.js
  - inprocess.js
  - outofprocess.js
  - utilsBundle.js
  - zipBundle.js
  - zipBundleImpl.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/cli** (3 files)
  - driver.js
  - program.js
  - programWithTestStub.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/client** (43 files)
  - accessibility.js
  - android.js
  - api.js
  - artifact.js
  - browser.js
  - browserContext.js
  - browserType.js
  - cdpSession.js
  - channelOwner.js
  - clientHelper.js
  - ... and 33 more files

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/common** (3 files)
  - socksProxy.js
  - timeoutSettings.js
  - types.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/generated** (6 files)
  - clockSource.js
  - consoleApiSource.js
  - injectedScriptSource.js
  - pollingRecorderSource.js
  - utilityScriptSource.js
  - webSocketMockSource.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/image_tools** (4 files)
  - colorUtils.js
  - compare.js
  - imageChannel.js
  - stats.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/protocol** (5 files)
  - debug.js
  - serializers.js
  - transport.js
  - validator.js
  - validatorPrimitives.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/remote** (2 files)
  - playwrightConnection.js
  - playwrightServer.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server** (43 files)
  - accessibility.js
  - artifact.js
  - browser.js
  - browserContext.js
  - browserType.js
  - clock.js
  - console.js
  - cookieStore.js
  - debugController.js
  - debugger.js
  - ... and 33 more files

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/android** (2 files)
  - android.js
  - backendAdb.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/bidi** (10 files)
  - bidiBrowser.js
  - bidiChromium.js
  - bidiConnection.js
  - bidiExecutionContext.js
  - bidiFirefox.js
  - bidiInput.js
  - bidiNetworkManager.js
  - bidiOverCdp.js
  - bidiPage.js
  - bidiPdf.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/bidi/third_party** (5 files)
  - bidiDeserializer.js
  - bidiKeyboard.js
  - bidiProtocol.js
  - bidiSerializer.js
  - firefoxPrefs.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/chromium** (18 files)
  - appIcon.png
  - chromium.js
  - chromiumSwitches.js
  - crAccessibility.js
  - crBrowser.js
  - crConnection.js
  - crCoverage.js
  - crDevTools.js
  - crDragDrop.js
  - crExecutionContext.js
  - ... and 8 more files

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/codegen** (8 files)
  - csharp.js
  - java.js
  - javascript.js
  - jsonl.js
  - language.js
  - languages.js
  - python.js
  - types.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/dispatchers** (23 files)
  - androidDispatcher.js
  - artifactDispatcher.js
  - browserContextDispatcher.js
  - browserDispatcher.js
  - browserTypeDispatcher.js
  - cdpSessionDispatcher.js
  - debugControllerDispatcher.js
  - dialogDispatcher.js
  - dispatcher.js
  - electronDispatcher.js
  - ... and 13 more files

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/electron** (2 files)
  - electron.js
  - loader.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/firefox** (8 files)
  - ffAccessibility.js
  - ffBrowser.js
  - ffConnection.js
  - ffExecutionContext.js
  - ffInput.js
  - ffNetworkManager.js
  - ffPage.js
  - firefox.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/har** (2 files)
  - harRecorder.js
  - harTracer.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/isomorphic** (1 files)
  - utilityScriptSerializers.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/recorder** (8 files)
  - contextRecorder.js
  - recorderApp.js
  - recorderCollection.js
  - recorderFrontend.js
  - recorderInTraceViewer.js
  - recorderRunner.js
  - recorderUtils.js
  - throttledFile.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/registry** (5 files)
  - browserFetcher.js
  - dependencies.js
  - index.js
  - nativeDeps.js
  - oopDownloadBrowserMain.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/trace/recorder** (3 files)
  - snapshotter.js
  - snapshotterInjected.js
  - tracing.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/trace/test** (1 files)
  - inMemorySnapshotter.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/trace/viewer** (1 files)
  - traceViewer.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/server/webkit** (10 files)
  - webkit.js
  - wkAccessibility.js
  - wkBrowser.js
  - wkConnection.js
  - wkExecutionContext.js
  - wkInput.js
  - wkInterceptableRequest.js
  - wkPage.js
  - wkProvisionalPage.js
  - wkWorkers.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/third_party** (2 files)
  - diff_match_patch.js
  - pixelmatch.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/utils** (32 files)
  - ascii.js
  - comparators.js
  - crypto.js
  - debug.js
  - debugLogger.js
  - env.js
  - eventsHelper.js
  - expectUtils.js
  - fileUtils.js
  - happy-eyeballs.js
  - ... and 22 more files

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/utils/isomorphic** (11 files)
  - cssParser.js
  - cssTokenizer.js
  - locatorGenerators.js
  - locatorParser.js
  - locatorUtils.js
  - mimeType.js
  - recorderUtils.js
  - selectorParser.js
  - stringUtils.js
  - traceUtils.js
  - ... and 1 more files

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/utilsBundleImpl** (2 files)
  - index.js
  - xdg-open

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/vite/htmlReport** (1 files)
  - index.html

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/vite/recorder/assets** (5 files)
  - codeMirrorModule-CR6kB851.js
  - codeMirrorModule-ez37Vkbh.css
  - codicon-DCmgc-ay.ttf
  - index-BW-aOBcL.css
  - index-BcaUAUCW.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/vite/recorder** (2 files)
  - index.html
  - playwright-logo.svg

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/vite/traceViewer/assets** (5 files)
  - codeMirrorModule-CZTtn9l8.js
  - inspectorTab-DTusvprx.js
  - testServerConnection-DeE2kSzz.js
  - workbench-DIEjrm3Z.js
  - xtermModule-BeNbaIVa.js

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/lib/vite/traceViewer** (20 files)
  - codeMirrorModule.ez37Vkbh.css
  - codicon.DCmgc-ay.ttf
  - embedded.Do_J5Hgs.js
  - embedded.html
  - embedded.w7WN2u1R.css
  - index.B21BXreT.js
  - index.CrbWWHbf.css
  - index.html
  - inspectorTab.DLjBDrQR.css
  - playwright-logo.svg
  - ... and 10 more files

**tests/Web.AcceptanceTests/bin/Debug/net9.0/.playwright/package/types** (3 files)
  - protocol.d.ts
  - structs.d.ts
  - types.d.ts

**tests/Web.AcceptanceTests/bin/Debug/net9.0** (47 files)
  - BoDi.dll
  - CleanArchitecture.Web.AcceptanceTests.deps.json
  - CleanArchitecture.Web.AcceptanceTests.dll
  - CleanArchitecture.Web.AcceptanceTests.pdb
  - CleanArchitecture.Web.AcceptanceTests.runtimeconfig.json
  - FluentAssertions.dll
  - Gherkin.dll
  - LivingDoc.Dtos.dll
  - LivingDoc.SpecFlowPlugin.dll
  - Microsoft.Bcl.AsyncInterfaces.dll
  - ... and 37 more files

**tests/Web.AcceptanceTests/bin/Debug/net9.0/cs** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/de** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/es** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/fr** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/it** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/ja** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/ko** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/pl** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/pt-BR** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/ru** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/runtimes/win/lib/netstandard2.0** (1 files)
  - System.Security.Cryptography.ProtectedData.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/tr** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/zh-Hans** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/bin/Debug/net9.0/zh-Hant** (5 files)
  - Microsoft.TestPlatform.CommunicationUtilities.resources.dll
  - Microsoft.TestPlatform.CoreUtilities.resources.dll
  - Microsoft.TestPlatform.CrossPlatEngine.resources.dll
  - Microsoft.VisualStudio.TestPlatform.Common.resources.dll
  - Microsoft.VisualStudio.TestPlatform.ObjectModel.resources.dll

**tests/Web.AcceptanceTests/obj/Debug/net9.0** (15 files)
  - .NETCoreApp,Version=v9.0.AssemblyAttributes.cs
  - CleanArchitecture.Web.AcceptanceTests.dll
  - CleanArchitecture.Web.AcceptanceTests.pdb
  - NUnit.AssemblyHooks.cs
  - Web.Acce.7C4131FC.Up2Date
  - Web.AcceptanceTests.AssemblyInfo.cs
  - Web.AcceptanceTests.AssemblyInfoInputs.cache
  - Web.AcceptanceTests.GeneratedMSBuildEditorConfig.editorconfig
  - Web.AcceptanceTests.GlobalUsings.g.cs
  - Web.AcceptanceTests.assets.cache
  - ... and 5 more files

**tests/Web.AcceptanceTests/obj/Debug/net9.0/ref** (1 files)
  - CleanArchitecture.Web.AcceptanceTests.dll

**tests/Web.AcceptanceTests/obj/Debug/net9.0/refint** (1 files)
  - CleanArchitecture.Web.AcceptanceTests.dll

**tests/Web.AcceptanceTests/obj** (5 files)
  - Web.AcceptanceTests.csproj.nuget.dgspec.json
  - Web.AcceptanceTests.csproj.nuget.g.props
  - Web.AcceptanceTests.csproj.nuget.g.targets
  - project.assets.json
  - project.nuget.cache

## 🔷 .NET Project Information
### Framework & Project Details
- **Target Framework:** net9.0
- **Framework Version:** .NET 9.0
- **Project Type:** Console Application
- **Output Type:** Exe
- **SDK Version:** 9.0.100
- **Modern .NET (Core/.NET 5+):** Yes

### Visual Studio Solution
- **Solution File Path:** CleanArchitecture.sln
- **Solution Format Version:** 12.00
- **Visual Studio Version:** 17.0.31903.59
- **Projects in Solution:** 15

#### Solution Projects
| Project Name | Type | Path |
|--------------|------|------|
| Domain | .NET Core Project | src\Domain\Domain.csproj |
| Application | .NET Core Project | src\Application\Application.csproj |
| Infrastructure | .NET Core Project | src\Infrastructure\Infrastructure.csproj |
| src | Solution Folder | src |
| tests | Solution Folder | tests |
| Application.UnitTests | .NET Core Project | tests\Application.UnitTests\Application.UnitTests.csproj |
| Domain.UnitTests | .NET Core Project | tests\Domain.UnitTests\Domain.UnitTests.csproj |
| Solution Items | Solution Folder | Solution Items |
| Web | .NET Core Project | src\Web\Web.csproj |
| Web.AcceptanceTests | .NET Core Project | tests\Web.AcceptanceTests\Web.AcceptanceTests.csproj |
| Application.FunctionalTests | .NET Core Project | tests\Application.FunctionalTests\Application.FunctionalTests.csproj |
| Infrastructure.IntegrationTests | .NET Core Project | tests\Infrastructure.IntegrationTests\Infrastructure.IntegrationTests.csproj |
| SentraUnitTests | .NET Core Project | SentraUnitTests\SentraUnitTests.csproj |
| AppHost | C# Project | src\AppHost\AppHost.csproj |
| ServiceDefaults | C# Project | src\ServiceDefaults\ServiceDefaults.csproj |

### Central Package Version Management (CPVM)
- **Enabled:** Yes
- **Directory.Packages.props:** Directory.Packages.props
- **Centrally Managed Packages:** 57

#### Centrally Managed Package Versions
| Package Name | Version |
|--------------|---------|
| Aspire.Npgsql.EntityFrameworkCore.PostgreSQL | $(AspireVersion) |
| Aspire.Hosting.PostgreSQL | $(AspireVersion) |
| Aspire.Microsoft.EntityFrameworkCore.SqlServer | $(AspireVersion) |
| Aspire.Hosting.SqlServer | $(AspireVersion) |
| Aspire.Hosting.AppHost | $(AspireVersion) |
| Microsoft.Extensions.Http.Resilience | 9.0.0 |
| Microsoft.Extensions.ServiceDiscovery | 9.0.0 |
| OpenTelemetry.Exporter.OpenTelemetryProtocol | 1.10.0 |
| OpenTelemetry.Extensions.Hosting | 1.10.0 |
| OpenTelemetry.Instrumentation.AspNetCore | 1.9.0 |
| OpenTelemetry.Instrumentation.Http | 1.9.0 |
| OpenTelemetry.Instrumentation.Runtime | 1.9.0 |
| Ardalis.GuardClauses | 4.6.0 |
| AutoMapper | 13.0.1 |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.2 |
| Azure.Identity | 1.13.1 |
| coverlet.collector | 6.0.2 |
| FluentAssertions | 6.12.2 |
| FluentValidation.AspNetCore | 11.3.0 |
| FluentValidation.DependencyInjectionExtensions | 11.11.0 |
| MediatR | 12.4.1 |
| Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | $(AspnetVersion) |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | $(AspnetVersion) |
| Microsoft.AspNetCore.Mvc.Testing | $(AspnetVersion) |
| Microsoft.AspNetCore.OpenApi | $(AspnetVersion) |
| Microsoft.AspNetCore.Identity.UI | $(AspnetVersion) |
| Microsoft.AspNetCore.SpaProxy | $(AspnetVersion) |
| Microsoft.Data.SqlClient | 5.2.2 |
| Microsoft.EntityFrameworkCore | $(EfcoreVersion) |
| Microsoft.EntityFrameworkCore.Design | $(EfcoreVersion) |
| Microsoft.EntityFrameworkCore.Relational | $(EfcoreVersion) |
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.1 |
| Testcontainers.PostgreSql | 4.0.0 |
| Microsoft.EntityFrameworkCore.Sqlite | $(EfcoreVersion) |
| Microsoft.EntityFrameworkCore.SqlServer | $(EfcoreVersion) |
| Testcontainers.MsSql | 4.0.0 |
| Microsoft.EntityFrameworkCore.Tools | $(EfcoreVersion) |
| Microsoft.Extensions.Configuration.Json | $(MicrosoftExtensionsVersion) |
| Microsoft.Extensions.Configuration.EnvironmentVariables | $(MicrosoftExtensionsVersion) |
| Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore | $(MicrosoftExtensionsVersion) |
| Microsoft.Extensions.Hosting | $(MicrosoftExtensionsVersion) |
| Microsoft.NET.Test.Sdk | 17.11.1 |
| Moq | 4.20.72 |
| NSwag.AspNetCore | 14.2.0 |
| NSwag.MSBuild | 14.2.0 |
| nunit | 3.14.0 |
| NUnit.Analyzers | 3.10.0 |
| NUnit3TestAdapter | 4.6.0 |
| Respawn | 6.2.1 |
| Shouldly | 4.2.1 |
| System.Configuration.ConfigurationManager | 9.0.0 |
| Microsoft.Playwright | 1.48.0 |
| SpecFlow.Plus.LivingDocPlugin | 3.9.57 |
| SpecFlow.NUnit | 3.9.74 |
| xunit | 2.6.1 |
| xunit.runner.visualstudio | 2.5.3 |
| AutoFixture | 4.18.0 |

### Project References (11)
| Reference Name | Path |
|----------------|------|
| Application | ../src/Application/Application.csproj |
| ..\Web\Web | ..\Web\Web.csproj |
| ..\Domain\Domain | ..\Domain\Domain.csproj |
| ..\Application\Application | ..\Application\Application.csproj |
| ..\Application\Application | ..\Application\Application.csproj |
| ..\Infrastructure\Infrastructure | ..\Infrastructure\Infrastructure.csproj |
| ..\ServiceDefaults\ServiceDefaults | ..\ServiceDefaults\ServiceDefaults.csproj |
| ..\..\src\Web\Web | ..\..\src\Web\Web.csproj |
| ..\..\src\Application\Application | ..\..\src\Application\Application.csproj |
| ..\..\src\Infrastructure\Infrastructure | ..\..\src\Infrastructure\Infrastructure.csproj |
| ..\..\src\Domain\Domain | ..\..\src\Domain\Domain.csproj |

## 📊 Statistics
- **Total Files:** 1175
- **Total Dependencies:** 0
- **Total Dev Dependencies:** 0
- **Total Test Folders:** 9
- **Total Test Files:** 757
- **Total Project References:** 11
- **Total Third-Party Libraries:** 0

---
*Report generated by Language Detector & Dependency Scanner v3*