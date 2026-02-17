CREATE DATABASE crm_system;

USE crm_system;

CREATE TABLE Sales_Rep (
    RepId INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE,
    Role VARCHAR(50)
);

CREATE TABLE Lead_Table (
    LeadId INT PRIMARY KEY IDENTITY(1,1),
    FirstName VARCHAR(100),
    LastName VARCHAR(100),
    Email VARCHAR(100),
    Phone VARCHAR(20),
    LeadSource VARCHAR(50),
    LeadStatus VARCHAR(50),
    RepId INT NULL,
    CreatedDate DATE,

    CONSTRAINT FK_Lead_SalesRep
    FOREIGN KEY (RepId)
    REFERENCES Sales_Rep(RepId)
    ON DELETE SET NULL
);





CREATE TABLE Lead_Interaction (
    InteractionId INT PRIMARY KEY IDENTITY(1,1),
    LeadId INT,
    InteractionDate DATE,
    InteractionType VARCHAR(50),
    Notes VARCHAR(MAX),

    CONSTRAINT FK_Interaction_Lead
    FOREIGN KEY (LeadId)
    REFERENCES Lead_Table(LeadId)
    ON DELETE CASCADE
);

CREATE TABLE Lead_Status_History (
    HistoryId INT PRIMARY KEY IDENTITY(1,1),
    LeadId INT,
    OldStatus VARCHAR(50),
    NewStatus VARCHAR(50),
    ChangedBy VARCHAR(100),
    ChangeDate DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_StatusHistory_Lead
    FOREIGN KEY (LeadId)
    REFERENCES Lead_Table(LeadId)
    ON DELETE CASCADE
);

CREATE TABLE Quotation (
    QuoteId INT PRIMARY KEY IDENTITY(1,1),
    QuoteNumber VARCHAR(50) UNIQUE,
    LeadId INT NULL,
    CustomerId INT NULL,
    ValidFrom DATE,
    ValidTo DATE,
    TotalAmount DECIMAL(12,2),
    QuoteStatus VARCHAR(50),

    CONSTRAINT FK_Quotation_Lead
    FOREIGN KEY (LeadId)
    REFERENCES Lead_Table(LeadId)
    ON DELETE SET NULL
);



CREATE TABLE QuotationLineItem (
    LineItemId INT PRIMARY KEY IDENTITY(1,1),
    QuoteId INT,
    ProductId INT,
    Quantity INT,
    UnitPrice DECIMAL(10,2),

    LineTotal AS (Quantity * UnitPrice) PERSISTED,

    CONSTRAINT FK_LineItem_Quotation
    FOREIGN KEY (QuoteId)
    REFERENCES Quotation(QuoteId)
    ON DELETE CASCADE
);



INSERT INTO Sales_Rep (Name, Email, Role)
VALUES
('Alice Smith','alice@example.com','Sales'),
('Bob Jones','bob@example.com','Sales');



INSERT INTO Lead_Table
(FirstName, LastName, Email, Phone, LeadSource, LeadStatus, RepId, CreatedDate)
VALUES
('Charlie','Brown','charlie@example.com','555-0101','Web','New',1,'2026-02-12'),
('Dana','White','dana@example.com','555-0202','Referral','Qualified',2,'2026-02-10');



INSERT INTO Lead_Interaction
(LeadId, InteractionDate, InteractionType, Notes)
VALUES
(1,'2026-02-13','Email','Sent pricing info'),
(1,'2026-02-14','Call','Follow-up call completed'),
(2,'2026-02-15','Meeting','Initial meeting scheduled');



INSERT INTO Quotation
(QuoteNumber, LeadId, ValidFrom, ValidTo, TotalAmount, QuoteStatus)
VALUES
('Q-1001',1,'2026-02-15','2026-03-15',1500.00,'Pending');



INSERT INTO QuotationLineItem
(QuoteId, ProductId, Quantity, UnitPrice)
VALUES
(1,101,2,500.00),
(1,102,1,500.00);


SELECT
    L.LeadId,
    L.FirstName,
    L.LastName,
    S.Name AS SalesRep,
    Q.QuoteNumber,
    Q.TotalAmount
FROM Lead_Table L
LEFT JOIN Sales_Rep S
    ON L.RepId = S.RepId
LEFT JOIN Quotation Q
    ON L.LeadId = Q.LeadId;






