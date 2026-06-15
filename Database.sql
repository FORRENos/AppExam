CREATE DATABASE AppExamDb;
GO

USE AppExamDb;
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

INSERT INTO Roles (Name)
VALUES (N'Администратор'), (N'Менеджер'), (N'Авторизованный клиент');

INSERT INTO Users (RoleId, FullName, Login, Password)
VALUES
(1, N'Никифорова Анна Семеновна', N'94d5ous@gmail.com', N'uzWC67'),
(2, N'Ситдикова Елена Анатольевна', N'ptec8ym@yahoo.com', N'LdNyos'),
(3, N'Степанов Михаил Артёмович', N'wpmrc3do@tutanota.com', N'RSbvHv');
