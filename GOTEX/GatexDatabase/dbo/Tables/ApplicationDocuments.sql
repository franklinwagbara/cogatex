CREATE TABLE [dbo].[ApplicationDocuments] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [ApplicationId]             INT            NOT NULL,
    [DocumentId]                INT            NOT NULL,
    [ApplicationTypeDocumentId] INT            NOT NULL,
    [UniqueId]                  NVARCHAR (MAX) NULL,
    [IsUploaded]                BIT            NOT NULL,
    [DocTypeId]                 INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ApplicationDocuments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationDocuments_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Applications] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ApplicationDocuments_ApplicationId]
    ON [dbo].[ApplicationDocuments]([ApplicationId] ASC);

