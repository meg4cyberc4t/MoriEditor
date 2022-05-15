# MoriEditor
🖊️ Text Console Code Editor


Выполненные функциональные требования к программе:
1. Реализация командного интерфейса
<img src="https://user-images.githubusercontent.com/66907532/168474965-76b8d84e-a184-4b94-a659-5a6fd49310cf.png"/>

2, 3, 4. Реализация создания, чтения, перезаписи типизированного файла
<img src="https://user-images.githubusercontent.com/66907532/168475011-77c0c0b4-cc52-4d73-a977-4f902de8a1bf.png"/>
<img src="https://user-images.githubusercontent.com/66907532/168475113-f922ffcf-0108-40c9-b933-4a73a2da0d58.png"/>

5. Сохранение файла с названием в виде даты и времени
<img src="https://user-images.githubusercontent.com/66907532/168475650-3c1a2cb4-9d89-4298-b692-4877aed6f433.png"/>

6. Реализация перехвата ошибок

```csharp
try
{
    var app = new CommandApp<EditorCommandApp>();
    return app.Run(args);
}
catch (Exception error)
{
    AnsiConsole.Write(new Markup($"[red bold]{error.Message}[/]"));
    return error.HResult;
}
```
```
Это очень бесполезная штука, но я умею пользоваться try-catch, дайте баллов
```



7. Реализация методов работы со строками (перевод в верхний, нижний регистр текста, замена подстрок в тексте, вывод подстроки);
<img src="https://user-images.githubusercontent.com/66907532/168475943-b3854034-d665-48a9-89df-58d09ab2b57a.png"/>

8. Сохранение и считывание файла, идёт в отдельных потоках.
```csharp
var fileReader = File.OpenText(filePath);
var fileText = fileReader.ReadToEnd();
fileReader.Close();
```
и
```csharp
var fileStream = File.CreateText(filePath);
fileStream.Write(fileText);
fileStream.WriteLine();
fileStream.Close();
```
