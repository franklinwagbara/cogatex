CREATE TABLE [dbo].[Invoices] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [ApplicationId]        INT            NOT NULL,
    [Amount]               FLOAT (53)     NOT NULL,
    [Status]               NVARCHAR (6)   NOT NULL,
    [PaymentCode]          NVARCHAR (100) NOT NULL,
    [PaymentType]          NVARCHAR (50)  NULL,
    [ReceiptNo]            NVARCHAR (25)  NULL,
    [DateAdded]            DATETIME2 (7)  NOT NULL,
    [DatePaid]             DATETIME2 (7)  NOT NULL,
    [PaymentTransactionId] INT            NOT NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Invoices_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Applications] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Invoices_ApplicationId]
    ON [dbo].[Invoices]([ApplicationId] ASC);

