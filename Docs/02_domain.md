# Modelo de Dominio

## Entidad: Item

Representa un precio unitario.

### Campos

- Id (int)
- Codigo (string)
- Descripcion (string)
- Unidad (enum)
- PrecioUnitario (decimal)
- Rubro (string)

## Unidades permitidas

- m2
- m3
- un
- grl

## Entidad: Presupuesto

### Campos

- Id (int)
- ClienteNombre (string)
- Fecha (DateTime)
- Items (List<PresupuestoItem>)
- Total (decimal)

## Entidad: PresupuestoItem

- Id (int)
- ItemId (int)
- Descripcion (string)
- Unidad (string)
- Cantidad (decimal)
- PrecioUnitario (decimal)
- Total (decimal)

## Reglas

- TotalItem = Cantidad * PrecioUnitario
- TotalPresupuesto = suma de todos los items

## Rubros

- Lista editable por el usuario