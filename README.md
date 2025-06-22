# Alamanda Management

This is a fullstack project including a .NET backend, a desktop app with Electron, and a mobile app using React Native.

## Project Structure

- **AlamandaApi**: Backend built with .NET 8.
- **AlamandaManagerDesktop**: Desktop application using Electron and VueJS.
- **AlamandaApp**: Mobile application built with React Native.

---

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Node.js 22+](https://nodejs.org/) 
- MongoDB

---

## Setup

1. Copy `.env.example` to `.env` in projects that use environment variables, and configure the necessary values.
2. Install dependencies by running the commands below (npm run i:all).

---
### Install dependencies (Use npm run i:all at first)

```bash
npm run i:back       # Restore backend (.NET) packages
npm run i:desktop    # Install Electron desktop dependencies
npm run i:mobile     # Install React Native mobile dependencies
npm run i:all        # Install all dependencies at once (backend, desktop, and mobile)
```

---
3. Start the project!
```bash
npm run start:backend      # Start backend with hot reload (dotnet watch)
npm run start:electron     # Start Electron desktop app
npm run start:react-native # Start React Native mobile app

npm run desktop  # Run backend + desktop simultaneously
npm run mobile   # Run backend + mobile simultaneously
```