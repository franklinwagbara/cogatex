CREATE TABLE [dbo].[Terminals] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Terminals] PRIMARY KEY CLUSTERED ([Id] ASC)
);

