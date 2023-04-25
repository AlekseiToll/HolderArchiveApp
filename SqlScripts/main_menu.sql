use [InnerPortalDbDeploy];
declare @Message nvarchar(max);
declare @DomainId int = 0;
declare @ModuleRootId int = 0;
declare @ArchiveRootId int = 0;
declare @MaxDomainEnvironmentId int = 0;
declare @MaxMainMenuItemId int = 0;
begin try
	begin transaction T;
		select @DomainId = max(Id) + 1 from [dbo].[Domain];
		insert into [dbo].[Domain](Id, Name) values(@DomainId, 'Archive');
		select @MaxDomainEnvironmentId = max(Id) from [dbo].[DomainEnvironment];
		insert into [dbo].[DomainEnvironment](Id, DomainId, EnvironmentType, HostUrl) values(@MaxDomainEnvironmentId + 1, @DomainId, 3, 'tracking-365.helix.ru');
		insert into [dbo].[DomainEnvironment](Id, DomainId, EnvironmentType, HostUrl) values(@MaxDomainEnvironmentId + 2, @DomainId, 2, 'tracking-365.beta.helix.ru');
		insert into [dbo].[DomainEnvironment](Id, DomainId, EnvironmentType, HostUrl) values(@MaxDomainEnvironmentId + 3, @DomainId, 1, 'tracking-365cc.spb.helix.ru');
		
		select @MaxMainMenuItemId = max(Id) from [dbo].[MainMenuItem];
		select @ModuleRootId = [Id] from [dbo].[MainMenuItem] where [Name] = N'Модули' and [ParentId] is null;
		
		insert [dbo].[MainMenuItem] ([Id], [ParentId], [DomainId], [Name], [Url], [Parameters], [ControllerRoles], [Roles], [HelixRoles], [NeedAuthorization], [IsVisible], [Weight], [HtmlAttributes], [Icon]) values (@MaxMainMenuItemId + 1, @ModuleRootId, @DomainId, N'Архив', null, null, null, null, null, 1, 1, 50, null, N'fa fa-archive');
		select @ArchiveRootId = [Id] from [dbo].[MainMenuItem] where [Name] = N'Архив' and [Url] is null;
		
		insert [dbo].[MainMenuItem] ([Id], [ParentId], [DomainId], [Name], [Url], [Parameters], [ControllerRoles], [Roles], [HelixRoles], [NeedAuthorization], [IsVisible], [Weight], [HtmlAttributes], [Icon]) values (@MaxMainMenuItemId + 2, @ArchiveRootId, @DomainId, N'Штативы', N'HolderTypes', null, N'Developer', N'Developer', null, 1, 1, 20, null, null);
		insert [dbo].[MainMenuItem] ([Id], [ParentId], [DomainId], [Name], [Url], [Parameters], [ControllerRoles], [Roles], [HelixRoles], [NeedAuthorization], [IsVisible], [Weight], [HtmlAttributes], [Icon]) values (@MaxMainMenuItemId + 3, @ArchiveRootId, @DomainId, N'Статусы', N'Statuses', null, N'Developer', N'Developer', null, 1, 1, 17, null, null);
		insert [dbo].[MainMenuItem] ([Id], [ParentId], [DomainId], [Name], [Url], [Parameters], [ControllerRoles], [Roles], [HelixRoles], [NeedAuthorization], [IsVisible], [Weight], [HtmlAttributes], [Icon]) values (@MaxMainMenuItemId + 4, @ArchiveRootId, @DomainId, N'Настройка пользователей', N'Users', null,  N'Developer', N'Developer',null, 1, 1, 14, null, null);
		insert [dbo].[MainMenuItem] ([Id], [ParentId], [DomainId], [Name], [Url], [Parameters], [ControllerRoles], [Roles], [HelixRoles], [NeedAuthorization], [IsVisible], [Weight], [HtmlAttributes], [Icon]) values (@MaxMainMenuItemId + 5, @ArchiveRootId, @DomainId, N'Архивация', N'Archive', null, N'Developer', N'Developer', null, 1, 1, 11, null, null);
		insert [dbo].[MainMenuItem] ([Id], [ParentId], [DomainId], [Name], [Url], [Parameters], [ControllerRoles], [Roles], [HelixRoles], [NeedAuthorization], [IsVisible], [Weight], [HtmlAttributes], [Icon]) values (@MaxMainMenuItemId + 6, @ArchiveRootId, @DomainId, N'Сброс', N'ResetHolder', null,  N'Developer', N'Developer',null, 1, 1, 7, null, null);
	commit transaction T;
end try
begin catch
	rollback transaction T;
	select @Message = error_message() 
	raiserror('Error Occured: %s', 20, 101, @Message) with log
end catch 