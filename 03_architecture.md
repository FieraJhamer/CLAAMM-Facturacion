# Arquitectura

## Stack tecnológico

- Lenguaje: C#
- Framework: .NET 8
- UI: WPF
- Base de datos: SQLite
- ORM: Dapper
- PDF: QuestPDF

## Estructura del proyecto

/ClaammApp
 ├── UI (WPF)
 ├── Application
 ├── Domain
 ├── Infrastructure

## Capas

### Domain
- Modelos
- Reglas de negocio

### Application
- Casos de uso
- Lógica de aplicación

### Infrastructure
- SQLite
- Repositorios
- Generación de PDF

### UI
- Pantallas
- Interacción usuario

## Base de datos

### Tabla: Items

- Id INTEGER PK
- Codigo TEXT
- Descripcion TEXT
- Unidad TEXT
- PrecioUnitario REAL
- Rubro TEXT

### Tabla: Presupuestos

- Id INTEGER PK
- ClienteNombre TEXT
- Fecha TEXT

### Tabla: PresupuestoItems

- Id INTEGER PK
- PresupuestoId INTEGER
- Descripcion TEXT
- Unidad TEXT
- Cantidad REAL
- PrecioUnitario REAL
- Total REAL

## Decisiones

- SQLite local → simplicidad
- Dapper → liviano
- QuestPDF → control total del layout