run:
	dotnet run
build:
	dotnet build
kill:
	powershell -Command "Get-Process -Name 'WindowsMediaController' -ErrorAction SilentlyContinue | Stop-Process -Force"
rebuild: kill build
publish:
	powershell -ExecutionPolicy Bypass -File publish.ps1
publish-selfcontained:
	powershell -ExecutionPolicy Bypass -File publish.ps1 -PublishType SelfContained
package:
	powershell -ExecutionPolicy Bypass -File package.ps1
package-selfcontained:
	powershell -ExecutionPolicy Bypass -File package.ps1 -PublishType SelfContained