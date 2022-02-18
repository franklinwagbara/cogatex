CREATE TABLE [dbo].[ApplicationTypes] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (MAX)  NULL,
    [ShortName]     NVARCHAR (MAX)  NULL,
    [Amount]        DECIMAL (18, 2) NOT NULL,
    [ServiceCharge] DECIMAL (18, 2) NOT NULL,
    [ProcessingFee] DECIMAL (18, 2) NOT NULL,
    [IssueType]     NVARCHAR (MAX)  NULL,
    [FullName]      NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_ApplicationTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

