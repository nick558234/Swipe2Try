-- ================================================================
-- Script to Delete All Data and Convert DISHES.DishID to Integer Autoincrement
-- WARNING: This will delete ALL data in the related tables!
-- BACKUP YOUR DATABASE BEFORE RUNNING THIS SCRIPT!
-- ================================================================

PRINT 'Starting script to delete all data and convert DISHES.DishID to INT IDENTITY...';
PRINT '================================================================================';

-- Step 1: Disable foreign key constraints to avoid issues during deletion
PRINT 'Step 1: Disabling foreign key constraints...';
ALTER TABLE DISH_CATEGORIES NOCHECK CONSTRAINT ALL;
ALTER TABLE DISHRESTAURANT NOCHECK CONSTRAINT ALL;
ALTER TABLE LIKES_DISLIKES NOCHECK CONSTRAINT ALL;
ALTER TABLE DISHES NOCHECK CONSTRAINT ALL;
ALTER TABLE USERS NOCHECK CONSTRAINT ALL;
PRINT 'Foreign key constraints disabled.';
PRINT '================================================================================';

-- Step 2: Delete data in correct order (child tables first)
PRINT 'Step 2: Deleting all data from tables...';

PRINT 'Deleting from DISH_CATEGORIES...';
DELETE FROM DISH_CATEGORIES;
PRINT 'DISH_CATEGORIES cleared.';

PRINT 'Deleting from DISHRESTAURANT...';
DELETE FROM DISHRESTAURANT;
PRINT 'DISHRESTAURANT cleared.';

PRINT 'Deleting from LIKES_DISLIKES...';
DELETE FROM LIKES_DISLIKES;
PRINT 'LIKES_DISLIKES cleared.';

PRINT 'Deleting from DISHES...';
DELETE FROM DISHES;
PRINT 'DISHES cleared.';

PRINT 'Deleting from USERS...';
DELETE FROM USERS;
PRINT 'USERS cleared.';

PRINT 'Deleting from ROLES...';
DELETE FROM ROLES;
PRINT 'ROLES cleared.';

PRINT 'Deleting from Restaurants...';
DELETE FROM Restaurants;
PRINT 'Restaurants cleared.';

PRINT 'Deleting from Categories...';
DELETE FROM Categories;
PRINT 'Categories cleared.';

PRINT 'All data deleted successfully.';
PRINT '================================================================================';

-- Step 3: Drop foreign key constraints that reference DISHES.DishID
PRINT 'Step 3: Dropping foreign key constraints that reference DISHES.DishID...';

-- Drop FK constraints
ALTER TABLE DISH_CATEGORIES DROP CONSTRAINT FK__DISH_CATE__DishI__334C8D9D1;
ALTER TABLE DISHRESTAURANT DROP CONSTRAINT FK__DISH_REST__Dish___33C69FB99;
ALTER TABLE LIKES_DISLIKES DROP CONSTRAINT FK__LIKES_DIS__DishI__3398D8EEE;

PRINT 'Foreign key constraints dropped.';
PRINT '================================================================================';

-- Step 4: Modify DISHES table structure
PRINT 'Step 4: Converting DISHES.DishID from nvarchar to INT IDENTITY...';

-- Create new DISHES table with INT IDENTITY DishID
CREATE TABLE DISHES_New (
    DishID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Price DECIMAL(10,2) NULL,
    Category NVARCHAR(100) NULL,
    HealthFactor INT NULL,
    PreparationTime INT NULL,
    Calories INT NULL,
    ImageURL NVARCHAR(500) NULL,
    Ingredients NVARCHAR(MAX) NULL,
    Allergens NVARCHAR(MAX) NULL,
    IsVegan BIT NULL,
    IsVegetarian BIT NULL,
    IsGlutenFree BIT NULL,
    CreatedDate DATETIME2 NULL DEFAULT GETDATE(),
    UserId NVARCHAR(450) NOT NULL -- This will be foreign key to USERS.UserID
);

PRINT 'New DISHES table created with INT IDENTITY DishID.';

-- Drop old DISHES table
DROP TABLE DISHES;
PRINT 'Old DISHES table dropped.';

-- Rename new table
EXEC sp_rename 'DISHES_New', 'DISHES';
PRINT 'New DISHES table renamed to DISHES.';
PRINT '================================================================================';

-- Step 5: Update related tables to use INT for DishID
PRINT 'Step 5: Updating related tables to use INT for DishID...';

-- Update DISH_CATEGORIES
PRINT 'Updating DISH_CATEGORIES.DishID to INT...';
ALTER TABLE DISH_CATEGORIES DROP COLUMN DishID;
ALTER TABLE DISH_CATEGORIES ADD DishID INT NOT NULL;

-- Update DISHRESTAURANT  
PRINT 'Updating DISHRESTAURANT.Dish_ID to INT...';
ALTER TABLE DISHRESTAURANT DROP COLUMN Dish_ID;
ALTER TABLE DISHRESTAURANT ADD Dish_ID INT NOT NULL;

-- Update LIKES_DISLIKES
PRINT 'Updating LIKES_DISLIKES.DishID to INT...';
ALTER TABLE LIKES_DISLIKES DROP COLUMN DishID;
ALTER TABLE LIKES_DISLIKES ADD DishID INT NOT NULL;

PRINT 'Related tables updated to use INT for DishID.';
PRINT '================================================================================';

-- Step 6: Recreate foreign key constraints
PRINT 'Step 6: Recreating foreign key constraints...';

-- Add FK constraint from DISH_CATEGORIES to DISHES
ALTER TABLE DISH_CATEGORIES 
ADD CONSTRAINT FK_DISH_CATEGORIES_DishID 
FOREIGN KEY (DishID) REFERENCES DISHES(DishID);

-- Add FK constraint from DISHRESTAURANT to DISHES
ALTER TABLE DISHRESTAURANT 
ADD CONSTRAINT FK_DISHRESTAURANT_Dish_ID 
FOREIGN KEY (Dish_ID) REFERENCES DISHES(DishID);

-- Add FK constraint from LIKES_DISLIKES to DISHES
ALTER TABLE LIKES_DISLIKES 
ADD CONSTRAINT FK_LIKES_DISLIKES_DishID 
FOREIGN KEY (DishID) REFERENCES DISHES(DishID);

-- Re-enable other foreign key constraints
ALTER TABLE LIKES_DISLIKES CHECK CONSTRAINT ALL;
ALTER TABLE DISHES CHECK CONSTRAINT ALL;
ALTER TABLE USERS CHECK CONSTRAINT ALL;

PRINT 'Foreign key constraints recreated and enabled.';
PRINT '================================================================================';

-- Step 7: Reset identity seeds for all tables
PRINT 'Step 7: Resetting identity seeds...';
DBCC CHECKIDENT('DISHES', RESEED, 0);
DBCC CHECKIDENT('Categories', RESEED, 0);
DBCC CHECKIDENT('Restaurants', RESEED, 0);
PRINT 'Identity seeds reset.';
PRINT '================================================================================';

-- Step 8: Verify the new structure
PRINT 'Step 8: Verifying new table structure...';
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE, 
    COLUMN_DEFAULT,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'DISHES' 
ORDER BY ORDINAL_POSITION;

PRINT '================================================================================';
PRINT 'Script completed successfully!';
PRINT 'DISHES.DishID is now INT IDENTITY(1,1) PRIMARY KEY';
PRINT 'All related tables updated to use INT for DishID references';
PRINT 'All data has been deleted - you can now start fresh with the new structure';
PRINT '================================================================================';
