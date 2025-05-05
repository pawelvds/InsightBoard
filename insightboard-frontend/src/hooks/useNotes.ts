import { useCallback, useEffect, useState } from "react"
import api from "../lib/api"

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
            console.error("Error fetching notes:", err)
        } finally {
            setLoading(false)
        }
    }, [])

    const deleteNote = useCallback(async (id: string) => {
        try {
            await api.delete(`/notes/${id}`)
            // Optymistycznie aktualizujemy stan
            setNotes(prev => prev.filter(note => note.id !== id))
        } catch (err) {
            console.error("Error deleting note:", err)
            setError("Failed to delete note")
        }
    }, [])

    const toggleNoteVisibility = useCallback(async (id: string, isPublic: boolean) => {
        console.log("TOGGLE VISIBILITY:", { id, isPublic })

        // Zapisujemy poprzedni stan na wypadek błędu
        const previousNotes = [...notes]

        // Optymistycznie aktualizujemy UI
        setNotes(prevNotes =>
            prevNotes.map(note =>
                note.id === id ? { ...note, isPublic } : note
            )
        )

        try {
            // Wysyłamy żądanie do API z jasno określonym typem boolean
            const response = await api.patch(
                `/notes/${id}/visibility`,
                { isPublic: Boolean(isPublic) },
                {
                    headers: {
                        "Content-Type": "application/json",
                    },
                }
            )

            console.log("API response:", response)
        } catch (err) {
            console.error("Error toggling note visibility:", err)

            // W przypadku błędu przywracamy poprzedni stan
            setNotes(previousNotes)

            // Informujemy użytkownika o błędzie
            setError("Failed to update note visibility")
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