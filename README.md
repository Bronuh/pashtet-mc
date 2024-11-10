# Private Minecraft Server Stack

> A monorepo containing source code for various projects used to create the infrastructure for private Minecraft servers.

## Components

- **PashtetMcBackend/** - The web backend infrastructure responsible for the automated downloading, installation, and updating of client components.
- **Launcher/** - The launcher responsible for downloading, installing, updating, and running the client-side game.
- **Common/** - A shared components library for both the launcher and the backend.

## Stack

- **[C# 12 / .NET 8](https://dotnet.microsoft.com/en-us/download)** - The primary programming language.
- **[ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet)** - The platform for server-side development.
- **[Docker Compose](https://docs.docker.com/compose/)** - Used for containerizing the server-side components.
- **[Godot Engine 4.3 - .NET](https://godotengine.org/download/windows/)** - Used for client-side (launcher) development.

## Deployment of the Server-Side
To launch the Minecraft server and the ASP.NET Web API backend, you need to:
- Clone this repository into the working directory of your Linux server.
- In `docker-compose.yml`, configure the UID of the user under which the `mcserver` service will run.
- Configure the services in the `.env` file or in the `docker-compose.yml` file.
- Then, proceed to launch the server and backend. This can be done in two ways:
  - Automatic: Run `update.sh`. The script will prepare the environment and start the required containers.
  - Manual:
     - Create the directories `./data/mcserver` and `./data/storage`.
     - Ensure all directories have read and write permissions for the user running the containers.
     - Run `docker compose up -d` from the root of the repository.

## Updating
You can make changes to the repository (if you have forked it) and update it on the server by running `update.sh` again.
