import { useCallback, useEffect, useState } from "react"
import api from "../lib/api"
import { toast } from "sonner"

export interface Note {
    id: string
    title: string
    content: string
    createdAt: string
    isPublic: boolean
}

export function useNotes() {
    const [notes, setNotes] = useState<Note[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    const fetchNotes = useCallback(async () => {
        try {
            setLoading(true)
            const res = await api.get<Note[]>("/notes")
            setNotes([...res.data])
        } catch (err) {
            setError("Failed to load notes")
        } finally {
            setLoading(false)
        }
    }, [])

    const deleteNote = useCallback(async (id: string) => {
        try {
            await api.delete(`/notes/${id}`)
            setNotes(prev => prev.filter(note => note.id !== id))

            toast.success("Note deleted successfully")
        } catch (err) {
            setError("Failed to delete note")

            toast.error("Failed to delete note")
        }
    }, [])

    const toggleNoteVisibility = useCallback(async (id: string, isPublic: boolean) => {
        const noteToUpdate = notes.find(note => note.id === id)
        if (!noteToUpdate) return

        const previousNotes = [...notes]

        setNotes(prevNotes =>
            prevNotes.map(note =>
                note.id === id ? { ...note, isPublic } : note
            )
        )

        try {
            // Wyślij żądanie do API
            await api.patch(
                `/notes/${id}/visibility`,
                { isPublic: Boolean(isPublic) },
                {
                    headers: {
                        "Content-Type": "application/json",
                    },
                }
            )

        } catch (err) {
            setNotes(previousNotes)

            toast.error(`Failed to ${isPublic ? 'publish' : 'unpublish'} note`)
        }
    }, [notes])

    useEffect(() => {
        fetchNotes()
    }, [fetchNotes])

    return {
        notes,
        loading,
        error,
        refresh: fetchNotes,
        deleteNote,
        toggleNoteVisibility,
    }
}