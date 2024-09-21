## Available API endpoints

### Core
- `api/core/jre` - download Java packet in zip file
- `api/core/minecraft` - download Minecraft minimal required files packed in zip file

### Mods
- `api/mods/list` - returns list of mods with their checksums, required by minecraft client.
- `api/mods/download/{mod-file-name}` - downloads mod

### Directory
- `api/directory` - returns contents of root directory of filesystem
- `api/directory/{path}` - returns contents of specified directory
- `api/directory/download/{path}` - downloads specified file
- `api/directory/details/` - returns detailed info about files in the root directory
- `api/directory/details/{path}` - returns detailed info about files in the specified directory
- `api/directory/fileinfo/{path}` - returns detailed info about specified file
- `api/directory/tree/` - returns files tree, starting from root directory
- `api/directory/tree/{path}` - returns files tree, starting from specified directory