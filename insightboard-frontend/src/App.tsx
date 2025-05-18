// src/App.tsx
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Toaster } from "sonner";
import { AuthProvider } from "@/contexts/AuthContext";
import { Navbar } from "@/components/Navbar";
import { AuthGuard, ProtectedRoute } from "@/components/AuthGuard";
import HomePage from "@/pages/HomePage";
import Login from "@/pages/Login";
import Register from "@/pages/Register";
import Dashboard from "@/pages/Dashboard";
import PublicProfile from "@/pages/PublicProfile";
import "./App.css";

function App() {
    return (
        <AuthProvider>
            <Router>
                <div className="min-h-screen flex flex-col">
                    <Navbar />
                    <main className="flex-1">
                        <Routes>
                            <Route path="/" element={<HomePage />} />

                            <Route path="/login" element={
                                <AuthGuard>
                                    <Login />
                                </AuthGuard>
                            } />

                            <Route path="/register" element={
                                <AuthGuard>
                                    <Register />
                                </AuthGuard>
                            } />

                            <Route path="/dashboard" element={
                                <ProtectedRoute>
                                    <Dashboard />
                                </ProtectedRoute>
                            } />

                            <Route path="/user/:username" element={<PublicProfile />} />
                            <Route path="*" element={<div className="p-4 text-center">404 Not Found</div>} />
                        </Routes>
                    </main>
                    <footer className="border-t py-6 text-center text-sm text-muted-foreground">
                        &copy; {new Date().getFullYear()} InsightBoard. Wszystkie prawa zastrzeżone.
                    </footer>
                </div>
                <Toaster richColors position="top-center" />
            </Router>
        </AuthProvider>
    );
}

export default App;