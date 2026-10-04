# CrossApp

Наскрізний проєкт з крос-платформного програмування.
Предметна область: Замовлення. Сутності: Customer, Product, Order, OrderLine.
Призначення: оформлення замовлень і підрахунок їхніх сум.

## Запуск

    dotnet build
    dotnet run --project src/Cli

## Середовище

.NET SDK 10.0.401, Windows 11 Home x64 (RID: win-x64)

## Публікація

Логіку збору інформації про середовище винесено в бібліотеку `Core`,
`Cli` лише форматує вивід (ProjectReference Cli → Core).

    dotnet publish src/Cli -c Release -r win-x64 --self-contained true  -o publish-out/win-x64-self
    dotnet publish src/Cli -c Release -r win-x64 --self-contained false -o publish-out/win-x64-framework
    dotnet publish src/Cli -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish-out/win-x64-singlefile
    dotnet publish src/Cli -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -o publish-out/win-x64-trimmed

Запуск без dotnet run: `.\publish-out\win-x64-self\Cli.exe`

| Режим                | Розмір каталогу        | Файлів | Потрібен runtime |
|----------------------|--------------------------|--------|-------------------|
| framework-dependent  | 0,19 МБ (200 186 б)     | 7      | Так (.NET 10)     |
| self-contained       | 76,9 МБ (80 584 538 б)  | 194    | Ні                |
| Single-File          | 70,1 МБ (73 559 136 б)  | 3      | Ні                |
| Trimmed (Single-File)| 12,4 МБ (12 972 017 б)  | 3      | Ні                |

Self-contained містить копію .NET runtime, тому значно більший, але не
вимагає встановленого .NET на цільовій машині. PublishSingleFile об'єднує
всі збірки в один файл. PublishTrimmed додатково вирізає невикористаний
код і дає найбільшу економію розміру (у цьому проєкті без попереджень
компілятора, бо код не використовує рефлексію чи серіалізацію).

## Multi-targeting

Спроба зробити Core multi-target (`net8.0;net10.0`) дала помилку `NU1201`,
бо Cli (ProjectReference на Core) лишається на net10.0. Залишено лише
`<TargetFramework>net10.0</TargetFramework>`.