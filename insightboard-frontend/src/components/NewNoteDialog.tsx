import { useState } from "react"
import {
    Dialog,
    DialogTrigger,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
} from "./ui/dialog"
import { Button } from "./ui/button"
import { Input } from "./ui/input"
import { Textarea } from "./ui/textarea"
import api from "../lib/api"
import { toast } from "sonner"

interface Props {
    onNoteCreated: () => void
}

export function NewNoteDialog({ onNoteCreated }: Props) {
    const [open, setOpen] = useState(false)
    const [title, setTitle] = useState("")
    const [content, setContent] = useState("")
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState("")

    const handleCreate = async () => {
        if (!title.trim()) {
            setError("Title is required")
            return
        }

        try {
            setLoading(true)
            await api.post("/notes", { title, content })

            toast.success("Note created successfully!")
            setOpen(false)
            setTitle("")
            setContent("")
            setError("")
            onNoteCreated()
        } catch {
            setError("Something went wrong. Please try again.")
        } finally {
            setLoading(false)
        }
    }

    return (
        <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
                <Button className="mb-4">+ New Note</Button>
            </DialogTrigger>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Create a new note</DialogTitle>
                </DialogHeader>

                <div className="grid gap-4 py-4">
                    <Input
                        placeholder="Note title"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                    />
                    <Textarea
                        placeholder="Note content (optional)"
                        value={content}
                        onChange={(e) => setContent(e.target.value)}
                    />
                    {error && <p className="text-sm text-red-500">{error}</p>}
                </div>

                <DialogFooter>
                    <Button disabled={loading} onClick={handleCreate}>
                        {loading ? "Creating..." : "Create"}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}
