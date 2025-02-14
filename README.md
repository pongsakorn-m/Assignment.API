# .NET API with PostgreSQL and Docker

This project is a .NET API that connects to a PostgreSQL database and is designed to run in a Docker container.

## Getting Started

1. run build command from docker-compose.yaml by default.
   ````bash
   docker compose build
2. run this command for use docker images that you build to run a container.
   ````bash
   docker compose up

If you don't nedd to install Docker, Just change connection to your DB connection and run initial script from file <b>init.sql<b/>
