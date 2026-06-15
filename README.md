# AppExam

WPF-приложение для модулей 2-3 демоэкзамена.

## Подключение к БД

Приложение подключается к SQL Server LocalDB:

```text
Server=(localdb)\AppExamSQL
Database=AppExamDb
```

В SSMS можно подключиться к серверу:

```text
(localdb)\AppExamSQL
```

Скрипт для SSMS:

- `AppExamDb.sql` - создает базу, таблицы и начальные данные.

## Таблицы

- `Roles`
- `Users`
- `Categories`
- `Manufacturers`
- `Suppliers`
- `Units`
- `Products`
- `PickupPoints`
- `Orders`
- `OrderItems`

## Тестовый вход

Администратор:

```text
Логин: 94d5ous@gmail.com
Пароль: uzWC67
```

## Git Bash

```bash
cd "/c/Users/Voron/OneDrive/Документы/экзамен/AppExam"
git add .
git commit -m "Connect WPF app to SQL database"
git push -u origin main
```

Открывать в Visual Studio 2022 нужно файл `AppExam.sln`.
