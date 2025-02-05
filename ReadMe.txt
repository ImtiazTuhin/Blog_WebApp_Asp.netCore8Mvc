#This prtoject is a Blogging Web App which is developed using Asp.net core 8.0 with MS Sql Server 2022 and Entity Framework

#IDE used VS 2022
#After downloading/Cloning the project Please Configure your appsettings.json file 

1. Install sql server 2022 and ssms (ss management studio). Then craete your username, pwd. Ater that, Create a Database name in your sql server. Now

2. Configure your appsettings.json file using your own db name,pwd. For EX: "DefaultConnection": "Server=localhost; Database=your db name; User Id=sa; Password=your pwd; TrustServerCertificate=True"

3. Install these pacages from the VS 2022 : Go to  Tools->Nuget Package Manager Console->Package Manager Console

 Install: a. microsoft.entityframeworkcore\8.0.10   b. microsoft.entityframeworkcore.sqlserver\8.0.10 c.microsoft.entityframeworkcore.tools\8.0.10

4. You may Run the Project now from VS 2022. 
Thanks. 