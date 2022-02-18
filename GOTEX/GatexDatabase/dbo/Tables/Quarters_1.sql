CREATE TABLE [dbo].[Quarters] (
    [Id]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]  NVARCHAR (MAX) NULL,
    [Title] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Quarters] PRIMARY KEY CLUSTERED ([Id] ASC)
);

