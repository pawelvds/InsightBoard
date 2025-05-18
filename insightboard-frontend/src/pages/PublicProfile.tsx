import { useParams, Link } from "react-router-dom"
import { useEffect, useState } from "react"
import axios from "axios"
import { useAuth } from "@/contexts/AuthContext"
import api from "@/lib/api"
import { Note } from "@/hooks/useNotes"
import { Card } from "@/components/ui/card"
import { UserMenu } from "@/components/UserMenu"
import { Button } from "@/components/ui/button"

export default function PublicProfile() {
    const { username } = useParams()
    const [notes, setNotes] = useState<Note[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState("")
    const { isAuthenticated } = useAuth()

    useEffect(() => {
        async function fetchNotes() {
            try {
                const res = await api.get(`/notes/public/${username}`)
                setNotes(res.data)
            } catch (err: unknown) {
                if (axios.isAxiosError(err)) {
                    if (err.response?.status === 404) {
                        setError("User not found.")
                    } else {
                        setError("Failed to load public notes.")
                    }
                } else {
                    setError("Unexpected error.")
                }
            } finally {
                setLoading(false)
            }
        }

        fetchNotes()
    }, [username])

    if (!loading && error) {
        return (
            <div className="max-w-4xl mx-auto p-6">
                {isAuthenticated && (
                    <div className="flex justify-end mb-6">
                        <UserMenu />
                    </div>
                )}
                <p className="text-red-500 text-center mt-6">{error}</p>
            </div>
        )
    }

    return (
        <div className="max-w-4xl mx-auto p-6">
            <div className="flex justify-between items-center mb-8">
                <h1 className="text-3xl font-bold">@{username}'s public notes</h1>

                {isAuthenticated && <UserMenu />}
            </div>

            {!isAuthenticated && (
                <div className="bg-muted/40 p-4 mb-6 rounded-lg border text-center">
                    <p className="mb-2">Want to create your own notes?</p>
                    <div className="flex justify-center gap-2">
                        <Button asChild size="sm" variant="outline">
                            <Link to="/login">Sign In</Link>
                        </Button>
                        <Button asChild size="sm">
                            <Link to="/register">Sign Up</Link>
                        </Button>
                    </div>
                </div>
            )}

            {loading && <p className="text-muted-foreground">Loading...</p>}

            {!loading && notes.length === 0 && (
                <div className="text-center my-12">
                    <p className="text-muted-foreground">
                        This user has no public notes yet.
                    </p>
                </div>
            )}

            <div className="grid gap-4">
                {notes.map((note) => (
                    <Card key={note.id} className="p-4">
                        <div className="mb-2">
                            <h2 className="text-xl font-semibold">{note.title}</h2>
                        </div>
                        <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                            {note.content}
                        </p>
                    </Card>
                ))}
            </div>
        </div>
    )
}