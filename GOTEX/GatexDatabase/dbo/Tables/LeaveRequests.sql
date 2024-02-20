CREATE TABLE [dbo].[LeaveRequests]
(
	[Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [LeaveId]             INT  NOT NULL,
    [ApprovingStaffId]             NVARCHAR (450)  NOT NULL,
    [DateCreated]             DATETIME2  NOT NULL,
    [DateApproved]             DATETIME2  NULL,
    [IsApproved]             BIT  NOT NULL DEFAULT 0,
    [IsDeleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_LeaveRequests] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LeaveRequests_Leaves_LeaveId] FOREIGN KEY ([LeaveId]) REFERENCES [dbo].[Leaves] ([Id]),
    CONSTRAINT [FK_LeaveRequests_Leaves_ApprovingStaffId] FOREIGN KEY ([ApprovingStaffId]) REFERENCES [dbo].[AspNetUsers] ([Id]),

)
