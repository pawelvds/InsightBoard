import { Card, CardHeader, CardTitle, CardContent, CardFooter } from "./ui/card"
import { Button } from "./ui/button"
import { Note } from "../hooks/useNotes"
import { format } from "date-fns"

interface Props {
    note: Note
    onEdit: (note: Note) => void
    onDelete: (id: string) => void
}

export function NoteCard({ note, onEdit, onDelete }: Props) {
    return (
        <Card className="flex flex-col justify-between shadow-sm border">
            <CardHeader className="pb-2">
                <CardTitle className="text-xl font-semibold">
                    {note.title}
                </CardTitle>
                <p className="text-sm text-muted-foreground">
                    {format(new Date(note.createdAt), "dd MMM yyyy, HH:mm")}
                </p>
            </CardHeader>

            <CardContent className="py-2">
                <p className="text-sm whitespace-pre-wrap text-muted-foreground">
                    {note.content || "No content."}
                </p>
            </CardContent>

            <CardFooter className="flex justify-end gap-2">
                <Button variant="outline" size="sm" onClick={() => onEdit(note)}>
                    Edit
                </Button>
                <Button variant="destructive" size="sm" onClick={() => onDelete(note.id)}>
                    Delete
                </Button>
            </CardFooter>
        </Card>
    )
}
