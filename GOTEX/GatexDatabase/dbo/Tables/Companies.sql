CREATE TABLE [dbo].[Companies] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [ElpsId]            INT            NOT NULL,
    [RegAddressId]      INT            NOT NULL,
    [Name]              NVARCHAR (MAX) NULL,
    [Nationality]       NVARCHAR (MAX) NULL,
    [YearIncorporated]  NVARCHAR (MAX) NULL,
    [RcNumber]          NVARCHAR (MAX) NULL,
    [TinNumber]         NVARCHAR (MAX) NULL,
    [DirectorSignature] NVARCHAR (MAX) NULL,
    [RegisteredAddress] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED ([Id] ASC)
);

