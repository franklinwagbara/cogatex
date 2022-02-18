CREATE TABLE [dbo].[Messages] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Subject]       NVARCHAR (MAX) NULL,
    [Content]       NVARCHAR (MAX) NULL,
    [UserId]        NVARCHAR (450) NULL,
    [Date]          DATETIME2 (7)  NOT NULL,
    [IsRead]        BIT            DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [ApplicationId] INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Messages_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_UserId]
    ON [dbo].[Messages]([UserId] ASC);

