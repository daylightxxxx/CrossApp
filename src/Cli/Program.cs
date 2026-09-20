using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

Console.OutputEncoding = Encoding.UTF8;

var info = new EnvInfo(
    Student: "Скиба Максим, група ФЕІ-35",
    OsDescription: RuntimeInformation.OSDescription,
    OsVersion: Environment.OSVersion.ToString(),
    ProcessArchitecture: RuntimeInformation.ProcessArchitecture.ToString(),
    DotNetVersion: Environment.Version.ToString(),
    Runtime: RuntimeInformation.FrameworkDescription,
    AppDirectory: AppContext.BaseDirectory,
    CurrentDirectory: Environment.CurrentDirectory,
    Domain: "Замовлення (клієнти, товари, замовлення, рядки замовлення)");

if (args.Contains("--json"))
{
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    Console.WriteLine(JsonSerializer.Serialize(info, options));
}
else
{
    var line = new string('-', 52);
    Console.WriteLine("CrossApp – практикум з крос-платформного програмування");
    Console.WriteLine($"Студент: {info.Student}");
    Console.WriteLine(line);
    Console.WriteLine($"ОС (OSDescription): {info.OsDescription}");
    Console.WriteLine($"ОС (Environment)  : {info.OsVersion}");
    Console.WriteLine($"Архітектура процесу: {info.ProcessArchitecture}");
    Console.WriteLine($"Версія .NET (CLR) : {info.DotNetVersion}");
    Console.WriteLine($"Runtime           : {info.Runtime}");
    Console.WriteLine($"Каталог застосунку: {info.AppDirectory}");
    Console.WriteLine($"Поточний каталог  : {info.CurrentDirectory}");
    Console.WriteLine(line);
    Console.WriteLine($"Предметна область: {info.Domain}");
}

record EnvInfo(
    string Student,
    string OsDescription,
    string OsVersion,
    string ProcessArchitecture,
    string DotNetVersion,
    string Runtime,
    string AppDirectory,
    string CurrentDirectory,
    string Domain);