CREATE TABLE [dbo].[ApplicationTypeDocuments] (
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    [DocumentTypeId]    INT NOT NULL,
    [ApplicationTypeId] INT NOT NULL,
    CONSTRAINT [PK_ApplicationTypeDocuments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationTypeDocuments_ApplicationTypes_ApplicationTypeId] FOREIGN KEY ([ApplicationTypeId]) REFERENCES [dbo].[ApplicationTypes] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ApplicationTypeDocuments_ApplicationTypeId]
    ON [dbo].[ApplicationTypeDocuments]([ApplicationTypeId] ASC);

