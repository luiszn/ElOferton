# 🛍️ EL OFERTÓN - Sistema de Gestión Comercial, POS e Inventario

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet)
[![Entity Framework](https://img.shields.io/badge/EF_Core-10.0-68217A?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Bootstrap](https://img.shields.io/badge/UI-Bootstrap_5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)

**EL OFERTÓN** es una solución integral desarrollada en **ASP.NET Core MVC** diseñada específicamente para la gestión de ventas minoristas (retail), control de inventario multialmacén, catálogo de cosméticos y belleza, administración de clientes y reportes comerciales en tiempo real para la red comercial de **El Ofertón (Cajamarca, Perú)**.

---

## 📋 Tabla de Contenidos

- [Características Principales](#-características-principales)
- [Arquitectura y Tecnologías](#-arquitectura-y-tecnologías)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Roles y Políticas de Acceso](#-roles-y-políticas-de-acceso)
- [Requisitos Previos](#-requisitos-previos)
- [Guía de Instalación y Configuración](#-guía-de-instalación-y-configuración)
- [Credenciales de Acceso Inicial](#-credenciales-de-acceso-inicial)
- [Flujo de Trabajo Operativo](#-flujo-de-trabajo-operativo)
- [Integridad y Concurrencia de Datos](#-integridad-y-concurrencia-de-datos)
- [Roadmap de Próximas Mejoras](#-roadmap-de-próximas-mejoras)

---

## ✨ Características Principales

### 🛒 1. Punto de Venta (POS) Ágil
- **Soporte para lector de código de barras:** Detección automática al escanear con la tecla Enter sin enviar el formulario de manera accidental.
- **Búsqueda reactiva de productos:** Filtro en tiempo real por nombre, código, marca o categoría, validando stock disponible en el ambiente actual.
- **Carrito dinámico:** Ajuste ágil de cantidades con validación en vivo frente a existencias físicas.
- **Múltiples métodos de pago:** Efectivo (con cálculo automático de vuelto), Yape, Plin, Tarjeta de Crédito/Débito y otros.
- **Gestión de clientes en caja:** Selección de clientes registrados o registro de clientes ocasionales de mostrador.
- **Control de descuentos:** Aplicación controlada de descuentos con restricciones de permisos según rol.
- **Emisión e impresión de comprobantes:** Vista detallada de venta con formato imprimible para tickets.

### 📦 2. Control de Inventario y Kardex Multialmacén
- **Existencias por ambiente:** Monitoreo del stock en almacén central, administración y cada uno de los puntos de venta (`PV-01`, `PV-02`, `PV-03`, `PV-04`, etc.).
- **Trazabilidad total (Kardex):** Registro cronológico de cada movimiento con stock anterior y posterior garantizados tras la actualización.
- **Tipos de movimiento:**
  - *Entradas:* Compras o ingresos a almacén.
  - *Salidas:* Bajas, mermas o productos dañados.
  - *Ajustes:* Regularización por conteos físicos.
  - *Traspasos entre ambientes:* Movimiento atómico origen-destino (descuenta del ambiente emisor e incrementa en el receptor en una única transacción segura).
- **Semáforo de stock bajo:** Detección de productos que han alcanzado o superado su umbral de stock mínimo.

### 💄 3. Catálogo de Belleza y Cosmética
- Gestión completa de **Productos**, **Categorías** (Maquillaje, Peluquería, Cuidado Facial, Barbería, etc.) y **Marcas** (Maybelline, L'Oréal, Nivea, Revlon, Babyliss, etc.).
- Control de precio de compra, precio de venta al público y márgenes.
- Activación y desactivación de productos y catálogos con preservación de histórico de ventas.

### 👥 4. Gestión de Clientes
- Registro con validación de tipo de documento oficial peruano (**DNI** de 8 dígitos, **RUC** de 11 dígitos, CE, Pasaporte).
- Historial de compras por cliente con total acumulado y detalle de comprobantes adquiridos.

### 📊 5. Dashboard Analítico y Reportes
- **Dashboard en tiempo real:** Ventas del día, ventas del mes, productos con stock crítico y gráfico SVG de evolución en los últimos 7 días.
- **Distribución de ingresos:** Métricas por método de pago (Efectivo vs. Billeteras digitales vs. Tarjetas).
- **Generación de Reportes Especializados:**
  - Ventas por fecha y por mes.
  - Rendimiento por vendedor y por punto de venta.
  - Productos más vendidos y productos bajo stock.
- **Exportación de datos:** Descarga inmediata de reportes en formato **CSV**.

---

## 🛠️ Arquitectura y Tecnologías

El sistema sigue el patrón de **Arquitectura en Capas (Layered Architecture)** con separación limpia de responsabilidades:

```
ElOferton/
 ├── Controllers/      # Controladores MVC livianos con inyección de dependencias
 ├── Services/         # Lógica de negocio (Ventas, Inventario, Clientes, Dashboard)
 ├── Models/           # Entidades de dominio y modelo de identidad
 ├── ViewModels/       # Modelos fuertemente tipados para vistas y formularios
 ├── Data/             # DbContext, Migraciones y Fluent API Configurations
 ├── Constants/        # Roles, Políticas de autorización, Enums y Reloj local (UTC-5)
 ├── Views/            # Vistas Razor estructuradas en módulos y partials
 └── wwwroot/          # Recursos estáticos: JS vanilla optimizado, CSS temático
```

### Stack Técnico
- **Framework:** .NET 10.0 (C# 13/14)
- **Persistencia:** Entity Framework Core 10 con SQL Server (`Microsoft.EntityFrameworkCore.SqlServer`)
- **Autenticación y Seguridad:** ASP.NET Core Identity con Claims personalizados (`PuntoVentaId`, `NombreCompleto`)
- **Protección CSRF:** `AutoValidateAntiforgeryTokenAttribute` global
- **Frontend:** Razor Pages / CSHTML, Bootstrap 5.3, Bootstrap Icons, Vanilla JavaScript modular
- **Zona Horaria:** Controlada mediante `Reloj.Ahora` adaptada a la zona horaria de Perú (`America/Lima` / `SA Pacific Standard Time`)

---

## 🛡️ Roles y Políticas de Acceso

El sistema implementa control de acceso basado en roles (**RBAC**):

| Módulo / Funcionalidad | Administrador | Vendedor | Almacenero |
| :--- | :---: | :---: | :---: |
| **Dashboard y Métricas** | ✅ | ✅ | ✅ |
| **Punto de Venta (POS)** | ✅ *(Todos los ambientes y descuento)* | ✅ *(Solo su ambiente asignado)* | ❌ |
| **Historial de Ventas** | ✅ | ✅ | ❌ |
| **Gestión de Clientes** | ✅ | ✅ | ❌ |
| **Catálogo de Productos** | ✅ *(Lectura y Edición)* | ✅ *(Solo lectura)* | ✅ *(Lectura y Edición)* |
| **Categorías y Marcas** | ✅ | ❌ | ✅ |
| **Existencias y Movimientos** | ✅ | ❌ | ✅ |
| **Traspasos de Inventario** | ✅ | ❌ | ✅ |
| **Reportes y Exportación CSV** | ✅ | ✅ | ✅ |
| **Gestión de Usuarios y Roles** | ✅ | ❌ | ❌ |

---

## 💻 Requisitos Previos

1. **.NET 10.0 SDK** (o superior) instalado en el equipo.
2. **Microsoft SQL Server** (Edición Express, Developer o LocalDB).
3. **Visual Studio 2022 / 2026** (con carga de trabajo de desarrollo ASP.NET) o **Visual Studio Code** con la extensión C# Dev Kit.
4. Herramienta global de Entity Framework Core:
   ```powershell
   dotnet tool install --global dotnet-ef
   ```

---

## 🚀 Guía de Instalación y Configuración

### 1. Clonar o abrir el repositorio
Abre una terminal en la carpeta del proyecto:
```powershell
cd c:\Users\luisd\OneDrive\Escritorio\IA\ElOferton
```

### 2. Configurar la Cadena de Conexión
Revisa el archivo `appsettings.json` y asegúrate de que la cadena apunte a tu servidor de SQL Server:

```json
{
  "ConnectionStrings": {
    "ElOfertonDb": "Server=localhost;Database=ElOfertonDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```
*(Reemplaza `localhost` por el nombre de tu instancia si usas una instancia nombrada como `SQLEXPRESS` o `(localdb)\\mssqllocaldb`).*

### 3. Aplicar Migraciones en la Base de Datos
Crea la base de datos y todas las tablas ejecutando:
```powershell
dotnet ef database update
```

### 4. Compilar y Ejecutar el Proyecto
```powershell
dotnet run
```
O presiona `F5` en Visual Studio. La aplicación se iniciará en `https://localhost:7000` (o el puerto configurado en `launchSettings.json`).

---

## 🔑 Credenciales de Acceso Inicial

El sistema incluye un mecanismo de arranque (**Bootstrap**) en desarrollo. Al iniciar por primera vez, se generará de manera automática el usuario administrador y los catálogos base de prueba:

- **Correo / Usuario:** `admin@eloferton.com`
- **Contraseña:** `Admin123*`
- **Rol asignado:** `Administrador`
- **Ambiente por defecto:** `ADM-01 (Administración)`

> [!TIP]
> Una vez dentro del sistema, el Administrador puede ingresar al menú **Seguridad > Usuarios** para dar de alta a los cajeros y almaceneros, asignándoles sus respectivos puntos de venta físicos.

---

## 🔒 Integridad y Concurrencia de Datos

El diseño de la base de datos incluye mecanismos avanzados de consistencia:

- **Tokens de Concurrencia (`RowVersion`):**
  Tanto `Producto` como `Inventario` disponen de sellos de concurrencia optimista (`byte[] RowVersion`) para evitar sobreescrituras si dos usuarios modifican el mismo producto al mismo tiempo.
- **Estrategia de Ejecución Resiliente:**
  Las operaciones críticas de descuento de inventario en ventas y transferencias se ejecutan mediante `db.Database.CreateExecutionStrategy()`, tolerando transacciones distribuidas y reintentos ante bloqueos de red o de base de datos.
- **Restricciones de Check a nivel de motor (Check Constraints):**
  - `CK_Inventarios_Cantidad`: Impide saldos negativos de stock a nivel de motor.
  - `CK_Ventas_Montos`: Garantiza que subtotales, totales y descuentos nunca sean inferiores a cero.
  - `CK_Productos_Precios`: Exige que los precios de compra y venta sean estrictamente positivos.

---

## 🗺️ Roadmap de Próximas Mejoras

- [ ] **Módulo de Anulación de Ventas (Devoluciones):** Botón administrativo para revertir ventas, reintegrando el stock al ambiente origen y registrando la anulación en el Kardex.
- [ ] **Modo Ticket Térmico (58mm / 80mm):** Estilos `@media print` optimizados para ticketeras térmicas de punto de venta.
- [ ] **Apertura y Arqueo de Caja:** Control de turnos con saldo inicial de efectivo y conciliación de caja al cierre de jornada.
- [ ] **Modal de Cliente Rápido:** Registro express de nuevos clientes desde la propia pantalla del POS.
- [ ] **Paginación Servidor:** Paginación de alto rendimiento en el catálogo de productos y clientes.

---

<p align="center">
  <b>EL OFERTÓN</b> · Cajamarca, Perú · Desarrollado con ❤️ en ASP.NET Core & C#
</p>
