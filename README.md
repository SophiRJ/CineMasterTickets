# 🛒 CineMaster Tickets MVC
 
**CineMaster Tickets** es una plataforma integral de gestión cinematográfica desarrollada bajo una arquitectura **ASP.NET MVC**. El sistema simula el ecosistema completo de una empresa de cines, desde la administración interna de recursos y personal hasta la experiencia de usuario final para la reserva de entradas y consumo de productos.
 
---
 
## 🚀 Características Principales
 
### 🏗️ Gestión Administrativa (Panel de Control)
* **Gestión de RRHH:** Altas, bajas y control de perfiles para empleados y usuarios.
* **Inventario de Complementos:** CRUD completo de productos (bebidas, snacks, etc.) y gestión de salida a la venta.
* **Cartelera Dinámica:** Creación y edición de películas y sesiones.
* **Business Intelligence:** Panel de recaudación con métricas detalladas por película y complementos.
* **Auditoría de Tickets:** Buscador avanzado por ID, rango de fechas e importe.
 
### 🎥 Experiencia del Cliente
* **Cartelera Inteligente:** * Las sesiones pasadas se marcan como **completadas** y se inhabilitan.
    * Si una película no tiene sesiones activas o todas han caducado, se retira automáticamente de la vista del cliente.
* **Sistema de Fidelización:** Los usuarios registrados acumulan puntos por cada compra, los cuales pueden canjear como descuento en el checkout.
* **Flujo de Reserva:** Selección interactiva de butacas y carrito de complementos.
* **Check-out & Tickets:** Generación de ticket digital descargable con **código QR** que contiene la firma digital de la compra.
 
---
 
## 🛠️ Tecnologías Utilizadas
 
* **Backend:** C# con ASP.NET Core MVC.
* **Frontend:** HTML5, CSS3, JavaScript y Razor Pages.
* **Lógica de Datos:** LINQ (Language Integrated Query) y Entity Framework Core.
* **Infraestructura:** Docker (Containerización) y Azure (Cloud Hosting).
* **IDE:** Visual Studio 2022.
 
---
 
## 📊 Arquitectura de Datos y Lógica de Negocio
 
El sistema utiliza **Entity Framework** para gestionar la persistencia. Dos puntos clave de la arquitectura son:
 
1.  **Relación Ticket-Puntos:** * Cada vez que un `Usuario Registrado` genera un `Ticket`, el sistema calcula un porcentaje del importe total y lo suma a su saldo de `Puntos`.
    * En el proceso de pago, el sistema valida si el usuario desea aplicar sus puntos acumulados, restándolos del total y actualizando el perfil del usuario en una sola transacción atómica.
 
2.  **Lógica de Disponibilidad:**
    * Se implementa lógica a nivel de servidor que compara `DateTime.Now` con las sesiones programadas. Este filtro se aplica mediante **LINQ** antes de enviar el modelo a la vista, garantizando que el cliente nunca vea contenido obsoleto.
 
 
---
 
## 🐳 Despliegue con Docker
 
Este proyecto está totalmente containerizado, lo que permite arrancarlo en segundos sin necesidad de configurar SQL Server o dependencias locales.
 
**Requisitos:** Tener instalado Docker Desktop.
 
1.  Abre una terminal (CMD o VS Code) en la carpeta raíz del proyecto.
2.  Ejecuta el comando:
    ```bash
    docker compose up -d
    ```
3.  Accede a `localhost:puerto` (ver puerto en archivo compose) en tu navegador.
 
 
[Image of Docker Compose architecture diagram]
 
 
---
 
## ⚙️ Configuración para Desarrollo (Local)
 
Si deseas clonar el proyecto y ejecutarlo desde Visual Studio, debes configurar el almacenamiento de secretos para las credenciales.
 
**IMPORTANTE:** Por seguridad, las cadenas de conexión no están en el código fuente. Debes configurar tu `secrets.json` de la siguiente manera:
 
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CinemaMasterTicketsMVC;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "MovieApi": {
    "ApiKey": "Pon aquí tu APIKey"
  },
  "AdminSettings": {
    "Email": "admin@site.com",
    "Password": "Admin123!" Puedes cambiar estas credenciales a tu gusto
  }
}
 
```
## 🤝 Reconocimientos y Colaboración
 
Este proyecto no habría sido posible sin el trabajo conjunto y la dedicación constante. Quiero expresar mi más sincero agradecimiento a **[Ruben](https://github.com/RglfDev)**, con quien he compartido cada fase de este desarrollo.
 
Hemos trabajado mano a mano al **50%**, fusionando nuestras ideas y habilidades para sacar adelante este sistema. Su capacidad para resolver problemas y su visión técnica han sido fundamentales para que **CineMaster Tickets** sea hoy una realidad funcional. Trabajar conjuntamente ha enriquecido no solo el código, sino también nuestra metodología de colaboración en equipo.
 
---
 
## 👋 Despedida y Contacto
 
¡Muchas gracias por haber dedicado parte de tu tiempo a leer sobre nuestro proyecto!
 
Esperamos que esta simulación de gestión cinematográfica te resulte interesante y útil. Si tienes alguna duda, sugerencia o simplemente quieres charlar sobre la implementación técnica, no dudes en contactar con nosotros o abrir un **Issue** en el repositorio.
 
**¡Un saludo y feliz código!** 🚀
