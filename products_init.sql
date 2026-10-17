-- Используем базу данных
USE Products;

-- Создание таблицы Product
CREATE TABLE IF NOT EXISTS Products (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Price DECIMAL(18,2) NOT NULL
);

-- Начальные данные
INSERT INTO Products (Name, Price) VALUES
('Apple', 1.20),
('Banana', 0.80),
('Orange', 1.50),
('Milk', 2.30),
('Bread', 1.10);
