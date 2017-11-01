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
    [RowVersion] [rowversion] not null,
    primary key ([Id])
);
create table [dbo].[Employee] (
    [Id] [int] not null,
    [DepartmentId] [int] null,
    [EmployementDate] [datetime] null,
    [PropertyA] [nvarchar](max) null,
    [PropertyB] [nvarchar](max) null,
    primary key ([Id])
);
create table [dbo].[EmployeeAudit] (
    [AuditId] [int] not null identity,
    [Id] [int] not null,
    [FirstName] [nvarchar](max) not null,
    [LastName] [nvarchar](max) not null,
    [AuditDate] [datetime] not null,
    [AuditUser] [nvarchar](max) not null,
    [AuditType] [int] not null,
    primary key ([AuditId])
);
create table [dbo].[Person] (
    [Id] [int] not null identity,
    [LastName] [nvarchar](255) not null,
    [FirstName] [nvarchar](255) not null,
    [Birthdate] [datetime] not null,
    [CountryId] [nvarchar](3) null,
    [RowVersion] [rowversion] not null,
    [CreatedDate] [datetime] not null,
    [UpdatedDate] [datetime] null,
    primary key ([Id])
);
create table [dbo].[Room] (
    [Id] [int] not null identity,
    [Level] [int] not null,
    [Sector] [nvarchar](900) null,
    [Description] [nvarchar](255) null,
    [CreatedDate] [datetime] not null,
    primary key ([Id])
);
create table [dbo].[Student] (
    [Id] [int] not null,
    [EnrollmentDate] [datetime] not null,
    primary key ([Id])
);
alter table [dbo].[Departments] add constraint [Department_Leader] foreign key ([LeaderId]) references [dbo].[Person]([Id]);
alter table [dbo].[Employee] add constraint [Employee_Department] foreign key ([DepartmentId]) references [dbo].[Departments]([Id]);
alter table [dbo].[Employee] add constraint [Employee_TypeConstraint_From_Person_To_Employee] foreign key ([Id]) references [dbo].[Person]([Id]);
alter table [dbo].[Person] add constraint [Person_Country] foreign key ([CountryId]) references [dbo].[Countries]([Id]);
alter table [dbo].[Student] add constraint [Student_TypeConstraint_From_Person_To_Student] foreign key ([Id]) references [dbo].[Person]([Id]);
