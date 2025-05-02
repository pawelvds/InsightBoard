import { useState } from "react"
import { useNavigate } from "react-router-dom"
import axios from "axios"

import { Card, CardHeader, CardTitle, CardContent, CardFooter } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Button } from "@/components/ui/button"

export default function Login() {
    const navigate = useNavigate()
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")
    const [error, setError] = useState("")

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setError("")
        try {
            const res = await axios.post("http://localhost:5085/api/auth/login", {
                email,
                password,
            })
            localStorage.setItem("token", res.data.token)
            localStorage.setItem("refreshToken", res.data.refreshToken)
            navigate("/dashboard")
        } catch {
            setError("Invalid credentials")
        }
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-background text-foreground px-4">
            <Card className="w-full max-w-md">
                <CardHeader>
                    <CardTitle className="text-2xl text-center">Sign in to your account</CardTitle>
                </CardHeader>
                <form onSubmit={handleSubmit}>
                    <CardContent className="space-y-4">
                        <div className="grid gap-2">
                            <Label htmlFor="email">Email</Label>
                            <Input
                                id="email"
                                type="email"
                                placeholder="you@example.com"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                            />
                        </div>
                        <div className="grid gap-2">
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
                    </CardContent>
                    <CardFooter className="flex flex-col gap-2">
                        <Button type="submit" className="w-full">Sign In</Button>
                        <p className="text-sm text-center text-muted-foreground">
                            Don't have an account? <span className="text-primary underline cursor-pointer">Create one</span>
                        </p>
                    </CardFooter>
                </form>
            </Card>
        </div>
    )
}
