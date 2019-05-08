FROM microsoft/dotnet:aspnetcore-runtime AS runtime
 
WORKDIR /app
 
EXPOSE 80
EXPOSE 443
 
COPY ./dotentArtifacts/WebApp ./

ARG ConnectionStrings:VhBookings="Server=192.168.1.214,1433\\SQLEXPRESS;Database=VhBookings;User Id=docker;Password=d0cker100!;"
 
ENV ASPNETCORE_ENVIRONMENT Development
 
ENV "ConnectionStrings:VhBookings"="$ConnectionStrings:VhBookings"

ENTRYPOINT ["dotnet", "Bookings.API.dll"]