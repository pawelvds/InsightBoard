import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Button } from "@/components/ui/button"

export default function Login() {
    return (
        <div className="flex items-center justify-center min-h-screen bg-background text-foreground">
            <Card className="w-[350px]">
                <CardHeader>
                    <CardTitle className="text-2xl">Tailwind test</CardTitle>
                </CardHeader>
                <div className="bg-card text-card-foreground p-4 rounded-lg shadow">
                    Test bg-card box
                </div>
                <CardContent className="space-y-4">
                    <div>
                        <Label htmlFor="email">Email</Label>
                        <Input id="email" type="email" placeholder="you@example.com" />
                    </div>
                    <div>
                        <Label htmlFor="password">Password</Label>
                        <Input id="password" type="password" />
                    </div>
                    <Button className="w-full">Sign In</Button>
                </CardContent>
            </Card>
        </div>
    )
}
