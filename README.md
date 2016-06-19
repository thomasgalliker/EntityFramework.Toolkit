# EntityFramework.Toolkit
EntityFramework.Toolkit is a library which provides implementations for EntityFramework best practices, patterns, utilities and extensions.
- Patterns such as UnitOfWork, Repository
- Helper classes, e.g. DbContextBase, ContextTestBase
- Generic way to seed data using IDataSeed and DataSeedBase
- DbConnection abstraction IDbConnection to inject ConnectionString into EntityFramework context
- EDMX tools to generate *.edmx file from EntityFramework context (when doing EF code-first)

### Download and Install EntityFramework.Toolkit
This library is available on NuGet: https://www.nuget.org/packages/EntityFramework.Toolkit/
Use the following command to install EntityFramework.Toolkit using NuGet package manager console:

    PM> Install-Package EntityFramework.Toolkit
	
For contract assemblies you may only want to use the abstractions of EntityFramework.Toolkit. For this reason, you can install EntityFramework.Toolkit.Core which comes as a dedicated NuGet package and is absolutely free of any dependencies.

	PM> Install-Package EntityFramework.Toolkit.Core

### API Usage
#### Repository pattern and GenericRepository
As the name implies, ```GenericRepository<T>``` implements the repository pattern for the generic type argument T. The repository pattern is basically a set of CRUD commands (create, read, update, delete) for the given entity type T. Depending on the application that consumes the data layer, CRUD operations are totally sufficient. However, some entity types may require more sophisticated CRUD logic. In this situation, you may want to override the virtual methods of GenericRepository.

#### UnitOfWork pattern
The UnitOfWork (UOW) pattern is by definition a way to commit a set of defined work steps or -if one step cannot be performed for whatever reason- to rollback all steps. The UOW implementation in this library is capable of committing to multiple EntityFramework DbContexts in one go. To do so, it makes use of System.Transaction, which involves MSDTC. The working 

#### Generic Data Seed with IDataSeed
Providing databases with predefined data is an essential feature. IDataSeed is the interface which abstracts the data seed of one particular entity type. Use the abstract base class ```DataSeedBase<T>``` to have the least possible effort to provide a data seed. ```DataSeedBase<T>``` allows you to define an AddOrUpdateExpression which is evaluated in order to check whether a certain entity of type T is already in the database or if it needs to be added. 
mode and configuration of MSDTC is beyond this documentation.
Further reading: http://martinfowler.com/eaaCatalog/unitOfWork.html 

### EntityFramework.Toolkit and IoC
EntityFramework.Toolkit is ready to be used with an IoC framework. You may intend to create a data access module which contains your EF context, the repositories, the entity type configurations, etc. On top of that, you want to promote the CRUD-style repositories to whoever want to consume your data access layer. So, simply create a seperate data access abstraction assembly which contains an interface definition for your repositories. Have a look at the ToolkitSample provided in this project. This sample project adds modularity using the well-known Autofac IoC framework. Have a look at the module configuration ```DataAccessModule``` to get an impression of how to set-up the dependencies.

```
// Register all data seeds:
builder.RegisterType<DepartmentDataSeed>().As<IDataSeed>().SingleInstance();

// Register an IDbConnection and an IDatabaseInitializer which are used to be injected into EmployeeContext
builder.RegisterType<EmployeeContextDbConnection>().As<IDbConnection>().SingleInstance();
builder.RegisterType<EmployeeContextDatabaseInitializer>().As<IDatabaseInitializer<EmployeeContext>>().SingleInstance();

// Finally, register the context all the repositories as InstancePerDependency
builder.RegisterType<EmployeeContext>().As<IEmployeeContext>().InstancePerDependency();
builder.RegisterType<EmployeeRepository>().As<IEmployeeRepository>().InstancePerDependency();
```

Depending on your application, you may need to change the instantiation mode for your EF context from InstancePerDependency to InstancePerRequest. It is recommended to give the EF context (and there for all its descendants, e.g. the repositories and the units of work) a minimal lifetime scope only. You should avoid to have a singleton instance of the context!

### Contribution
If you have any further ideas or specific needs, do not hesitate to submit a [new issue](https://github.com/thomasgalliker/EntityFramework.Toolkit/issues).

### License
This project is Copyright &copy; 2016 [Thomas Galliker](https://ch.linkedin.com/in/thomasgalliker). Free for non-commercial use. For commercial use please contact the author.
