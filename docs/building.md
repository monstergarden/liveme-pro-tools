## Building/Compiling LiveMe Pro Tools

### Prerequisits
- Dotnet Core SDK (https://dotnet.microsoft.com/download)
- NodeJS

### Verify everthing is installed
- Run "dotnet --version" , it should print the Dotnet Core version you have installed
- Run "node --version", it should print the NodeJs version you have installed

### Development Workflow
In the root directory of this repo run:
- "npm run watch" to start the dotnet core backend. It watches the files in ./src/core and if you do changes it will build and restart the backend.
- "npm run start" to run the electron frontend.

### Create a release
Run "npm run release" to create releases for all 3 plattforms. The zip files will be located in the ./dist folder

