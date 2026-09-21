# CrossApp

Наскрізний проєкт з крос-платформного програмування.
Предметна область: Замовлення. Сутності: Customer, Product, Order, OrderLine.
Призначення: оформлення замовлень і підрахунок їхніх сум.

## Запуск

    dotnet build
    dotnet run --project src/Cli

Вивід у форматі JSON (додаткове завдання):

dotnet run --project src/Cli -- --json

## Середовище

.NET SDK 10.0.401, Windows 11 Home x64

## Розмір publish (self-contained)

- win-x64: 153 МБ (161 139 922 байтів)
- linux-x64: 157 МБ (165 253 706 байтів)