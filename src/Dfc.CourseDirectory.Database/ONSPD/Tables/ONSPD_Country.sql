﻿CREATE TABLE [ONSPD].[ONSPD_Country] (
    [CTRY12CD]  NVARCHAR (50) NOT NULL,
    [CTRY12CDO] INT           NOT NULL,
    [CTRY12NM]  NVARCHAR (50) NOT NULL,
    [CTRY12NMW] NVARCHAR (50) NULL,
    CONSTRAINT [PK_ONSPD_Country] PRIMARY KEY CLUSTERED ([CTRY12CD] ASC)
);

