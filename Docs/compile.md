### Comando para compilar el proyecto

> dotnet publish "ClaammApp.UI\ClaammApp.UI.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publicar