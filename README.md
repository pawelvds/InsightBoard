# 📜 InsightBoard

**InsightBoard** is a lightweight platform for managing and sharing personal notes.  
It supports private/public visibility, user authentication, and is built with a modern full-stack architecture.

---

## ✨ Features

- 🔒 **Secure Authentication** using JWT & Refresh Tokens
- 📝 **Create, Edit, Delete** notes with private/public visibility
- 🔁 **Toggle note visibility** with one click
- 🌍 **View public notes** via public user profiles
- 📄 **Simple, responsive UI** (React + Tailwind CSS)
- 🧠 **Planned: Rich Text Editing** (bold, lists, links etc.)
- 🚀 **Built with:** ASP.NET Core 9.0, EF Core, PostgreSQL

---

## ⚙️ Tech Stack

### 🔧 Backend
- ASP.NET Core 9.0
- Entity Framework Core
- AutoMapper
- PostgreSQL
- Docker (for DB)

### 💻 Frontend
- React (Vite)
- TypeScript
- Tailwind CSS
- Radix UI
- Sonner (toasts)

---

## 🏁 Getting Started

### 🔙 Backend
```bash
# 1. Clone the repository
git clone https://github.com/your-username/InsightBoard

# 2. Navigate to the backend
cd insightboard-backend

# 3. Configure database in appsettings.json

# 4. Apply migrations
dotnet ef database update

# 5. Run the server
dotnet run
```

### 💻 Frontend
```bash
# Navigate to frontend
cd insightboard-frontend

# Install dependencies
npm install

# Start dev server
npm run dev
```

---

## ✅ TODO (in progress)

- [ ] ✍️ Unit tests for `NoteService`
- [ ] 🧪 Add CI (GitHub Actions: test + lint)
- [ ] 🪄 Add Rich Text Editor (`@tiptap/react` or `Quill`)
- [ ] 📄 Public note detail page (e.g. `/note/:id`)
- [ ] 👁️ Note view counters (basic analytics)
- [ ] 🗃️ SQLite support for local/demo usage
- [ ] 🐳 Dockerize fullstack (backend + frontend)
- [ ] 🔒 Improve error handling (401/403 feedback)
- [ ] 🧰 Add ESLint/Prettier config for frontend

---

## 📈 Future Plans

- 🧵 Comments on public notes (basic thread support)
- 🎨 Themes/dark mode toggle
- 🛂 Admin dashboard (role-based management)
- 📦 Redis cache for public notes
- 📤 Note exporting (PDF/Markdown)
- 📊 Dashboard metrics (notes, views, likes)
- 👥 Followers/following system (optional)

---

## 📄 License

MIT