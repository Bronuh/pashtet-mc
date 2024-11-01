# Stop everything
docker compose down

# Cleanup backend image
docker image remove mc-backend

# Cleanup mcserver config
rm ./data/mcserver/server.properties

# Update sources
git pull

# Rebuild and start everything
docker compose up -d