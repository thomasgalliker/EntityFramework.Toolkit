# EntityFramework.Toolkit
EntityFramework.Toolkit is a library which provides implementations for best practice patterns, utilities and extensions for EntityFramework 6.

### Download and Install EntityFramework.Toolkit
This library is available on NuGet: https://www.nuget.org/packages/EntityFramework.Toolkit/
Use the following command to install EntityFramework.Toolkit using NuGet package manager console:

    PM> Install-Package EntityFramework.Toolkit

### API Usage
#### Repository pattern and GenericRepository
As the name implies, ```GenericRepository<T>``` implements the repository pattern for the generic type argument T. The repository pattern is basically a set of CRUD commands (create, read, update, delete) for the given entity type T. Depending on the application that consumes the data layer, CRUD operations are totally sufficient. However, some entity types may require more sophisticated CRUD logic. In this situation, you may want to override the virtual methods of GenericRepository.
TODO: Document IoC registration when using Autofac.

#### UnitOfWork pattern
The UnitOfWork (UOW) pattern is by definition a way to commit a set of defined work steps or -if one step cannot be performed for whatever reason- to rollback all steps. The UOW implementation in this library is capable of committing to multiple EntityFramework DbContexts in one go. To do so, it makes use of System.Transaction, which involves MSDTC. The working 

#### Generic Data Seed with IDataSeed
Providing databases with predefined data is an essential feature. IDataSeed is the interface which abstracts the data seed of one particular entity type. Use the abstract base class ```DataSeedBase<T>``` to have the least possible effort to provide a data seed. ```DataSeedBase<T>``` allows you to define an AddOrUpdateExpression which is evaluated in order to check whether a certain entity of type T is already in the database or if it needs to be added. 
mode and configuration of MSDTC is beyond this documentation.
Further reading: http://martinfowler.com/eaaCatalog/unitOfWork.html 

### Contribution
If you have any further ideas or specific needs, do not hesitate to submit a [new issue](https://github.com/thomasgalliker/EntityFramework.Toolkit/issues).

### License
This project is Copyright &copy; 2016 [Thomas Galliker](https://ch.linkedin.com/in/thomasgalliker). Free for non-commercial use. For commercial use please contact the author.
