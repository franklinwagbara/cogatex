CREATE TABLE [dbo].[RemitaPayments] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [Type]                NVARCHAR (50)  NOT NULL,
    [TransactionDate]     NVARCHAR (50)  NOT NULL,
    [ReferenceNumber]     NVARCHAR (50)  NULL,
    [OnlineReference]     NVARCHAR (50)  NULL,
    [PaymentReference]    NVARCHAR (100) NULL,
    [ApprovedAmount]      NVARCHAR (10)  NULL,
    [ResponseDescription] NVARCHAR (100) NULL,
    [ResponseCode]        NVARCHAR (50)  NULL,
    [TransactionAmount]   NVARCHAR (10)  NULL,
    [TransactionCurrency] NVARCHAR (5)   NULL,
    [CustomerName]        NVARCHAR (200) NULL,
    [UserId]              NVARCHAR (450) NULL,
    [OrderId]             NVARCHAR (20)  NOT NULL,
    [PaymentLogId]        NVARCHAR (50)  NULL,
    [QueryDate]           DATETIME2 (7)  NOT NULL,
    [WebpayReference]     NVARCHAR (MAX) NULL,
    [RRR]                 NVARCHAR (MAX) NULL,
    [PaymentSource]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_RemitaPayments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RemitaPayments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_RemitaPayments_UserId]
    ON [dbo].[RemitaPayments]([UserId] ASC);

