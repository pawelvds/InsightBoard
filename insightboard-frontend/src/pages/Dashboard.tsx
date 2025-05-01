import { useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { Button } from "../components/ui/button"
import { Card } from "../components/ui/card"
import { useNotes } from "../hooks/useNotes"
import { NewNoteDialog } from "../components/NewNoteDialog"

function Dashboard() {
    const navigate = useNavigate()
    const { notes, loading, error } = useNotes()

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
                <div className="flex gap-2">
                    <NewNoteDialog onNoteCreated={() => window.location.reload()} />
                    <Button variant="destructive" onClick={handleLogout}>
                        Logout
                    </Button>
                </div>
            </div>

            {loading && (
                <div className="text-muted-foreground">Loading notes...</div>
            )}

            {error && (
                <div className="text-red-500">Something went wrong: {error}</div>
            )}

            {!loading && notes.length === 0 && (
                <div className="border rounded-lg p-4 bg-muted/50 text-muted-foreground">
                    You don’t have any notes yet.
                </div>
            )}

            {!loading && notes.length > 0 && (
                <div className="grid gap-4">
                    {notes.map((note) => (
                        <Card key={note.id} className="p-4">
                            <h2 className="text-xl font-semibold">{note.title}</h2>
                            <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                                {note.content}
                            </p>
                        </Card>
                    ))}
                </div>
            )}
        </div>
    )
}

export default Dashboard
