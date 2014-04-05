--
-- Reseed in unit tests acts different depending on whether the database is freshly created.
-- This keeps it consistent.
--

USE [DirectConfig]
GO

INSERT INTO [dbo].[Domains]
           ([DomainName]
           ,[AgentName]
           ,[CreateDate]
           ,[UpdateDate]
           ,[Status])
     VALUES
           ('seedname'
           ,'noagent'
           ,GETDATE()
           ,GETDATE()
           ,0
		   )
GO

