del /q release\*.*
dotnet publish ExprodesC.csproj /p:TargetFramework=net8.0 /p:Configuration=Release /p:Platform=AnyCPU /p:PublishDir=release\ /p:SelfContained=true /p:RuntimeIdentifier=win-x64
::cd bin\Release\net8.0\linux-x64\publish\
del /q C:\temp\AvalonReleases\Win\ExprodesC\*.*
xcopy /s /i  "release"  "C:\temp\AvalonReleases\Win\ExprodesC" 
::cd release 
::tar -cvzf C:\temp\AvalonReleases\Win\dlgMvvm.tar.gz *.*
::C:\temp\At\bin\Publish\netcoreapp8.0\linux-x64 
::tar -cvzf test.tar.gz -C bin\Release\net8.0\linux-x64\publish 
