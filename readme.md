# PassoCourseApp

A lightweight “learn & teach” platform built with **.NET 9** + **EF Core (PostgreSQL)** and **Angular 20**.  
Goal: let **Users** discover and enroll in courses while **Instructors** create and manage their own courses (lessons + quizzes). Authentication is JWT-based with role-aware UI/permissions.

---
## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) (comes with npm)
- [Angular CLI](https://angular.dev/tools/cli)
- PostgreSQL database (Neon/Postgres Cloud or local)

Optional for performance:
- [Redis](https://redis.io/) (see setup below)

---

## What you can do (Use Cases)

### As a User
- Browse public courses with instructor info, difficulty, duration.
- **Enroll / Unenroll** in a course (client guards prevent instructors from enrolling).
- Open a course “Outline” and **complete lessons & quizzes** one by one.
- See **My Learning** dashboard with progress bars, percentage and quick actions.

### As an Instructor
- Register as an Instructor and sign in.
- **Create, edit, delete** your courses (title, description, duration, difficulty).
- Add **ordered Lessons** (title + order) and **Quizzes** (title + order).
- Total counts are derived from content (no manual totals needed).
- Manage your course list; only owners can modify their courses.

### Platform Rules
- Role-based routes/menus:
  - Instructors cannot enroll; student dashboard is hidden for them.
  - Students can’t access instructor admin pages.

---

## Project Layout

```
/passo-course-be/src
  PassoCourseApp.Api/            # ASP.NET Core API (Swagger on dev)
  PassoCourseApp.Application/    # DTOs, interfaces, options
  PassoCourseApp.Domain/         # Entities + enums
  PassoCourseApp.Infrastructure/ # EF Core DbContext, Services, Migrations
  PassoCourseApp.UnitTest/       # xUnit tests (EFCore Sqlite in-memory)
/passo-course-ui                  # Angular 20 app (standalone components)
```

---

## Prerequisites

- **Backend**
  - .NET SDK **9.0.x**
  - PostgreSQL connection (Neon recommended)

- **Frontend**
  - Node **18+**
  - Angular CLI **20** (`npm i -g @angular/cli@20`) or use `npx`

---


## Database & Migrations (Cloud-friendly)

- Migrations have already been applied **once**, you do **not** need to run them again. Neon 
PostgreSQL DB used.

Commands (only when needed to add migrations):

```bash
# from repo root
dotnet ef migrations add <Name>   -p src/PassoCourseApp.Infrastructure/PassoCourseApp.Infrastructure.csproj   -s src/PassoCourseApp.Api/PassoCourseApp.Api.csproj

dotnet ef database update   -p src/PassoCourseApp.Infrastructure/PassoCourseApp.Infrastructure.csproj   -s src/PassoCourseApp.Api/PassoCourseApp.Api.csproj
```

---

# How to Run (dev)

## Running Redis (without Docker)

!!Note:  If redis server is not running, api endpoints will response slowly due to waiting for the redis response. If redis not running or responding, it’ll go to service not cache for a while. After 5 minutes, it’ll try to connect redis again.

### macOS (Homebrew)

```bash
brew install redis
brew services start redis
redis-cli ping   # should return PONG
```

### Windows (Option 1 – Memurai, Recommended)

- Download and install [Memurai](https://www.memurai.com/download) (a Redis-compatible server for Windows).  
- It runs as a Windows Service automatically after installation.  
- Test the connection:

```powershell
memurai-cli ping   # or redis-cli ping if CLI tools are installed
```

### Windows (Option 2 – WSL2 + Ubuntu)

- Enable WSL2 and install Ubuntu from the Microsoft Store.  
- Inside the Ubuntu terminal:

```bash
sudo apt update
sudo apt install -y redis-server
sudo service redis-server start
redis-cli ping   # should return PONG
```

- The Redis server will be accessible from Windows at `localhost:6379`.

---

### 1) Backend (API)
```bash
dotnet restore
dotnet run --project src/PassoCourseApp.Api/PassoCourseApp.Api.csproj --launch-profile https
```
- Swagger will be available in dev at `https://localhost:5235/swagger`.

### 2) Frontend (Angular)
```bash
cd passo-course-ui
npm install
ng serve -o
```
- Opens `http://localhost:4200`.

Open the UI, register/login, and start testing **user** vs **instructor** flows.

---

## Testing

### Backend: xUnit
- Uses **Sqlite in-memory** to run service tests against a real DbContext.
```bash
dotnet test src/PassoCourseApp.UnitTest/PassoCourseApp.UnitTest.csproj
```

### Frontend: Jasmine/Karma
- Unit tests for key services (Auth/Course) and components (optional).
```bash
cd passo-course-ui
ng test
```

---


## Users
- Some users/instructors have already set with some courses. You can also register or you can use this preset users:
- emilybrown@course.com / password: 123456 role: user
- joeknow@course.com / password: 123456 role: instructor
- nazantop@course.com / password: 123456 role: instructor
