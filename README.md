# Declarative SQL using SqlClient (for SQL Server)

When you have implemented the SQL statements using functions and types in Mutex.DeclarativeSql then you have done the important part of writing correct SQL.

You need this package when you want to execute the commands in SQL Server or in Azure.

## Example

<pre>

module Sql =
   
    let connectionString =
        "server=YOUR_SERVER; database=YOUR_DATABASE; Integrated Security=SSPI"
   
    // Let's create functions by partially apply the connection string.
    let executeReadArray command =
        Sql.executeReadArray connectionString command

    let executeReadFirst command =
        Sql.executeReadFirst connectionString command

    let executeScalar command =
        Sql.executeScalar connectionString command

    let execute command =
        Sql.execute connectionString command


module Example =

    let someWorkflow print context =
        result {
            let! customers = Sql.executeReadArray (Customer.getAll context)
            Array.iter print customers
            return customers
		}

</pre>

------

Copyright (c) 2020 Jarmo Muukka, Mutex Oy