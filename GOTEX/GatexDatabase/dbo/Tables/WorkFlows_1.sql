CREATE TABLE [dbo].[WorkFlows] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Action]            NVARCHAR (MAX) NULL,
    [ApplicationTypeId] INT            DEFAULT ((0)) NOT NULL,
    [TargetRole]        NVARCHAR (MAX) NULL,
    [Status]            NVARCHAR (MAX) NULL,
    [TriggeredByRole]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_WorkFlows] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WorkFlows_ApplicationTypes_ApplicationTypeId] FOREIGN KEY ([ApplicationTypeId]) REFERENCES [dbo].[ApplicationTypes] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_WorkFlows_ApplicationTypeId]
    ON [dbo].[WorkFlows]([ApplicationTypeId] ASC);

