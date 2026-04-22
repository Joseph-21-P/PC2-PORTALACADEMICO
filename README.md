# Portal Académico - Examen Parcial

**URLs del Proyecto:**
* **GitHub:** https://github.com/TuUsuario/PC2-PORTALACADEMICO
* **Render:** https://pc2-portalacademico.onrender.com

## Descripción
Este es un portal web interno para gestionar cursos, estudiantes y matrículas, desarrollado con ASP.NET Core MVC 8, Entity Framework Core (SQLite), Identity para autenticación y Redis para gestión de caché y sesiones.

## Ejecución Local
1. Clonar el repositorio:
   ```bash
   git clone [URL_DE_TU_REPOSITORIO]
2. Ejecutar `dotnet restore`
3. Ejecutar `dotnet ef database update`
4. Ejecutar `dotnet run`

## Usuarios de Prueba
* **Coordinador:** coordinador@universidad.edu / Password123!

## Variables de Entorno en Render
Para que el despliegue funcione correctamente, se configuraron las siguientes variables:
* `ASPNETCORE_ENVIRONMENT` = Production
* `ASPNETCORE_URLS` = http://0.0.0.0:${PORT}
* `ConnectionStrings__DefaultConnection` = DataSource=app.db
* `Redis__ConnectionString` = [Tu cadena de conexión de Redis Labs]