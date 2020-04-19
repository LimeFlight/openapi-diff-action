# FROM mcr.microsoft.com/powershell:alpine-3.10
# FROM mcr.microsoft.com/dotnet/core/runtime:3.1.3-alpine3.11
FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201-alpine3.11

COPY entrypoint.ps1 /entrypoint.ps1
COPY lib/ActionsCore.ps1 /lib/ActionsCore.ps1

RUN ["chmod", "+x", "/entrypoint.ps1"]

SHELL [ "/usr/bin/pwsh" ]

ENTRYPOINT ["/entrypoint.ps1"]