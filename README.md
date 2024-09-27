# 123vendas-api
API to manipulate sales information.

[1] Create databases

a. In your SQL server instance, create a database called 123vendas (feel free to choose some other name)
b. (Optional) Again in your SQL server instance, create a database called 123vendas_log. This db is going to be use by Serilog. You have the option to use the same application database (123vendas) for register application logs, in case you need it.

[2] Run the EF core migrations

a. Go to project 123Vendas.Vendas.DB.
b. In the appsettings.json file, find the "ConnectionStrings" section. 
c. Put the connection string to the database you've created for application.
d. Go to the terminal.
e. In terminal, navigate to 123Vendas.Vendas.DB folder.
f. Run the command "dotnet ef database update"

[3] Set up your connection string

a. Go to project 123Vendas.Vendas.API.
b. Go to appsettings.json file.
c. In the appsettings.json file, find the "ConnectionStrings" section. 
d. Put the connection string to the database you've created for application.
e. Find the "Serilog" section.
f. Into "Serilog" section, localize "WriteTo" section, and then a configuration with name "MSSqlServer".
g. Into "MSSqlServer" section, find "connectionString" key.
h. Put yor serilog database connection string in there.
