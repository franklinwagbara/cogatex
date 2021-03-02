CREATE TABLE [dbo].[Applications] (
    [Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [UserId]             NVARCHAR (450)  NULL,
    [Year]               INT             NOT NULL,
    [Fee]                DECIMAL (18, 2) NOT NULL,
    [ServiceCharge]      DECIMAL (18, 2) NOT NULL,
    [Status]             NVARCHAR (MAX)  NULL,
    [Reference]          NVARCHAR (MAX)  NULL,
    [Description]        NVARCHAR (MAX)  NULL,
    [GasStream]          NVARCHAR (MAX)  NULL,
    [Quantity]           INT             NOT NULL,
    [ProductAmount]      DECIMAL (18, 2) NOT NULL,
    [IsAssigned]         BIT             NOT NULL,
    [IsProcessed]        BIT             NOT NULL,
    [ProductId]          INT             CONSTRAINT [DF__Applicati__Produ__74AE54BC] DEFAULT ((0)) NOT NULL,
    [QuarterId]          INT             CONSTRAINT [DF__Applicati__Quart__75A278F5] DEFAULT ((0)) NOT NULL,
    [TerminalId]         INT             CONSTRAINT [DF__Applicati__Termi__76969D2E] DEFAULT ((0)) NOT NULL,
    [ApplicationTypeId]  INT             CONSTRAINT [DF__Applicati__Appli__7A672E12] DEFAULT ((0)) NOT NULL,
    [PaymentEvidenceId]  INT             NULL,
    [Date]               DATETIME2 (7)   CONSTRAINT [DF__Applicatio__Date__14270015] DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    [Submitted]          BIT             CONSTRAINT [DF__Applicati__Submi__2CF2ADDF] DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [StageId]            INT             CONSTRAINT [DF__Applicati__Stage__37703C52] DEFAULT ((0)) NOT NULL,
    [LastAssignedUserId] NVARCHAR (MAX)  NULL,
    [CurrentPermit]      NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Applications_ApplicationTypes_ApplicationTypeId] FOREIGN KEY ([ApplicationTypeId]) REFERENCES [dbo].[ApplicationTypes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Applications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_Applications_PaymentEvidence] FOREIGN KEY ([PaymentEvidenceId]) REFERENCES [dbo].[PaymentEvidences] ([Id]),
    CONSTRAINT [FK_Applications_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Applications_Quarters_QuarterId] FOREIGN KEY ([QuarterId]) REFERENCES [dbo].[Quarters] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Applications_Terminals_TerminalId] FOREIGN KEY ([TerminalId]) REFERENCES [dbo].[Terminals] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Applications_UserId]
    ON [dbo].[Applications]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Applications_ProductId]
    ON [dbo].[Applications]([ProductId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Applications_QuarterId]
    ON [dbo].[Applications]([QuarterId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Applications_TerminalId]
    ON [dbo].[Applications]([TerminalId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Applications_ApplicationTypeId]
    ON [dbo].[Applications]([ApplicationTypeId] ASC);

