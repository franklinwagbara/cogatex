CREATE TABLE [dbo].[Facilities] (
    [Id]         INT IDENTITY (1, 1) NOT NULL,
    [CompanyId]  INT NOT NULL,
    [TerminalId] INT NOT NULL,
    [ProductId]  INT NOT NULL,
    [ElpsId]     INT NOT NULL,
    CONSTRAINT [PK_Facilities] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Facilities_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([Id]),
    CONSTRAINT [FK_Facilities_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]),
    CONSTRAINT [FK_Facilities_Terminals] FOREIGN KEY ([TerminalId]) REFERENCES [dbo].[Terminals] ([Id])
);

