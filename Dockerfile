FROM mcr.microsoft.com/dotnet/sdk:7.0 as builder

COPY . /project

WORKDIR /project

RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runner

COPY --from=builder /project/bin/Release/net7.0/ /app/

WORKDIR /app

RUN chmod +x ./TelegramWebhooks

ENTRYPOINT ["./TelegramWebhooks", "--urls http://0.0.0.0:80"]
