import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from "sonner";
import { AuthProvider } from "@/contexts/AuthContext";
import { AuthGuard, ProtectedRoute } from "@/components/AuthGuard";
import Login from "@/pages/Login";
import Register from "@/pages/Register";
import Dashboard from "@/pages/Dashboard";
import PublicProfile from "@/pages/PublicProfile";
import { useAuth } from "@/contexts/AuthContext";

function RedirectRoute() {
    const { isAuthenticated } = useAuth();
    return <Navigate to={isAuthenticated ? "/dashboard" : "/login"} replace />;
}

function App() {
    return (
        <AuthProvider>
            <Router>
                <div className="min-h-screen flex flex-col">
                    <main className="flex-1">
                        <Routes>
                            <Route path="/" element={<RedirectRoute />} />

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
                </div>
                <Toaster richColors position="top-center" />
            </Router>
        </AuthProvider>
    );
}

export default App;