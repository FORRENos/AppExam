# AppExam

WPF-приложение для модулей 2-3 демоэкзамена.

## База данных в SSMS

Базу лучше назвать `AppExamDb`.

Таблицы:

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

Если преподаватель просит ровно 9 таблиц, можно не создавать `Roles`, а хранить роль пользователя в поле `Users.RoleName`. Для нормальной связанной БД лучше оставить `Roles`.

SQL-скрипт для SSMS лежит в файле `Database.sql`.

## Git Bash

```bash
git clone https://github.com/FORRENos/AppExam.git
cd AppExam
git add .
git commit -m "Add WPF exam application"
git push origin main
```

Если основная ветка называется `master`, последняя команда:

```bash
git push origin master
```

Открывать в Visual Studio 2022 нужно файл `AppExam.sln`.
