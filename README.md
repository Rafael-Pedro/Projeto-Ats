# ATS MVP

Sistema de Applicant Tracking System (ATS).

## Tecnologias

- **Backend:** .NET 8, Clean Architecture, DDD, CQRS, MongoDB.
- **Frontend:** Angular 19, PO-UI (TOTVS), Standalone Components.
- **Infra:** Docker, Nginx.

## Como rodar

Pré-requisito: Ter o **Docker** instalado.

1. Clone o repositório.
2. Na raiz do projeto, execute:

docker-compose up --build

3. Acesse:
- **Frontend:** http://localhost:4200
- **API (Swagger):** http://localhost:5000/swagger

## Funcionalidades
- CRUD Completo de Candidatos.
- CRUD Completo de Vagas.
- Candidato pode se candidatar a uma vaga.
- Listagem de candidatos em uma vaga.
- Upload de arquivo de currículo.
- Soft Delete e Paginação.
- Testes Unitários no Backend.
