use [InnerPortalDbDeploy];
declare @Message nvarchar(max);
declare @DomainId int = 0;
declare @ArchiveRootId int = 0;
begin try
	begin transaction T;
		select @ArchiveRootId = [Id] from [dbo].[MainMenuItem] where [Name] = N'Архив' and [Url] is null;
		delete from [dbo].[MainMenuItem] where [ParentId] = @ArchiveRootId;
		delete from [dbo].[MainMenuItem] where [Id] = @ArchiveRootId;
		
		select @DomainId = [Id] from [dbo].[Domain] where [Name] = 'Archive';
		delete from [dbo].[DomainEnvironment] where [DomainId] = @DomainId;
	
		delete from [dbo].[Domain] where [Id] = @DomainId;
		
	commit transaction T;
end try
begin catch
	rollback transaction T;
	select @Message = error_message() 
	raiserror('Error Occured: %s', 20, 101, @Message) with log
end catch 