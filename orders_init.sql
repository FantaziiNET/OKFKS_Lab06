-- Используем базу данных
USE Orders;

-- Создание таблицы Product
CREATE TABLE IF NOT EXISTS Orders (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

-- Начальные данные
INSERT INTO Orders (ProductId, Quantity) VALUES
(1, 5),
(2, 10),
(3, 2),
(4, 1);