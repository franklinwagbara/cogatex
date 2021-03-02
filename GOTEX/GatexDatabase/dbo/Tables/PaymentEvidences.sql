CREATE TABLE [dbo].[PaymentEvidences] (
    [Id]                  INT             IDENTITY (1, 1) NOT NULL,
    [ReferenceType]       INT             NOT NULL,
    [ReferenceCode]       NVARCHAR (MAX)  NOT NULL,
    [HashCode]            NVARCHAR (MAX)  NULL,
    [ApplicationQuantity] INT             NOT NULL,
    [Amount]              DECIMAL (18, 2) NOT NULL,
    [UsageCount]          INT             NOT NULL,
    [Description]         TEXT            NULL,
    CONSTRAINT [PK_PaymentEvidences] PRIMARY KEY CLUSTERED ([Id] ASC)
);

