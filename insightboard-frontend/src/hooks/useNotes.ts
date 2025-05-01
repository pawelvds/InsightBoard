import { useCallback, useEffect, useState } from "react"
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

    const fetchNotes = useCallback(async () => {
        try {
            setLoading(true)
            const res = await api.get<Note[]>("/notes")
            setNotes(res.data)
        } catch {
            setError("Failed to load notes")
        } finally {
            setLoading(false)
        }
    }, [])

    useEffect(() => {
        fetchNotes()
    }, [fetchNotes])

    return { notes, loading, error, refresh: fetchNotes }}
