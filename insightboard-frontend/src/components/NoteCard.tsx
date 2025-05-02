import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Switch } from "@/components/ui/switch"
import { Note } from "@/hooks/useNotes"
import { Pencil, Trash, Clipboard } from "lucide-react"
import { format } from "date-fns"
import { toast } from "sonner"

interface NoteCardProps {
    note: Note
    onEdit: (note: Note) => void
    onDelete: (id: string) => void
    onTogglePublish?: (id: string, publish: boolean) => void
}

export function NoteCard({
                             note,
                             onEdit,
                             onDelete,
                             onTogglePublish,
                         }: NoteCardProps) {
    const handleCopy = async () => {
        await navigator.clipboard.writeText(note.content)
        toast.success("Note copied to clipboard")
    }

    const handleToggle = () => {
        if (onTogglePublish) {
            onTogglePublish(note.id, !note.published)
        }
    }

    return (
        <Card className="group relative transition-shadow hover:shadow-md">
            <CardHeader className="pb-2">
                <div className="flex items-center justify-between">
                    <div>
                        <CardTitle className="text-lg font-semibold">
                            {note.title}
                        </CardTitle>
                        <CardDescription className="text-xs">
                            Created: {format(new Date(note.createdAt), "dd MMM yyyy HH:mm")}
                        </CardDescription>
                    </div>
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
                        <Button
                            variant="ghost"
                            size="icon"
                            className="h-8 w-8"
                            onClick={handleCopy}
                        >
                            <Clipboard className="h-4 w-4" />
                        </Button>
                    </div>
                </div>
            </CardHeader>
            <CardContent className="text-sm text-muted-foreground whitespace-pre-wrap space-y-4">
                <p>{note.content}</p>
                <div className="flex items-center justify-between pt-2 border-t">
                    <span className="text-xs">Published</span>
                    <Switch
                        checked={note.published}
                        onCheckedChange={handleToggle}
                    />
                </div>
            </CardContent>
        </Card>
    )
}
