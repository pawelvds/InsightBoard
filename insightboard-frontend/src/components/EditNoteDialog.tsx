import { useEffect, useState } from "react"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "./ui/dialog"
import { Input } from "./ui/input"
import { Textarea } from "./ui/textarea"
import { Button } from "./ui/button"
import { Note } from "../hooks/useNotes"
import api from "../lib/api"

interface Props {
  note: Note | null
  open: boolean
  onClose: () => void
  onNoteUpdated: () => void
}

export function EditNoteDialog({ note, open, onClose, onNoteUpdated }: Props) {
  const [title, setTitle] = useState("")
  const [content, setContent] = useState("")
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")

  useEffect(() => {
    if (note) {
      setTitle(note.title)
      setContent(note.content)
      setError("")
    }
  }, [note])

  const handleSave = async () => {
    if (!note || !title.trim()) {
      setError("Title is required")
      return
    }

    try {
      setLoading(true)
      await api.put(`/notes/${note.id}`, { title, content })
      onNoteUpdated()
      onClose()
    } catch {
      setError("Something went wrong. Please try again.")
    } finally {
      setLoading(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Edit Note</DialogTitle>
        </DialogHeader>

        <div className="space-y-4">
          <Input
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Note title"
          />
          <Textarea
            value={content}
            onChange={(e) => setContent(e.target.value)}
            placeholder="Note content"
          />
          {error && <p className="text-sm text-red-500">{error}</p>}
        </div>

        <DialogFooter className="mt-4">
          <Button disabled={loading} onClick={handleSave}>
            {loading ? "Saving..." : "Save"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
