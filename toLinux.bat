del /q release\*.*
dotnet publish ExprodesC.csproj  /p:TargetFramework=net8.0 /p:Configuration=Release /p:Platform=AnyCPU /p:PublishDir=release\ /p:SelfContained=true /p:RuntimeIdentifier=linux-x64
::cd bin\Release\net8.0\linux-x64\publish\
cd release
del C:\temp\AvalonReleases\Linux\ExprodesC.tar.gz /f/s/q 
tar -cvzf C:\temp\AvalonReleases\Linux\ExprodesC.tar.gz *.*
::C:\temp\At\bin\Publish\netcoreapp8.0\linux-x64 
::tar -cvzf test.tar.gz -C bin\Release\net8.0\linux-x64\publish 
