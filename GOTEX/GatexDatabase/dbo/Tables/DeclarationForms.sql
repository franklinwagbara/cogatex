CREATE TABLE [dbo].[DeclarationForms] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [QuarterId]         INT            NOT NULL,
    [Year]              INT            NOT NULL,
    [UserId]            NVARCHAR (450) NOT NULL,
    [ExportVolume]      DECIMAL (18)   NOT NULL,
    [CrudeTheft]        INT            NOT NULL,
    [ExportProceedings] INT            NOT NULL,
    [Bribe]             INT            NOT NULL,
    [StaffBribe]        NVARCHAR (MAX) NULL,
    [OutstandingFee]    INT            NOT NULL,
    [Offence]           INT            NOT NULL,
    [Violation]         INT            NOT NULL,
    CONSTRAINT [PK_DeclarationForms] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DeclarationForms_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

