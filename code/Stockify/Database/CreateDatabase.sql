-- CREATE DATABASE

CREATE DATABASE IF NOT EXISTS StockifyDb;
USE StockifyDb;

-- CREATE TABLES


-- Users Table
CREATE TABLE IF NOT EXISTS Utilisateur (
    Utilisateur_Id INT PRIMARY KEY AUTO_INCREMENT,
    Nom CHAR(50) NOT NULL,
    Prenom CHAR(50) NOT NULL,
    Email char(100) NOT NULL UNIQUE,
    Mot_De_Passe char(255) NOT NULL,
    Solde DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    Solde_Devise char(5) NOT NULL DEFAULT 'CAD',
    
);

-- Stocks/Actions Table
CREATE TABLE IF NOT EXISTS Actions (
    Action_Id INT PRIMARY KEY AUTO_INCREMENT,
    Nom_Entreprise char(50) UNIQUE NOT NULL,
    Valeur DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    
-- Portfolio/Possession Table
CREATE TABLE IF NOT EXISTS Possession (
    Possession_Id INT PRIMARY KEY AUTO_INCREMENT,
    Utilisateur_Id INT,
    Action_Id INT,
    Quantite Int NOT NULL,
    FOREIGN KEY (Utilisateur_Id) REFERENCES Utilisateur(Utilisateur_Id)
    FOREIGN KEY (Action_Id) REFERENCES Actions(Action_Id)

    
);

-- Transactions Table
CREATE TABLE IF NOT EXISTS Transactions (
    Transaction_Id INT PRIMARY KEY AUTO_INCREMENT,
    Utilisateur_Id INT NOT NULL,
    Action_Id INT NOT NULL,
    Transaction_Type CHAR(5) NOT NULL,
    Valeur DECIMAL(10, 2) NOT NULL,
    Quantite INT NOT NULL,
    Transaction_Date DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (Utilisateur_Id) REFERENCES Utilisateur(Id),
    FOREIGN KEY (StockId) REFERENCES Stocks(Id) ON DELETE CASCADE,
    INDEX idx_user (UserId),
    INDEX idx_stock (StockId),
    INDEX idx_date (TransactionDate)
);

-- Administrateur Table
CREATE TABLE IF NOT EXISTS Administrateur (
    Admin_Id INT PRIMARY KEY AUTO_INCREMENT,
    Nom CHAR(50) NOT NULL,
    Prenom CHAR(50),
    Email char(100) NOT NULL UNIQUE,
    Mot_De_Passe char(50) NOT NULL
);
