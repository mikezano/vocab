# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Runtime stage (Nginx serving static files)
FROM nginx:alpine
COPY --from=build /app/out/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

# Optional: Remove default Nginx index page
#RUN rm /usr/share/nginx/html/index.html 2>/dev/null || true

EXPOSE 80