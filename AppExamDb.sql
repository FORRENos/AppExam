IF DB_ID(N'AppExamDb') IS NULL
    CREATE DATABASE AppExamDb;
GO

USE AppExamDb;
GO

IF OBJECT_ID(N'OrderItems', N'U') IS NOT NULL DROP TABLE OrderItems;
IF OBJECT_ID(N'Orders', N'U') IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID(N'Products', N'U') IS NOT NULL DROP TABLE Products;
IF OBJECT_ID(N'PickupPoints', N'U') IS NOT NULL DROP TABLE PickupPoints;
IF OBJECT_ID(N'Users', N'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID(N'Roles', N'U') IS NOT NULL DROP TABLE Roles;
IF OBJECT_ID(N'Categories', N'U') IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID(N'Manufacturers', N'U') IS NOT NULL DROP TABLE Manufacturers;
IF OBJECT_ID(N'Suppliers', N'U') IS NOT NULL DROP TABLE Suppliers;
IF OBJECT_ID(N'Units', N'U') IS NOT NULL DROP TABLE Units;
GO

CREATE TABLE Roles
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Users
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleId INT NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Login NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

CREATE TABLE Categories
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL
);

CREATE TABLE Manufacturers
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL
);

CREATE TABLE Suppliers
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL
);

CREATE TABLE Units
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL
);

CREATE TABLE Products
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Article NVARCHAR(50) NOT NULL UNIQUE,
    Name NVARCHAR(200) NOT NULL,
    UnitId INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    SupplierId INT NOT NULL,
    ManufacturerId INT NOT NULL,
    CategoryId INT NOT NULL,
    Discount INT NOT NULL DEFAULT 0,
    QuantityInStock INT NOT NULL DEFAULT 0,
    Description NVARCHAR(MAX) NULL,
    PhotoPath NVARCHAR(260) NULL,
    CONSTRAINT FK_Products_Units FOREIGN KEY (UnitId) REFERENCES Units(Id),
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id),
    CONSTRAINT FK_Products_Manufacturers FOREIGN KEY (ManufacturerId) REFERENCES Manufacturers(Id),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    CONSTRAINT CK_Products_Price CHECK (Price >= 0),
    CONSTRAINT CK_Products_Discount CHECK (Discount >= 0 AND Discount <= 100),
    CONSTRAINT CK_Products_Quantity CHECK (QuantityInStock >= 0)
);

CREATE TABLE PickupPoints
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Address NVARCHAR(300) NOT NULL
);

CREATE TABLE Orders
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderDate DATE NOT NULL,
    DeliveryDate DATE NOT NULL,
    PickupPointId INT NOT NULL,
    UserId INT NULL,
    ReceiveCode INT NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Orders_PickupPoints FOREIGN KEY (PickupPointId) REFERENCES PickupPoints(Id),
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE OrderItems
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id),
    CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0)
);
GO

INSERT INTO Roles (Name)
VALUES (N'Администратор'), (N'Менеджер'), (N'Авторизованный клиент');

INSERT INTO Users (RoleId, FullName, Login, Password)
VALUES
(1, N'Никифорова Анна Семеновна', N'94d5ous@gmail.com', N'uzWC67'),
(1, N'Стелина Евгения Петровна', N'uth4iz@mail.com', N'2L6KZG'),
(1, N'Михайлюк Анна Вячеславовна', N'5d4zbu@tutanota.com', N'rwVDh9'),
(2, N'Ситдикова Елена Анатольевна', N'ptec8ym@yahoo.com', N'LdNyos'),
(2, N'Ворсин Петр Евгеньевич', N'1qz4kw@mail.com', N'gynQMT'),
(2, N'Старикова Елена Павловна', N'4np6se@mail.com', N'AtnDjr'),
(3, N'Никифорова Весения Николаевна', N'yzls62@outlook.com', N'JIFRCZ'),
(3, N'Сазонов Руслан Германович', N'1diph5e@tutanota.com', N'8ntwUp'),
(3, N'Одинцов Серафим Артёмович', N'tjde7c@yahoo.com', N'YOyhfR'),
(3, N'Степанов Михаил Артёмович', N'wpmrc3do@tutanota.com', N'RSbvHv');

INSERT INTO Units (Name)
VALUES (N'шт.');

INSERT INTO Categories (Name)
VALUES (N'Художественная литература'), (N'Учебник для вузов');

INSERT INTO Manufacturers (Name)
VALUES (N'Яуза'), (N'Т8 Издательские технологии');

INSERT INTO Suppliers (Name)
VALUES
(N'Виктор Астафьев'),
(N'Гилберт Кит Честертон'),
(N'Кирилл Каланадзе'),
(N'Людмила Улицкая'),
(N'Аркадий Гайдар');

DECLARE @UnitId INT = (SELECT Id FROM Units WHERE Name = N'шт.');
DECLARE @CategoryFictionId INT = (SELECT Id FROM Categories WHERE Name = N'Художественная литература');
DECLARE @YauzaId INT = (SELECT Id FROM Manufacturers WHERE Name = N'Яуза');
DECLARE @T8Id INT = (SELECT Id FROM Manufacturers WHERE Name = N'Т8 Издательские технологии');

INSERT INTO Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, Discount, QuantityInStock, Description, PhotoPath)
VALUES
(N'A112T4', N'Прокляты и убиты', @UnitId, 585, (SELECT Id FROM Suppliers WHERE Name = N'Виктор Астафьев'), @YauzaId, @CategoryFictionId, 25, 6, N'Роман-эпопея о военной прозе.', N'1.jpg'),
(N'G843H5', N'Тайны и загадки отца Брауна', @UnitId, 193, (SELECT Id FROM Suppliers WHERE Name = N'Гилберт Кит Честертон'), @YauzaId, @CategoryFictionId, 30, 9, N'Классические детективные рассказы.', N'2.jpg'),
(N'D325D4', N'Девайс', @UnitId, 1599, (SELECT Id FROM Suppliers WHERE Name = N'Кирилл Каланадзе'), @T8Id, @CategoryFictionId, 5, 12, N'Современный технологический роман.', N'3.jpg'),
(N'S432T5', N'Необыкновенное чудо. Школьные истории', @UnitId, 549, (SELECT Id FROM Suppliers WHERE Name = N'Людмила Улицкая'), @T8Id, @CategoryFictionId, 15, 15, N'Сборник рассказов.', N'4.jpg'),
(N'F325D4', N'Чук и Гек', @UnitId, 209, (SELECT Id FROM Suppliers WHERE Name = N'Аркадий Гайдар'), @T8Id, @CategoryFictionId, 18, 0, N'Детская повесть.', N'5.jpg');
