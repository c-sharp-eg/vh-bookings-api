FROM microsoft/dotnet:aspnetcore-runtime AS runtime
 
WORKDIR /app
 
EXPOSE 80
EXPOSE 443
 
COPY ./dotentArtifacts/WebApp ./

ARG ConnectionStringsVhBookings
 
ENV ASPNETCORE_ENVIRONMENT Development
 
ENV ConnectionStrings:VhBookings=$ConnectionStringsVhBookings

ENTRYPOINT ["dotnet", "Bookings.API.dll"]