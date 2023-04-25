use [InnerPortalDbDeploy];
declare @Message nvarchar(max);
declare @ArchiveRootId int = 0;
begin try
	begin transaction T;
		select @ArchiveRootId = [Id] from [dbo].[MainMenuItem] where [Name] = N'Архив' and [Url] is null;
		
		UPDATE [dbo].[MainMenuItem] SET [ControllerRoles] = N'Developer, ArchiveAdmin'
			,[Roles] = N'Developer, ArchiveAdmin'
		WHERE [ParentId] = @ArchiveRootId and [Url] = N'HolderTypes';
		
		UPDATE [dbo].[MainMenuItem] SET [ControllerRoles] = N'Developer, ArchiveAdmin'
			,[Roles] = N'Developer, ArchiveAdmin'
		WHERE [ParentId] = @ArchiveRootId and [Url] = N'Statuses';
		
		UPDATE [dbo].[MainMenuItem] SET [ControllerRoles] = N'Developer, ArchiveAdmin'
			,[Roles] = N'Developer, ArchiveAdmin'
		WHERE [ParentId] = @ArchiveRootId and [Url] = N'Users';
		UPDATE [dbo].[MainMenuItem] SET [ControllerRoles] = N'Developer, ArchiveAdmin, Archive'
			,[Roles] = N'Developer, ArchiveAdmin'
		WHERE [ParentId] = @ArchiveRootId and [Url] = N'Archive';
		
		UPDATE [dbo].[MainMenuItem] SET [ControllerRoles] = N'Developer, ArchiveAdmin, ArchiveDropper'
			,[Roles] = N'Developer, ArchiveAdmin'
		WHERE [ParentId] = @ArchiveRootId and [Url] = N'ResetHolder';
 
	commit transaction T;
end try
begin catch
	rollback transaction T;
	select @Message = error_message() 
	raiserror('Error Occured: %s', 20, 101, @Message) with log
end catch 