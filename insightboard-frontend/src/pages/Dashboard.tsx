import { useEffect, useState } from "react"
import { useNavigate } from "react-router-dom"
import { Button } from "../components/ui/button"
import { useNotes, Note } from "../hooks/useNotes"
import { NewNoteDialog } from "../components/NewNoteDialog"
import { NoteCard } from "../components/NoteCard"
import { EditNoteDialog } from "../components/EditNoteDialog"

function Dashboard() {
    const navigate = useNavigate()
    const { notes, loading, error, refresh, deleteNote } = useNotes()

    const [editingNote, setEditingNote] = useState<Note | null>(null)
    const [editOpen, setEditOpen] = useState(false)

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

    const handleEdit = (note: Note) => {
        setEditingNote(note)
        setEditOpen(true)
    }

    const handleDelete = async (id: string) => {
        const confirmed = window.confirm("Are you sure you want to delete this note?")
        if (!confirmed) return

        try {
            await deleteNote(id)
        } catch (err) {
            console.error("Failed to delete note", err)
        }
    }

    return (
        <div className="max-w-4xl mx-auto p-6">
            <div className="flex justify-between items-center mb-8">
                <h1 className="text-3xl font-bold">Your Dashboard</h1>
                <div className="flex gap-2">
                    <NewNoteDialog onNoteCreated={refresh} />
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
                        <NoteCard
                            key={note.id}
                            note={note}
                            onEdit={handleEdit}
                            onDelete={handleDelete}
                        />
                    ))}
                </div>
            )}

            <EditNoteDialog
                note={editingNote}
                open={editOpen}
                onClose={() => setEditOpen(false)}
                onNoteUpdated={refresh}
            />
        </div>
    )
}

export default Dashboard
