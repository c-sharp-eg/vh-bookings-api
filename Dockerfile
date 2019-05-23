FROM microsoft/dotnet:aspnetcore-runtime AS runtime
 
WORKDIR /app


# Arguments used in the docker image build process

ARG ConnectionStringsVhBookings
ARG ApplicationInsightsInstrumentationKey
ARG AzureAdTenantId
ARG VhServicesBookingsApiResourceId
ARG ServiceBusQueueConnectionString
ARG ServiceBusQueueQueueName


# All the environment variables used during container runtime

ENV ConnectionStrings:VhBookings=$ConnectionStringsVhBookings
ENV ApplicationInsights:InstrumentationKey = $ApplicationInsightsInstrumentationKey
ENV AzureAd:TenantId = $AzureAdTenantId
ENV VhServices:BookingsApiResourceId = $VhServicesBookingsApiResourceId
ENV ServiceBusQueue:ConnectionString = $ServiceBusQueueConnectionString
ENV ServiceBusQueue:QueueName = $ServiceBusQueueQueueName

ENV ASPNETCORE_ENVIRONMENT Development
 
EXPOSE 80
EXPOSE 443
 
COPY ./dotentArtifacts/WebApp ./

ENTRYPOINT ["dotnet", "Bookings.API.dll"]