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



## How to Run

> Note: the front end tooling (npm, tsconfig, package.json) lives in `./ContactManager` (the project folder with `ContactManager.csproj` and `wwwroot`)

### 1) Install front end dependencies

```bash
cd ContactManager
npm install
```

### 2) Build TypeScript (outputs to wwwroot/js)

```bash
npm run build
```

### 3) Run the app

From the repo root

```bash
cd ..
dotnet run --project ContactManager --launch-profile https
```

If you prefer HTTP only

```bash
dotnet run --project ContactManager --launch-profile http
```

### 4) Run tests

```bash
dotnet test

That’s it. If you do not want to add the launch profile lines, your version is still totally fine.
```