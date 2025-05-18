// src/components/Navbar.tsx
import { Link } from "react-router-dom"
import { useAuth } from "@/contexts/AuthContext"
import { Button } from "@/components/ui/button"

export function Navbar() {
    const { isAuthenticated, user, logout } = useAuth()

    const handleLogout = () => {
        logout()
    }

    return (
        <header className="border-b">
            <div className="container flex h-16 items-center justify-between px-4">
                <div className="flex items-center gap-6">
                    <Link to="/" className="flex items-center font-bold text-xl">
                        InsightBoard
                    </Link>

                    <nav className="hidden md:flex items-center gap-4">
                        <Link to="/" className="text-sm font-medium hover:text-primary">
                            Home
                        </Link>
                        {isAuthenticated && (
                            <Link to="/dashboard" className="text-sm font-medium hover:text-primary">
                                Dashboard
                            </Link>
                        )}
                    </nav>
                </div>

                <div className="flex items-center gap-4">
                    {isAuthenticated ? (
                        <div className="flex items-center gap-4">
                            <span className="text-sm hidden md:inline-block">
                                Witaj, <span className="font-medium">{user?.username}</span>
                            </span>
                            <Button variant="ghost" size="sm" onClick={handleLogout}>
                                Wyloguj
                            </Button>
                        </div>
                    ) : (
                        <>
                            <Link to="/login">
                                <Button variant="outline" size="sm">Zaloguj</Button>
                            </Link>
                            <Link to="/register">
                                <Button size="sm">Zarejestruj</Button>
                            </Link>
                        </>
                    )}
                </div>
            </div>
        </header>
    )
}