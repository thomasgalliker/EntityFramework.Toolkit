create table [dbo].[Departments] (
    [Id] [int] not null identity,
    [Name] [nvarchar](255) not null,
    [LeaderId] [int] null,
    primary key ([Id])
);
create table [dbo].[Employees] (
    [Id] [int] not null identity,
    [LastName] [nvarchar](20) not null,
    [FirstName] [nvarchar](20) not null,
    [Birthdate] [datetime] not null,
    [DepartmentId] [int] null,
    [RowVersion] [varbinary](max) null,
    primary key ([Id])
);
alter table [dbo].[Departments] add constraint [Department_Leader] foreign key ([LeaderId]) references [dbo].[Employees]([Id]);
alter table [dbo].[Employees] add constraint [Employee_Department] foreign key ([DepartmentId]) references [dbo].[Departments]([Id]);
