CREATE TABLE [dbo].[PaymentApprovals] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [ApplicationId] INT             NOT NULL,
    [Bank]          NVARCHAR (MAX)  NULL,
    [PaymentId]     NVARCHAR (MAX)  NULL,
    [UserId]        NVARCHAR (450)  NULL,
    [ApprovedBy]    NVARCHAR (MAX)  NULL,
    [PaymentUrl]    NVARCHAR (MAX)  NULL,
    [Amount]        DECIMAL (18, 2) NOT NULL,
    [Date]          DATETIME2 (7)   NOT NULL,
    [Status]        NVARCHAR (MAX)  NULL,
    [Comment]       NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_PaymentApprovals] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PaymentApprovals_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Applications] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PaymentApprovals_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_PaymentApprovals_ApplicationId]
    ON [dbo].[PaymentApprovals]([ApplicationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PaymentApprovals_UserId]
    ON [dbo].[PaymentApprovals]([UserId] ASC);

