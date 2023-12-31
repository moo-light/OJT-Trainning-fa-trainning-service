# FA Training Management System
## Prepair to Run
1. Modify the ```DatabaseConnection``` in ```appsettings.Development.json```

2. Apply the latest migrations by running this command(run command from apis folder)
```
dotnet ef database update -s WebAPI -p Infrastructures
```

## EF migration
0. Install global tool to make migration(do only 1 time & your machine is good to go for the next)
```
dotnet tool install --global dotnet-ef
```
1. Create migrations & the dbcontext snapshot will rendered.
Open CLI at apis folder & run command
-s is startup project(create dbcontext instance at design time)
-p is migrations assembly project
```
dotnet ef migrations add MigrationName -s WebAPI -p Infrastructures
```

2. Apply the change
```
dotnet ef database update -s WebAPI -p Infrastructures
```

## Running unit test & test coverage
0. Install the tool that will generate the report after run test collect data(install only one time)
open terminal(CLI) & run command below
```
dotnet tool install -g dotnet-reportgenerator-globaltool
```

1. In the root folder(where file .sln) run the test & collect the result
```
dotnet test --collect:"XPlat Code Coverage"
```
2. To make it more easy to human readable run this command

```
reportgenerator -reports:**/**/coverage.cobertura.xml -targetdir:"Tests\CoverageReport" -reporttypes:"Html;JsonSummary;TextSummary" -classfilters:-AutoGeneratedProgram
```
3. The file report will be generated in Tests/CoverageReport open the index.html to view result
4. Docs if you have any problem
https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=windows

## Postman collection & env in the root project
1. Import the postman collection
2. Import postman enviroment
3. Start test
https://learning.postman.com/docs/writing-scripts/script-references/test-examples/#testing-status-codes

## Azure Server   
Link: https://deploy-fa-training.azurewebsites.net/swagger/index.html

&copy; 2023 FA Internship - Group456

