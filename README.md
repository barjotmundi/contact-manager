# Contact Manager (ASP.NET Core 8 MVC)

A simple Contact Manager web app built with **ASP.NET Core 8 MVC** and **AJAX** (front-end uses TypeScript + jQuery).  
It supports viewing, searching, adding, editing, and deleting contacts using an **in-memory** data store (no database).

## Features
- View all contacts (Name, Email, Phone)
- Search contacts by name or email (case-insensitive)
- Add a new contact (modal form)
- Edit an existing contact (modal form)
- Delete a contact
- All actions happen via AJAX (no full page reload)

## Tech Stack
- Back-end: ASP.NET Core 8 MVC (C#)
- Front-end: TypeScript + jQuery + Bootstrap
- Storage: In-memory repository
- Testing: xUnit + Moq

## Architecture
Layered approach:
- **Controller**: handles HTTP requests / returns views + partial HTML or JSON
- **Service**: business logic + validation (e.g., email/phone rules)
- **Repository**: in-memory data access

## Validation Rules
- Name is required
- Email is required and must be valid format
- Phone is required and must match: `(###)-###-####`

## Project Structure (example)
- `Controllers/`
- `Services/`
- `Repositories/`
- `Models/`
- `Dtos/`
- `Utilities/` (e.g., `OperationResult`)
- `Views/`
- `wwwroot/` (scripts/styles)
- `ContactManager.Tests/` (unit tests)



## Getting Started

### Prereqs

* .NET SDK (to run `dotnet`)
* Node.js + npm (to run `npm`)

### 1) Install front-end dependencies

Run this from the repo root (the folder that contains the `ContactManager` folder):

```bash
cd ContactManager
npm install
```

If you are already in `...\contact-manager\ContactManager>`, you can skip the `cd ContactManager` line and run:

```bash
npm install
```

### 2) Build TypeScript (outputs to `wwwroot/js`)

Run this from the `ContactManager` folder:

```bash
npm run build
```

### 3) Run the app

From the repo root:

```bash
cd ..
dotnet run --project ContactManager --launch-profile https
```

If you prefer HTTP only:

```bash
dotnet run --project ContactManager --launch-profile http
```

If you are already in the `ContactManager` folder, you can run:

```bash
dotnet run --launch-profile https
```

or:

```bash
dotnet run --launch-profile http
```

### 4) Open the app

After running, the terminal will print a line like:

* `Now listening on: https://localhost:7243`
  Open `https://localhost:7243/`

* `Now listening on: http://localhost:5297`
  Open `http://localhost:5297/`

Use the exact URL shown in your terminal.

### 5) Run tests

From the repo root:

```bash
dotnet test
```
