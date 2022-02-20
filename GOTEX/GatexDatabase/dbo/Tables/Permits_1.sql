CREATE TABLE [dbo].[Permits] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [ApplicationId] INT            NOT NULL,
    [DateIssued]    DATETIME2 (7)  NOT NULL,
    [ExpiryDate]    DATETIME2 (7)  NOT NULL,
    [ElpsId]        INT            DEFAULT ((0)) NOT NULL,
    [IsRenewed]     NVARCHAR (MAX) NULL,
    [LicenseName]   NVARCHAR (MAX) NULL,
    [PermitNumber]  NVARCHAR (MAX) NULL,
    [Printed]       BIT            DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [UserId]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Permits] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Permits_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Applications] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Permits_ApplicationId]
    ON [dbo].[Permits]([ApplicationId] ASC);

