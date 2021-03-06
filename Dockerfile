#-------------------------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See https://go.microsoft.com/fwlink/?linkid=2090316 for license information.
#-------------------------------------------------------------------------------------------------------------

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
ENV TZ=America/Sao_Paulo

# Install git, process tools
RUN apt-get update && apt-get -y install git procps libc6-dev

# Clean up
RUN apt-get autoremove -y \
    && apt-get clean -y \
    && rm -rf /var/lib/apt/lists/*

# Install .NET Core Diagnostics Tools
RUN dotnet tool install --global dotnet-counters --version 3.1.141901 \
    && dotnet tool install --global dotnet-trace --version 3.1.141901 \
    && dotnet tool install --global dotnet-dump --version 3.1.141901 \
    && dotnet tool install -g dotnet-symbol


WORKDIR /build
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

ENV PATH="$PATH:~/.dotnet/tools"