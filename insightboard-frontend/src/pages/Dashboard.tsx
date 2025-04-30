import { Button } from "../components/ui/button"
import { useNavigate } from "react-router-dom"
import { useEffect } from "react"

function Dashboard() {
    const navigate = useNavigate()

    useEffect(() => {
        const token = localStorage.getItem("token")
        if (!token) {
            navigate("/login")
        }
    }, [navigate])

    const handleLogout = () => {
        localStorage.removeItem("token")
        localStorage.removeItem("refreshToken")
        navigate("/login")
    }

    return (
        <div className="max-w-4xl mx-auto p-6">
            <div className="flex justify-between items-center mb-8">
                <h1 className="text-3xl font-bold">Your Dashboard</h1>
                <Button variant="destructive" onClick={handleLogout}>
                    Logout
                </Button>
            </div>

            <div className="border rounded-lg p-4 bg-muted/50 text-muted-foreground">
                Your notes will appear here soon 📒
            </div>
        </div>
    )
}

export default Dashboard