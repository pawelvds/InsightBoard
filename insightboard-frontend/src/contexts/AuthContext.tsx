import { createContext, useContext, useState, useEffect, ReactNode } from "react";
import axios from "axios";
import { jwtDecode } from "jwt-decode";
import { toast } from "sonner";

interface User {
    id: string;
    username: string;
    email: string;
}

interface AuthContextType {
    isAuthenticated: boolean;
    user: User | null;
    loading: boolean;
    login: (credentials: { email: string; password: string }) => Promise<boolean>;
    register: (userData: { username: string; email: string; password: string }) => Promise<boolean>;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error("useAuth must be used within an AuthProvider");
    }
    return context;
};

interface AuthProviderProps {
    children: ReactNode;
}

export const AuthProvider = ({ children }: AuthProviderProps) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);

    const API_URL = "http://localhost:5085/api";

    const decodeToken = (token: string): User => {
        const decoded: any = jwtDecode(token);
        return {
            id: decoded.sub,
            username: decoded.unique_name,
            email: decoded.email
        };
    };

    useEffect(() => {
        const checkAuth = () => {
            const token = localStorage.getItem("token");
            if (!token) {
                setLoading(false);
                return;
            }

            try {
                const decodedToken: any = jwtDecode(token);
                const currentTime = Date.now() / 1000;

                if (decodedToken.exp < currentTime) {
                    logout();
                } else {
                    // Token jest ważny
                    setUser(decodeToken(token));
                    setIsAuthenticated(true);
                }
            } catch (error) {
                console.error("Token validation error:", error);
                logout();
            }

            setLoading(false);
        };

        checkAuth();
    }, []);

    useEffect(() => {
        if (!isAuthenticated || !user) return;

        const token = localStorage.getItem("token");
        if (!token) return;

        try {
            const decodedToken: any = jwtDecode(token);
            const expirationTime = decodedToken.exp * 1000;
            const currentTime = Date.now();

            const timeToRefresh = expirationTime - currentTime - 300000;

            if (timeToRefresh <= 0) {
                refreshToken();
            } else {
                const timeoutId = setTimeout(() => {
                    refreshToken();
                }, timeToRefresh);

                return () => clearTimeout(timeoutId);
            }
        } catch (error) {
            console.error("Error scheduling token refresh:", error);
        }
    }, [isAuthenticated, user]);

    const login = async (credentials: { email: string; password: string }): Promise<boolean> => {
        try {
            const response = await axios.post(`${API_URL}/auth/login`, credentials);

            const { token, refreshToken } = response.data;

            localStorage.setItem("token", token);
            localStorage.setItem("refreshToken", refreshToken);

            setUser(decodeToken(token));
            setIsAuthenticated(true);

            return true;
        } catch (error) {
            console.error("Login error:", error);
            return false;
        }
    };

    const register = async (userData: { username: string; email: string; password: string }): Promise<boolean> => {
        try {
            const response = await axios.post(`${API_URL}/auth/register`, userData);

            const { token, refreshToken } = response.data;

            localStorage.setItem("token", token);
            localStorage.setItem("refreshToken", refreshToken);

            setUser(decodeToken(token));
            setIsAuthenticated(true);

            return true;
        } catch (error) {
            console.error("Registration error:", error);
            return false;
        }
    };

    const logout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("refreshToken");
        setUser(null);
        setIsAuthenticated(false);
    };

    const refreshToken = async () => {
        try {
            const refreshTokenValue = localStorage.getItem("refreshToken");
            if (!refreshTokenValue) {
                logout();
                return;
            }

            const response = await axios.post(`${API_URL}/auth/refresh-token`, {
                refreshToken: refreshTokenValue
            });

            const { token, refreshToken: newRefreshToken } = response.data;

            localStorage.setItem("token", token);
            localStorage.setItem("refreshToken", newRefreshToken);

            setUser(decodeToken(token));

        } catch (error) {
            console.error("Token refresh error:", error);
            toast.error("Session expired. Log in again");
            logout();
        }
    };

    const value = {
        isAuthenticated,
        user,
        loading,
        login,
        register,
        logout
    };

    return (
        <AuthContext.Provider value={value}>
            {!loading && children}
        </AuthContext.Provider>
    );
};