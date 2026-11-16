# CrossApp

Наскрізний проєкт з крос-платформного програмування.
Предметна область: Замовлення. Сутності: Customer, Product, Order, OrderLine.
Призначення: оформлення замовлень і підрахунок їхніх сум.

## Запуск

    dotnet build
    dotnet run --project src/Cli

Cli демонструє два сценарії: успішну роботу із замовленням і спроби порушити інваріанти.

## Середовище

.NET SDK 10.0.401, Windows 11 Home x64 (RID: win-x64)

## Формат даних

Файли в `data/`: CSV з роздільником `;`, кодування UTF-8, заголовок `id;name;price`; JSON з тими самими полями.

## Інваріанти доменної моделі

1. Ідентифікатор замовлення не порожній (`ArgumentException`, `Order.Create`).
2. Ідентифікатор клієнта не порожній (`ArgumentException`, `Order.Create`).
3. Ідентифікатор і назва товару в рядку не порожні (`ArgumentException`, `Order.AddLine`).
4. Кількість у рядку більша за нуль (`ArgumentOutOfRangeException`, `Order.AddLine`).
5. Ціна не від'ємна (`ArgumentOutOfRangeException`, `Order.AddLine`).
6. До підтвердженого замовлення не можна додавати рядки (`InvalidOperationException`, `Order.AddLine`).
7. Порожнє замовлення не можна підтвердити (`InvalidOperationException`, `Order.Confirm`).
8. Підтверджене замовлення не можна підтвердити повторно (`InvalidOperationException`, `Order.Confirm`).

`Order.FromDto` відновлює замовлення через `Create`, `AddLine` і `Confirm`, тому проходить ті самі перевірки.

## Публікація

Логіку збору інформації про середовище винесено в бібліотеку `Core`, `Cli` лише форматує вивід (ProjectReference Cli → Core).

    dotnet publish src/Cli -c Release -r win-x64 --self-contained true  -o publish-out/win-x64-self
    dotnet publish src/Cli -c Release -r win-x64 --self-contained false -o publish-out/win-x64-framework
    dotnet publish src/Cli -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish-out/win-x64-singlefile
    dotnet publish src/Cli -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -o publish-out/win-x64-trimmed

Запуск без dotnet run: `.\publish-out\win-x64-self\Cli.exe`

| Режим                 | Розмір каталогу        | Файлів | Потрібен runtime |
|-----------------------|-------------------------|--------|-------------------|
| framework-dependent   | 0,19 МБ (200 186 б)     | 7      | Так (.NET 10)     |
| self-contained        | 76,9 МБ (80 584 538 б)  | 194    | Ні                |
| Single-File           | 70,1 МБ (73 559 136 б)  | 3      | Ні                |
| Trimmed (Single-File) | 12,4 МБ (12 972 017 б)  | 3      | Ні                |

Self-contained містить копію .NET runtime, тому значно більший, але не вимагає встановленого .NET на цільовій машині. PublishSingleFile об'єднує всі збірки в один файл. PublishTrimmed додатково вирізає невикористаний код і дає найбільшу економію розміру (у цьому проєкті без попереджень компілятора, бо код не використовує рефлексію чи серіалізацію).

## Multi-targeting

Core і Cli збираються під `net8.0` та `net10.0` (`<TargetFrameworks>` в обох `.csproj`). Спроба зробити multi-target лише Core давала помилку `NU1201`, бо Cli лишався на `net10.0`.