# Próximos pasos para generar código

## Paso 1

Generar solución .NET:

dotnet new sln -n ClaammApp

## Paso 2

Crear proyectos:

dotnet new wpf -n ClaammApp.UI
dotnet new classlib -n ClaammApp.Domain
dotnet new classlib -n ClaammApp.Application
dotnet new classlib -n ClaammApp.Infrastructure

## Paso 3

Agregar referencias:

UI → Application
Application → Domain
Infrastructure → Domain + Application

## Paso 4

Instalar paquetes:

- Dapper
- Microsoft.Data.Sqlite
- QuestPDF

## Paso 5

Crear:

- Modelos (Domain)
- Repositorios (Infrastructure)
- Servicios (Application)
- UI básica (WPF)

## Paso 6

Implementar:

1. CRUD Items
2. Crear Presupuesto
3. Generar PDF