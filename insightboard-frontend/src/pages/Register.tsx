import { useState } from "react"
import { useNavigate, Link } from "react-router-dom"
import axios from "axios"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Button } from "@/components/ui/button"

export default function Register() {
    const navigate = useNavigate()
    const [username, setUsername] = useState("")
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")
    const [error, setError] = useState("")

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setError("")

        try {
            const res = await axios.post("http://localhost:5085/api/auth/register", {
                username,
                email,
                password,
            })
            localStorage.setItem("token", res.data.token)
            localStorage.setItem("refreshToken", res.data.refreshToken)
            navigate("/dashboard")
        } catch {
            setError("Registration failed. Please try again.")
        }
    }

    return (
        <div className="min-h-screen grid grid-cols-1 md:grid-cols-2">
            {/* Left section */}
            <div className="hidden md:flex items-center justify-center bg-primary text-primary-foreground p-10">
                <div className="max-w-md space-y-6 text-center">
                    <h2 className="text-4xl font-bold">InsightBoard</h2>
                    <p className="text-lg">Create an account and start organizing your notes effectively.</p>
                </div>
            </div>

            {/* Right section */}
            <div className="flex items-center justify-center p-6">
                <Card className="w-full max-w-sm shadow-lg">
                    <CardHeader>
                        <CardTitle className="text-2xl">Sign Up</CardTitle>
                    </CardHeader>
                    <form onSubmit={handleSubmit}>
                        <CardContent className="space-y-4">
                            <div className="space-y-2">
                                <Label htmlFor="username">Username</Label>
                                <Input
                                    id="username"
                                    type="text"
                                    value={username}
                                    onChange={(e) => setUsername(e.target.value)}
                                    required
                                />
                            </div>
                            <div className="space-y-2">
                                <Label htmlFor="email">Email</Label>
                                <Input
                                    id="email"
                                    type="email"
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                    required
                                />
                            </div>
                            <div className="space-y-2">
                                <Label htmlFor="password">Password</Label>
                                <Input
                                    id="password"
                                    type="password"
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    required
                                />
                            </div>
                            {error && <p className="text-sm text-red-500">{error}</p>}
                            <Button type="submit" className="w-full">
                                Create Account
                            </Button>

                            <p className="text-center text-sm text-muted-foreground mt-2">
                                Already have an account?{" "}
                                <Link to="/login" className="text-primary underline">
                                    Sign in
                                </Link>
                            </p>
                        </CardContent>
                    </form>
                </Card>
            </div>
        </div>
    )
}
