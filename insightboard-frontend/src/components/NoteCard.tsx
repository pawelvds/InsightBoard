import { Card } from "./ui/card"
import { Button } from "./ui/button"
import { Note } from "../hooks/useNotes"

interface Props {
    note: Note
    onEdit: (note: Note) => void
    onDelete: (id: string) => void
}

export function NoteCard({ note, onEdit, onDelete }: Props) {
    return (
        <Card className="p-4 relative">
            <div className="absolute top-2 right-2 flex gap-2">
                <Button variant="outline" size="sm" onClick={() => onEdit(note)}>
                    Edit
                </Button>
                <Button variant="destructive" size="sm" onClick={() => onDelete(note.id)}>
                    Delete
                </Button>
            </div>
            <h2 className="text-xl font-semibold">{note.title}</h2>
            <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                {note.content}
            </p>
        </Card>
    )
}
