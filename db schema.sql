create schema `edrak.orders`;
use `edrak.orders`;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Customers` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Address` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Customers` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `OrderStatuses` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_OrderStatuses` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Products` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
    `Description` varchar(1024) CHARACTER SET utf8mb4 NOT NULL,
    `Price` decimal(65,30) NOT NULL,
    `StockQuantity` int NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreatedDate` datetime NOT NULL,
    `CreatedBy` varchar(64) CHARACTER SET utf8mb4 NULL,
    `LastModifiedDate` datetime NULL,
    `LastModifiedBy` varchar(64) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Products` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Orders` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `CustomerId` int NOT NULL,
    `datetime` datetime(6) NOT NULL,
    `TotalAmount` decimal(65,30) NOT NULL,
    `StatusId` int NOT NULL,
    `CreatedDate` datetime NOT NULL,
    `CreatedBy` varchar(64) CHARACTER SET utf8mb4 NULL,
    `LastModifiedDate` datetime NULL,
    `LastModifiedBy` varchar(64) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Orders` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Orders_Customers_CustomerId` FOREIGN KEY (`CustomerId`) REFERENCES `Customers` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Orders_OrderStatuses_StatusId` FOREIGN KEY (`StatusId`) REFERENCES `OrderStatuses` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `OrderLineItems` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ProductId` int NOT NULL,
    `ProductMetaData` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Quantity` int NOT NULL,
    `OrderId` int NULL,
    CONSTRAINT `PK_OrderLineItems` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_OrderLineItems_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`),
    CONSTRAINT `FK_OrderLineItems_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_OrderLineItems_OrderId` ON `OrderLineItems` (`OrderId`);

CREATE INDEX `IX_OrderLineItems_ProductId` ON `OrderLineItems` (`ProductId`);

CREATE INDEX `IX_Orders_CustomerId` ON `Orders` (`CustomerId`);

CREATE INDEX `IX_Orders_StatusId` ON `Orders` (`StatusId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230724121221_Init', '7.0.9');

COMMIT;

