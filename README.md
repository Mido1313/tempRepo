# Finance & Zakat Manager

Finance & Zakat Manager is a full-stack personal finance and zakat tracking platform. It provides secure account management, budgeting, transaction history, and automated zakat calculations using gold or silver nisab.

## Features
- ASP.NET Core Web API (.NET 8) with PostgreSQL and Entity Framework Core
- Angular 17 frontend with ngx-charts visualisations
- Keycloak integration for OpenID Connect authentication
- Structured logging with Serilog and OpenTelemetry tracing
- Docker images for API and frontend plus docker-compose for local development
- Kubernetes manifests for production deployments
- GitHub Actions pipeline for CI and container publishing

## Prerequisites
- .NET 8 SDK
- Node.js 20+
- Docker

## Local development
### Backend
```bash
dotnet restore
dotnet ef database update --project src/Infrastructure/FinanceZakatManager.Infrastructure.csproj --startup-project src/Api/FinanceZakatManager.Api.csproj
dotnet run --project src/Api/FinanceZakatManager.Api.csproj
```

### Frontend
```bash
cd frontend
npm install
npm start
```

### Docker Compose
```bash
docker compose up --build
```
API will be available on `http://localhost:8080` and the Angular app on `http://localhost:4200`.

## Testing
Run backend and frontend tests with:
```bash
dotnet test
cd frontend && npm test
```

## Kubernetes deployment
Apply the manifests in `k8s/` after updating image references and TLS configuration:
```bash
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/postgres-statefulset.yaml
kubectl apply -f k8s/api-deployment.yaml
kubectl apply -f k8s/frontend-deployment.yaml
kubectl apply -f k8s/hpa.yaml
kubectl apply -f k8s/ingress.yaml
```

## OpenAPI specification
A reference OpenAPI document is included at `openapi/openapi.json`. The GitHub Actions pipeline builds the API and can be extended to export the live swagger document.

## License
MIT
