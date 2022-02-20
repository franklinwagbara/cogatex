CREATE TABLE [dbo].[ApplicationHistories] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [ApplicationId]      INT            NOT NULL,
    [DateAssigned]       DATETIME2 (7)  NOT NULL,
    [DateTreated]        DATETIME2 (7)  NULL,
    [Comment]            NVARCHAR (MAX) NULL,
    [Status]             NVARCHAR (MAX) NULL,
    [AutoPushed]         BIT            DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [IsAssigned]         BIT            DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [Action]             NVARCHAR (MAX) NULL,
    [CurrentUser]        NVARCHAR (MAX) NULL,
    [CurrentUserRole]    NVARCHAR (MAX) NULL,
    [ProcessingUser]     NVARCHAR (MAX) NULL,
    [ProcessingUserRole] NVARCHAR (MAX) NULL,
    [WorkFlowId]         INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ApplicationHistories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationHistories_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Applications] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ApplicationHistories_ApplicationId]
    ON [dbo].[ApplicationHistories]([ApplicationId] ASC);

