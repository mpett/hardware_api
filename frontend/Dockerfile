FROM microsoft/dotnet:2.0-sdk
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet add package RestSharp --version 105.2.3
RUN dotnet add package Newtonsoft.Json --version 10.0.3
RUN dotnet restore

# copy and build everything else
COPY . ./
RUN dotnet publish -c Release -o out
ENTRYPOINT ["frontend", "out/frontendapp.dll"]