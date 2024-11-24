FROM mcr.microsoft.com/vscode/devcontainers/dotnet:9.0

COPY --from=golang:1.23-bookworm /usr/local/go/ /usr/local/go/

ENV PATH="/usr/local/go/bin:${PATH}"
