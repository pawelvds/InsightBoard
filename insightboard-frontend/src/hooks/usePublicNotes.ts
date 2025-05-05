import { useEffect, useState } from "react"
import api from "../lib/api"

export interface PublicNote {
    id: string
    title: string
    content: string
    createdAt: string
}

export function usePublicNotes(username: string) {
    const [notes, setNotes] = useState<PublicNote[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        const fetchNotes = async () => {
            try {
                setLoading(true)
                const res = await api.get<PublicNote[]>(`/notes/public/${username}`)
                setNotes(res.data)
            } catch (err) {
                setError("Failed to fetch public notes")
            } finally {
                setLoading(false)
            }
        }

        if (username) {
            fetchNotes()
        }
    }, [username])

    return { notes, loading, error }
}
