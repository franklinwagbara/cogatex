CREATE TABLE [dbo].[Configurations] (
    [Id]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]  NVARCHAR (MAX) NULL,
    [Value] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Configurations] PRIMARY KEY CLUSTERED ([Id] ASC)
);

