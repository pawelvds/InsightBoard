import { Link } from "react-router-dom";
import { useAuth } from "@/contexts/AuthContext";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";

export default function HomePage() {
    const { isAuthenticated, user } = useAuth();

    return (
        <div className="container max-w-6xl mx-auto px-4 py-12">
            <div className="text-center space-y-6 mb-12">
                <h1 className="text-4xl font-bold">Witaj w InsightBoard</h1>
                <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
                    Aplikacja do zarządzania notatkami i zapisywania swoich przemyśleń.
                </p>

                <div className="mt-8">
                    {isAuthenticated ? (
                        <div className="space-y-4">
                            <p className="text-lg">
                                Witaj z powrotem, <span className="font-semibold">{user?.username}</span>!
                            </p>
                            <Button asChild size="lg">
                                <Link to="/dashboard">Przejdź do swoich notatek</Link>
                            </Button>
                        </div>
                    ) : (
                        <div className="flex flex-col sm:flex-row justify-center gap-4">
                            <Button asChild size="lg">
                                <Link to="/login">Zaloguj się</Link>
                            </Button>
                            <Button asChild variant="outline" size="lg">
                                <Link to="/register">Zarejestruj się</Link>
                            </Button>
                        </div>
                    )}
                </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mt-12">
                <Card>
                    <CardContent className="pt-6">
                        <div className="space-y-2">
                            <h3 className="text-xl font-bold">Prywatne & Publiczne Notatki</h3>
                            <p className="text-muted-foreground">
                                Twórz notatki prywatne lub udostępniaj swoje przemyślenia publicznie.
                            </p>
                        </div>
                    </CardContent>
                </Card>

                <Card>
                    <CardContent className="pt-6">
                        <div className="space-y-2">
                            <h3 className="text-xl font-bold">Formatowanie Tekstu</h3>
                            <p className="text-muted-foreground">
                                Używaj bogatego edytora tekstu do formatowania swoich notatek.
                            </p>
                        </div>
                    </CardContent>
                </Card>

                <Card>
                    <CardContent className="pt-6">
                        <div className="space-y-2">
                            <h3 className="text-xl font-bold">Przeglądaj Publiczne Notatki</h3>
                            <p className="text-muted-foreground">
                                Odkrywaj przemyślenia innych użytkowników i inspiruj się.
                            </p>
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
}