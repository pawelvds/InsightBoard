import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Note } from "@/hooks/useNotes"
import { Pencil, Trash } from "lucide-react"

interface NoteCardProps {
    note: Note
    onEdit: (note: Note) => void
    onDelete: (id: string) => void
}

export function NoteCard({ note, onEdit, onDelete }: NoteCardProps) {
    return (
        <Card className="group relative transition-shadow hover:shadow-md">
            <CardHeader className="pb-2">
                <div className="flex items-center justify-between">
                    <CardTitle className="text-lg font-semibold">
                        {note.title}
                    </CardTitle>
                    <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                        <Button
                            variant="ghost"
                            size="icon"
                            className="h-8 w-8"
                            onClick={() => onEdit(note)}
                        >
                            <Pencil className="h-4 w-4" />
                        </Button>
                        <Button
                            variant="ghost"
                            size="icon"
                            className="h-8 w-8 text-red-500 hover:bg-red-50"
                            onClick={() => onDelete(note.id)}
                        >
                            <Trash className="h-4 w-4" />
                        </Button>
                    </div>
                </div>
            </CardHeader>
            <CardContent className="text-sm text-muted-foreground whitespace-pre-wrap">
                {note.content}
            </CardContent>
        </Card>
    )
}
