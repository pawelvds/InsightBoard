import { useParams } from "react-router-dom"
import { useEffect, useState } from "react"
import api from "@/lib/api"
import { Note } from "@/hooks/useNotes"
import { Card } from "@/components/ui/card"

export default function PublicProfile() {
    const { username } = useParams()
    const [notes, setNotes] = useState<Note[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState("")

    useEffect(() => {
        async function fetchNotes() {
            try {
                const res = await api.get(`/notes/public/${username}`)
                setNotes(res.data)
            } catch {
                setError("Failed to load public notes")
            } finally {
                setLoading(false)
            }
        }

        fetchNotes()
    }, [username])

    return (
        <div className="max-w-4xl mx-auto p-6">
            <h1 className="text-3xl font-bold mb-4">@{username}'s public notes</h1>

            {loading && <p className="text-muted-foreground">Loading...</p>}
            {error && <p className="text-red-500">{error}</p>}

            {!loading && notes.length === 0 && (
                <p className="text-muted-foreground">This user has no public notes yet.</p>
            )}

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
        </div>
    )
}
