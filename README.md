# wspolbiezne2

Prosta implementacja problemu **Uczty Filozofów** w C# z **kelnerem (waiter) i kolejką FIFO**. 
Kelner przydziela oba widelce jednocześnie, a kolejka FIFO daje pierwszeństwo wcześniejszym zgłoszeniom. Przy kończących się posiłkach i działającym planowaniu wątków eliminuje to cykliczne oczekiwanie i pozwala kolejnym filozofom robić postęp. Symulacja trwa domyślnie 30 sekund i wypisuje statystyki.

## Jak uruchomić

### Visual Studio (Windows)
1. Otwórz `wspolbiezne2.sln` w Visual Studio 2022+.
2. Ustaw konfigurację `Debug` lub `Release` i naciśnij **Start**.

### .NET CLI (opcjonalnie)
Wymagany SDK .NET 8 lub nowszy z obsługą aplikacji .NET 8. Z katalogu repozytorium:
```bash
dotnet build wspolbiezne2.sln
dotnet run --project wspolbiezne2/wspolbiezne2.csproj -c Release
```

## Pliki
- `wspolbiezne2/Program.cs` – punkt wejścia, metoda `Philosopher` i klasy `Waiter` oraz `Stats`.
- `wspolbiezne2/wspolbiezne2.csproj` – projekt .NET 8 Console.
- `wspolbiezne2.sln` – plik rozwiązania Visual Studio.

## Parametry
- Liczbę filozofów (`n`) i czas symulacji możesz łatwo zmienić na początku `Main`.

## Uwagi implementacyjne
- Kelner (monitor) utrzymuje kolejkę FIFO chętnych i przydziela widelce tylko temu, kto jest na początku kolejki i ma oba widelce wolne.
- Synchronizacja na `lock`/`Monitor`; brak „busy waiting”.
- Każdy filozof aktualizuje własne liczniki (`Stats.Think`, `Stats.Eat`); główny wątek drukuje podsumowanie po `Join`.
- Rezerwacja widelców implementuje `IDisposable`. `using` zwalnia widelce również przy anulowaniu tuż po ich przydzieleniu albo przy wyjątku.
- FIFO może zmniejszać równoległość: filozof na początku kolejki czeka na swoje widelce, nawet jeśli dalszy filozof mógłby już jeść.

## Testy

```bash
dotnet test wspolbiezne2.sln
```

Testy sprawdzają zwolnienie rezerwacji po anulowaniu oraz usunięcie anulowanego zgłoszenia z kolejki.
