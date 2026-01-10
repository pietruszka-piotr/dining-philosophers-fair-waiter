# wspolbiezne2

Prosta implementacja problemu **Uczty Filozofów** w C# z **kelnerem (waiter) i kolejką FIFO**. 
Zapewnia brak deadlocków i brak zagłodzenia. Symulacja trwa domyślnie 30 sekund i wypisuje statystyki.

## Jak uruchomić

### Visual Studio (Windows)
1. Otwórz `wspolbiezne2.sln` w Visual Studio 2022+.
2. Ustaw konfigurację `Debug` lub `Release` i naciśnij **Start**.

### .NET CLI (opcjonalnie)
```bash
cd wspolbiezne2
dotnet run -c Release
```

## Pliki
- `Program.cs` – punkt wejścia + definicje `Waiter`, `Philosopher`, `PhilosopherStats`.
- `wspolbiezne2.csproj` – projekt .NET 8 Console.
- `wspolbiezne2.sln` – plik rozwiązania Visual Studio.

## Parametry
- Liczbę filozofów (`n`) i czas symulacji możesz łatwo zmienić na początku `Main`.

## Uwagi implementacyjne
- Kelner (monitor) utrzymuje kolejkę FIFO chętnych i przydziela widelce tylko temu, kto jest na początku kolejki i ma oba widelce wolne.
- Synchronizacja na `lock`/`Monitor`; brak „busy waiting”.
- Każdy filozof trzyma lokalne liczniki (`ThinkCount`, `EatCount`) i drukuje podsumowanie po zakończeniu.