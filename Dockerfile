FROM microsoft/dotnet:aspnetcore-runtime AS runtime
 
WORKDIR /app
 
EXPOSE 80
EXPOSE 443
 
ARG SourcePath
 
COPY $SourcePath ./
 
ENV ASPNETCORE_ENVIRONMENT Development
 
ENV "ConnectionStrings:VhBookings"="Server=192.168.1.214,1433\\SQLEXPRESS;Database=VhBookings;User Id=docker;Password=d0cker100!;"

ENTRYPOINT ["dotnet", "Bookings.API.dll"]