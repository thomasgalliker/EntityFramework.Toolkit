create table [dbo].[ApplicationSettings] (
    [Id] [int] not null,
    [Path] [nvarchar](255) null,
    primary key ([Id])
);
create table [dbo].[Countries] (
    [Id] [nvarchar](3) not null,
    [Name] [nvarchar](255) not null,
    primary key ([Id])
);
create table [dbo].[Departments] (
    [Id] [int] not null identity,
    [Name] [nvarchar](255) not null,
    [LeaderId] [int] null,
    primary key ([Id])
);
create table [dbo].[Employees] (
    [Id] [int] not null identity,
    [LastName] [nvarchar](255) not null,
    [FirstName] [nvarchar](255) not null,
    [Birthdate] [datetime] not null,
    [DepartmentId] [int] null,
    [CountryId] [nvarchar](3) null,
    [RowVersion] [rowversion] not null,
    primary key ([Id])
);
alter table [dbo].[Departments] add constraint [Department_Leader] foreign key ([LeaderId]) references [dbo].[Employees]([Id]);
alter table [dbo].[Employees] add constraint [Employee_Country] foreign key ([CountryId]) references [dbo].[Countries]([Id]);
alter table [dbo].[Employees] add constraint [Employee_Department] foreign key ([DepartmentId]) references [dbo].[Departments]([Id]);
