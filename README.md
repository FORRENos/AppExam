# AppExam

WPF-приложение на C# для модулей 2-3 демоэкзамена.

## Что есть в проекте

- авторизация по ролям;
- гостевой вход;
- просмотр списка товаров из БД;
- поиск по товарам;
- сортировка по цене и остатку;
- фильтр по размеру скидки;
- добавление, редактирование и удаление товаров для администратора;
- подсветка товаров со скидкой больше 25%;
- подсветка товаров, которых нет на складе.

## Как открыть

Открыть в Visual Studio 2022 файл:

```text
AppExam.sln
```

## База данных

Файл скрипта для SSMS:

```text
AppExamDb.sql
```

Скрипт создает:

- базу `AppExamDb`;
- таблицы;
- роли;
- тестовых пользователей;
- начальные товары.

В SSMS сервер для LocalDB:

```text
(localdb)\AppExamSQL
```

В приложении используется строка подключения:

```text
Server=(localdb)\AppExamSQL;Database=AppExamDb;Trusted_Connection=True;TrustServerCertificate=True;
```

## Тестовый вход

Администратор:

```text
Логин: 94d5ous@gmail.com
Пароль: uzWC67
```

Менеджер:

```text
Логин: ptec8ym@yahoo.com
Пароль: LdNyos
```

Клиент:

```text
Логин: wpmrc3do@tutanota.com
Пароль: RSbvHv
```

## Основные файлы

- `MainWindow.xaml` - главное окно приложения.
- `Windows/ProductEditorWindow.xaml` - окно добавления и редактирования товара.
- `Models/Product.cs` - класс товара.
- `Models/User.cs` - класс пользователя.
- `Services/SqlDataStore.cs` - работа с SQL Server.
- `Services/ProductViewManager.cs` - поиск, фильтрация и сортировка.
- `Services/ProductFactory.cs` - копирование данных товара при редактировании.
- `AppExamDb.sql` - скрипт базы данных для SSMS.

## Клонирование

```bash
git clone https://github.com/FORRENos/AppExam.git
```
