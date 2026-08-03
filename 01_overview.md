# Sistema de Presupuestos CLAMM

## Objetivo

Aplicación desktop para Windows que permite gestionar una lista de precios unitarios y generar presupuestos en PDF de forma simple e intuitiva.

## Contexto

Actualmente la empresa trabaja con Excel para:
- Mantener precios
- Generar presupuestos
- Exportar a PDF

El objetivo es reemplazar Excel por una aplicación más eficiente y fácil de usar.

## Usuario objetivo

- Dueño de empresa constructora
- Bajo conocimiento técnico
- Necesita rapidez y simplicidad

## Características principales

- CRUD de ítems de precios
- Búsqueda por nombre (no por código)
- Generación de presupuestos
- Exportación a PDF con formato profesional
- Base de datos local (SQLite)
- Sin instalación (ejecutable único)

## Restricciones

- Funciona offline
- Solo Windows
- Sin backend
- Sin login

## Decisiones clave

- No se importa Excel
- No hay historial de precios (por ahora)
- Unidades predefinidas