CREATE TABLE [dbo].[ManualRemitaValues] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [RRR]           NVARCHAR (MAX) NULL,
    [Company]       NVARCHAR (MAX) NULL,
    [Beneficiary]   NVARCHAR (MAX) NULL,
    [FundingBank]   NVARCHAR (MAX) NULL,
    [PaymentSource] NVARCHAR (MAX) NULL,
    [NetAmount]     FLOAT (53)     NOT NULL,
    [Status]        NVARCHAR (MAX) NULL,
    [DateAdded]     DATETIME2 (7)  NOT NULL,
    [AddedBy]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ManualRemitaValues] PRIMARY KEY CLUSTERED ([Id] ASC)
);

