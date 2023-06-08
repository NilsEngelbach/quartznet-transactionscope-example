ARG VARIANT="7.0"
FROM mcr.microsoft.com/devcontainers/dotnet:${VARIANT}

USER vscode
RUN dotnet tool install --global dotnet-ef && echo 'export PATH=$PATH:/home/vscode/.dotnet/tools' >> ~/.bashrc