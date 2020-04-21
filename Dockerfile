FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine

COPY entrypoint.ps1 /entrypoint.ps1
COPY lib/ActionsCore.ps1 /lib/ActionsCore.ps1

RUN ["chmod", "+x", "/entrypoint.ps1"]

SHELL [ "/usr/bin/pwsh" ]

ENTRYPOINT ["/entrypoint.ps1"]