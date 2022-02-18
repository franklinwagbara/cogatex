CREATE TABLE [dbo].[MailTemplates] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Type]     NVARCHAR (MAX) NULL,
    [Category] NVARCHAR (MAX) NULL,
    [Content]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_MailTemplates] PRIMARY KEY CLUSTERED ([Id] ASC)
);

