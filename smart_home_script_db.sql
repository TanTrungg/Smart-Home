USE master
GO

DROP DATABASE IF EXISTS SMART_HOME_DB
GO

CREATE DATABASE SMART_HOME_DB
GO

USE SMART_HOME_DB
GO

--Table Role
DROP TABLE IF EXISTS [Role];
GO
CREATE TABLE [Role](
	Id uniqueidentifier primary key NOT NULL,
	RoleName nvarchar(50) NOT NULL
);
GO

INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (N'2cb0985f-3d9e-4d3b-8318-2a67212e782d', N'Owner')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (N'fd396f04-d521-4300-9d16-66efe124f8f6', N'Staff')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (N'8cdbbcd6-b6be-430e-ac02-ae3978595ee3', N'Customer')
INSERT [dbo].[Role] ([Id], [RoleName]) VALUES (N'12555e1b-14b2-46c9-b49b-cf1835a17204', N'Teller')
GO

--Table account
DROP TABLE IF EXISTS Account;
GO
CREATE TABLE Account(
	Id uniqueidentifier primary key NOT NULL,
	RoleId uniqueidentifier foreign key references [Role](Id) NOT NULL,
	PhoneNumber varchar(30) unique NOT NULL,
	PasswordHash varchar(255) NOT NULL,
	[Status] nvarchar(100) NOT NULL,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table device token
DROP TABLE IF EXISTS DeviceToken;
GO
CREATE TABLE DeviceToken(
	Id uniqueidentifier primary key NOT NULL,
	AccountId uniqueidentifier foreign key references Account(Id) NOT NULL,
	Token varchar(max),
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table owner
DROP TABLE IF EXISTS [OwnerAccount];
GO
CREATE TABLE [OwnerAccount](
	AccountId uniqueidentifier foreign key references Account(Id) NOT NULL,
	FullName nvarchar(255) NOT NULL,
	Email varchar(100),
	Avatar varchar(max),
	primary key(AccountId)
);
GO

--Table staff lead
DROP TABLE IF EXISTS [StaffAccount];
GO
CREATE TABLE [StaffAccount](
	AccountId uniqueidentifier foreign key references Account(Id) NOT NULL,
	StaffLeadId uniqueidentifier foreign key references [StaffAccount](AccountId),
	FullName nvarchar(255) NOT NULL,
	Email varchar(100),
	IsLead bit DEFAULT 0 NOT NULL,
	Avatar varchar(max),
	primary key(AccountId)
);
GO

--Table teller
DROP TABLE IF EXISTS TellerAccount;
GO
CREATE TABLE TellerAccount(
	AccountId uniqueidentifier foreign key references Account(Id) NOT NULL,
	FullName nvarchar(255) NOT NULL,
	Email varchar(100),
	Avatar varchar(max),
	primary key(AccountId)
);
GO

--Table customer
DROP TABLE IF EXISTS CustomerAccount;
GO
CREATE TABLE CustomerAccount(
	AccountId uniqueidentifier foreign key references Account(Id) NOT NULL,
	FullName nvarchar(255) NOT NULL,
	Email varchar(100),
	Avatar varchar(max),
	[Address] nvarchar(255) NOT NULL,
	Otp varchar(100),
	primary key(AccountId)
);
GO

--Table notification
DROP TABLE IF EXISTS [Notification];
GO
CREATE TABLE [Notification](
	Id uniqueidentifier primary key NOT NULL,
	AccountId uniqueidentifier foreign key references Account(Id) NOT NULL,
	Title nvarchar(255) NOT NULL,
	Body nvarchar(max) NOT NULL,
	Type nvarchar(255),
	Link nvarchar(255),
	IsRead bit DEFAULT 0 NOT NULL,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table manufacturer
DROP TABLE IF EXISTS Manufacturer;
GO
CREATE TABLE Manufacturer(
	Id uniqueidentifier primary key NOT NULL,
	[Name] nvarchar(255) NOT NULL,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table Promotion
DROP TABLE IF EXISTS Promotion;
GO
CREATE TABLE Promotion(
	Id uniqueidentifier primary key NOT NULL,
	[Name] nvarchar(255) NOT NULL,
	DiscountAmount int,
	StartDate datetime NOT NULL,
	EndDate datetime NOT NULL,
	[Description] nvarchar(max),
	[Status] nvarchar(100) NOT NULL,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

CREATE TRIGGER trg_Promotion_StatusExpired
ON Promotion
AFTER UPDATE
AS
BEGIN
    IF UPDATE(Status)
    BEGIN
        UPDATE DevicePackage
        SET PromotionId = NULL
        FROM DevicePackage AS dp
        JOIN Promotion AS p ON dp.PromotionId = p.Id
        JOIN inserted AS i ON p.Id = i.Id
        WHERE i.Status = 'Expired';
    END
END;

--Table SmartDevice
DROP TABLE IF EXISTS SmartDevice;
GO
CREATE TABLE SmartDevice(
	Id uniqueidentifier primary key NOT NULL,
	ManufacturerId uniqueidentifier foreign key references Manufacturer(Id) NOT NULL,
	[Name] nvarchar(255) NOT NULL,
	[Description] nvarchar(max) NOT NULL,
	Price int NOT NULL,
	InstallationPrice int NOT NULL,
	DeviceType nvarchar(255),
	[Status] nvarchar(100) NOT NULL,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table DevicePackage
DROP TABLE IF EXISTS DevicePackage;
GO
CREATE TABLE DevicePackage(
	Id uniqueidentifier primary key NOT NULL,
	ManufacturerId uniqueidentifier foreign key references Manufacturer(Id) NOT NULL,
	PromotionId uniqueidentifier foreign key references Promotion(Id),
	[Name] nvarchar(255) NOT NULL,
	WarrantyDuration int,
	[Description] nvarchar(max) NOT NULL,
	Price int NOT NULL,
	CompletionTime int NOT NULL,
	[Status] nvarchar(100) NOT NULL,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table SmartDevicePackage
DROP TABLE IF EXISTS SmartDevicePackage;
GO
CREATE TABLE SmartDevicePackage(
	SmartDeviceId uniqueidentifier foreign key references SmartDevice(Id) NOT NULL,
	DevicePackageId uniqueidentifier foreign key references DevicePackage(Id) NOT NULL,
	SmartDeviceQuantity int NOT NULL,
	primary key(SmartDeviceId, DevicePackageId)
);
GO

--Table FeedbackDevicePackage
DROP TABLE IF EXISTS FeedbackDevicePackage;
GO
CREATE TABLE FeedbackDevicePackage(
	Id uniqueidentifier primary key NOT NULL,
	CustomerId uniqueidentifier foreign key references CustomerAccount(AccountId) NOT NULL,
	DevicePackageId uniqueidentifier foreign key references DevicePackage(Id) NOT NULL,
	Rating int NOT NULL,
	Content nvarchar(max),
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table Image
DROP TABLE IF EXISTS [Image];
GO
CREATE TABLE [Image](
	Id uniqueidentifier primary key NOT NULL,
	DevicePackageId uniqueidentifier foreign key references DevicePackage(Id),
	SmartDeviceId uniqueidentifier foreign key references SmartDevice(Id),
	[Url] varchar(max) NOT NULL,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table SurveyRequest
DROP TABLE IF EXISTS SurveyRequest;
GO
CREATE TABLE SurveyRequest(
	Id uniqueidentifier primary key NOT NULL,
	CustomerId uniqueidentifier foreign key references CustomerAccount(AccountId) NOT NULL,
	StaffId uniqueidentifier foreign key references StaffAccount(AccountId),
	SurveyDate datetime NOT NULL,
	[Description] nvarchar(max) NOT NULL,
	[Status] nvarchar(100) NOT NULL,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table Survey
DROP TABLE IF EXISTS Survey;
GO
CREATE TABLE Survey(
	Id uniqueidentifier primary key NOT NULL,
	SurveyRequestId uniqueidentifier unique foreign key references SurveyRequest(Id) NOT NULL,
	RecommendDevicePackageId uniqueidentifier foreign key references DevicePackage(Id),
	RoomArea decimal(18, 2),
	[Description] nvarchar(max) NOT NULL,
	AppointmentDate datetime NOT NULL,
	[Status] nvarchar(100) NOT NULL,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO


--Table Contract
DROP TABLE IF EXISTS [Contract];
GO
CREATE TABLE [Contract](
	Id varchar(255) primary key NOT NULL,
	SurveyId uniqueidentifier unique foreign key references Survey(Id) NOT NULL,
	StaffId uniqueidentifier foreign key references [StaffAccount](AccountId) NOT NULL,
	TellerId uniqueidentifier foreign key references TellerAccount(AccountId) NOT NULL,
	CustomerId uniqueidentifier foreign key references CustomerAccount(AccountId) NOT NULL,
	Title nvarchar(255) NOT NULL,
	[Description] nvarchar(max) NOT NULL,
	StartPlanDate datetime NOT NULL,
	EndPlanDate datetime NOT NULL,
	ActualStartDate datetime,
	ActualEndDate datetime,
	TotalAmount int NOT NULL,
	ImageUrl varchar(max),
	Deposit int NOT NULL,
	[Status] nvarchar(100) NOT NULL,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table ContractModificationRequest
DROP TABLE IF EXISTS ContractModificationRequest;
GO
CREATE TABLE ContractModificationRequest(
    Id uniqueidentifier primary key NOT NULL,
    ContractId varchar(255) foreign key references [Contract](Id) NOT NULL,
    [Type] nvarchar(100) NOT NULL, -- Có thể là "Cancel" hoặc "Modify"
    [Description] nvarchar(max) NOT NULL,
    Status nvarchar(100) NOT NULL, -- Trạng thái của yêu cầu, ví dụ: "Pending", "Approved", "Rejected"
    CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO



--Table DevicePackageUsage
DROP TABLE IF EXISTS DevicePackageUsage;
GO
CREATE TABLE DevicePackageUsage(
	Id uniqueidentifier primary key NOT NULL,
	ContractId varchar(255) foreign key references [Contract](Id) NOT NULL,
	DevicePackageId uniqueidentifier foreign key references DevicePackage(Id) NOT NULL,
	DiscountAmount int,
	Price int NOT NULL,
	WarrantyDuration int,
	StartWarranty datetime,
	EndWarranty datetime,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table ContractDetail
DROP TABLE IF EXISTS ContractDetail;
GO
CREATE TABLE ContractDetail(
	Id uniqueidentifier primary key NOT NULL,
	ContractId varchar(255) foreign key references [Contract](Id) NOT NULL,
	SmartDeviceId uniqueidentifier foreign key references SmartDevice(Id) NOT NULL,
	[Name] nvarchar(255) NOT NULL,
	[Type] varchar(100) NOT NULL, --Purchase/Package
	IsInstallation bit NOT NULL DEFAULT 0,
	InstallationPrice int NOT NULL DEFAULT 0,
	Price int NOT NULL,
	Quantity int NOT NULL,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table Payment
DROP TABLE IF EXISTS Payment;
GO
CREATE TABLE Payment(
	Id varchar(255) primary key NOT NULL,
	ContractId varchar(255) foreign key references [Contract](Id) NOT NULL,
	[Name] nvarchar(255) NOT NULL,
	PaymentMethod nvarchar(100) NOT NULL,
	Amount int NOT NULL,
	[Status] nvarchar(100) NOT NULL,
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

--Table Acceptance
DROP TABLE IF EXISTS Acceptance;
GO
CREATE TABLE Acceptance(
	Id uniqueidentifier primary key NOT NULL,
	ContractId varchar(255) unique foreign key references [Contract](Id) NOT NULL,
	ImageUrl varchar(max),
	CreateAt datetime NOT NULL DEFAULT DATEADD(HOUR, 7, GETUTCDATE())
);
GO

