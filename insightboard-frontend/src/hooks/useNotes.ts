import { useEffect, useState } from "react"
import api from "../lib/api"

export interface Note {
    id: string
    title: string
    content: string
    createdAt: string
}

export function useNotes() {
    const [notes, setNotes] = useState<Note[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        async function fetchNotes() {
            try {
                const res = await api.get<Note[]>("/notes")
                setNotes(res.data)
            } catch {
                setError("Failed to load notes")
            } finally {
                setLoading(false)
            }
        }

        fetchNotes()
    }, [])

    return { notes, loading, error }
}
