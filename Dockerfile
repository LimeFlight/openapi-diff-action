FROM mcr.microsoft.com/powershell:alpine-3.10

COPY entrypoint.ps1 /entrypoint.ps1

RUN ["chmod", "+x", "/entrypoint.ps1"]

SHELL [ "/usr/bin/pwsh" ]

ENTRYPOINT ["/entrypoint.ps1"]