CREATE TABLE [dbo].[Leaves]
(
	[Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [StaffId]             NVARCHAR (450)  NOT NULL,
    [ActingStaffId]             NVARCHAR (450)  NOT NULL,
    [Start]             DATETIME2  NOT NULL,
    [End]             DATETIME2  NOT NULL,
    [IsApproved]             BIT  NOT NULL DEFAULT 0,
    [IsDeleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Leaves] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Leaves_AspNetUsers_StaffId] FOREIGN KEY ([StaffId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_Leaves_AspNetUsers_ActingStaffId] FOREIGN KEY ([ActingStaffId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
)
