CREATE TABLE [dbo].[Surveys] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [PermitId]       INT            NOT NULL,
    [Effectiveness]  INT            NOT NULL,
    [IssuanceTime]   INT            NOT NULL,
    [PaymentProcess] INT            NOT NULL,
    [DocumentUpload] INT            NOT NULL,
    [KnowledgeGain]  INT            NOT NULL,
    [UserFriendly]   INT            NOT NULL,
    [PermitProcess]  INT            NOT NULL,
    [Comment]        NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Surveys] PRIMARY KEY CLUSTERED ([Id] ASC)
);

