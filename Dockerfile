# Este archivo define las instrucciones para crear las imagenes docker que se usara 
#para que luego docker compose pueda levantar los contenedores
# indica la imagen base desde la que se contruye la imagen docker
FROM mcr.microsoft.com/dotnet/sdk:8.0 
# Este es el directorio de trabajo dentro del contenedor 
WORKDIR /app  

# Copiamos el contenido del proyecto al contenedor /app
COPY ./CinemaMasterTicketsMVC .

# Restauramos compilamos y publicamos en la carpeta out
#este es el comando que se ejecuta dentro del contenedor para restuarar dependencias, compilar el proyecto en la carpeta out
RUN dotnet publish -c Release -o out

# Cambiamos de ruta a donde se publico la app la carpeta out
WORKDIR /app/out

# Aqui se indica que usa el puerto 80, pero esto es solo informativo en realidad esta usando el 8080:80 definido en el compose
EXPOSE 80
# En esta linea se define el comando que se ejecuta para arrancar el contenedor
ENTRYPOINT ["dotnet", "CinemaMasterTicketsMVC.dll"]
