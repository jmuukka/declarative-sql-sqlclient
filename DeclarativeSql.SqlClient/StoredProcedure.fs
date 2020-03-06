namespace Mutex.DeclarativeSql.SqlClient

open System.Data

open Mutex.DeclarativeSql

module StoredProcedure =

    let executeReadArray connectionString (command : StoredProcedureSelectCommand<'ret>) =
        Command.exec connectionString command.StoredProcedure CommandType.StoredProcedure command.Parameters (Command.readArray command.Value)

    let executeReadFirst connectionString (command : StoredProcedureSelectCommand<'ret>) =
        Command.exec connectionString command.StoredProcedure CommandType.StoredProcedure command.Parameters (Command.readFirst command.Value)

    let executeScalar connectionString (command : StoredProcedureScalarCommand<'ret>) =
        Command.exec connectionString command.StoredProcedure CommandType.StoredProcedure command.Parameters (Command.executeScalar command.ScalarValue)

    let execute connectionString (command : StoredProcedureCommand) =
        Command.exec connectionString command.StoredProcedure CommandType.StoredProcedure command.Parameters Command.executeNonQuery
