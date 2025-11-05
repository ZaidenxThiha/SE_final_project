-- Add StockQuantity column to Products table
-- Run this script to add the StockQuantity field to existing databases

USE AWEfinal;
GO

-- Check if column exists before adding
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'StockQuantity')
BEGIN
    ALTER TABLE [Products]
    ADD [StockQuantity] INT NOT NULL DEFAULT 0;
    
    PRINT 'StockQuantity column added successfully';
END
ELSE
BEGIN
    PRINT 'StockQuantity column already exists';
END
GO

-- Update existing products to have a default stock quantity if they are in stock
UPDATE [Products]
SET [StockQuantity] = CASE 
    WHEN [InStock] = 1 THEN 10 
    ELSE 0 
END
WHERE [StockQuantity] = 0;
GO

PRINT 'Database migration completed successfully';
GO

