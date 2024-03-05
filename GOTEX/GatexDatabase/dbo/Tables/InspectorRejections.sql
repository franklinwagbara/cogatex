CREATE TABLE [dbo].[InspectorRejections]
(
	[Id]            INT            IDENTITY (1, 1) NOT NULL,
    [InspectorId]       NVARCHAR (450) NOT NULL,
    [AppId] INT            DEFAULT ((0)) NOT NULL,
    [TargetUserId]       NVARCHAR (450) NOT NULL,
    [CreatedAt]          DATETIME2 (7)  NOT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_InspectorRejections] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_InspectorRejections_AspNetUsers_InspectorId] FOREIGN KEY ([InspectorId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_InspectorRejections_Applications_AppId] FOREIGN KEY ([AppId]) REFERENCES [dbo].[Applications] ([Id]),
    CONSTRAINT [FK_InspectorRejections_AspNetUsers_TargetUserId] FOREIGN KEY ([TargetUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
)
