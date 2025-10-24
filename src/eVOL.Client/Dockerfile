# Stage 1: Build the Blazor WebAssembly app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and restore
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o /out

# Stage 2: Serve with Nginx
FROM nginx:alpine
COPY --from=build /out/wwwroot /usr/share/nginx/html

# Expose port 80 for the web app
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
