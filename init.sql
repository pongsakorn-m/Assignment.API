
CREATE TABLE IF NOT EXISTS Transactions (
    Id VARCHAR(50) NOT NULL PRIMARY KEY,
    AccountNumber VARCHAR(30) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    CurrencyCode VARCHAR(3) NOT NULL,
    TransactionDate TIMESTAMP NOT NULL,
    Status VARCHAR(20) NOT NULL
);

