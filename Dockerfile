# Usamos el SDK porque es el único que tiene las herramientas de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

# 1. Copiamos todos los archivos de tu computadora al contenedor
COPY ./CinemaMasterTicketsMVC .

# 2. Restauramos, compilamos y publicamos todo en una carpeta llamada 'out'
RUN dotnet publish -c Release -o out

# 3. Nos movemos a esa carpeta para ejecutar la app
WORKDIR /app/out

# 4. Exponemos el puerto y ejecutamos
EXPOSE 80
ENTRYPOINT ["dotnet", "CinemaMasterTicketsMVC.dll"]
