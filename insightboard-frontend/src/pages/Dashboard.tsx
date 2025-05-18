// src/pages/Dashboard.tsx
import { useState } from "react"
import { useNotes, Note } from "@/hooks/useNotes"
import { NewNoteDialog } from "@/components/NewNoteDialog"
import { NoteCard } from "@/components/NoteCard"
import { EditNoteDialog } from "@/components/EditNoteDialog"
import { Skeleton } from "@/components/ui/skeleton"
import { UserMenu } from "@/components/UserMenu"

export default function Dashboard() {
    const {
        notes,
        loading,
        error,
        refresh,
        deleteNote,
        toggleNoteVisibility,
    } = useNotes()

    const [editingNote, setEditingNote] = useState<Note | null>(null)
    const [editOpen, setEditOpen] = useState(false)

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
        <div className="min-h-screen px-6 py-10 bg-background text-foreground">
            <header className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 mb-10">
                <div>
                    <h1 className="text-3xl font-bold">Your Dashboard</h1>
                    <p className="text-muted-foreground text-sm">
                        Manage and organize your personal notes.
                    </p>
                </div>
                <div className="flex items-center gap-4">
                    <NewNoteDialog onNoteCreated={refresh} />
                    <UserMenu />
                </div>
            </header>

            {loading && (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                    {[...Array(3)].map((_, i) => (
                        <Skeleton key={i} className="h-36 w-full rounded-lg" />
                    ))}
                </div>
            )}

            {error && (
                <div className="text-red-500 text-sm mb-4">
                    Something went wrong: {error}
                </div>
            )}

            {!loading && notes.length === 0 && (
                <div className="border border-dashed rounded-lg p-6 text-center text-muted-foreground bg-muted/40">
                    You don't have any notes yet. Start by creating one!
                </div>
            )}

            {!loading && notes.length > 0 && (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                    {notes.map((note) => (
                        <NoteCard
                            key={note.id}
                            note={note}
                            onEdit={handleEdit}
                            onDelete={handleDelete}
                            onTogglePublish={toggleNoteVisibility}
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