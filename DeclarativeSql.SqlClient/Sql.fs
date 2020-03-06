namespace Mutex.DeclarativeSql.SqlClient

open System.Data

open Mutex.DeclarativeSql

module Sql =

    let executeReadArray connectionString (command : SelectCommand<'ret>) =
        Command.exec connectionString command.Sql CommandType.Text command.Parameters (Command.readArray command.Value)

    let executeReadFirst connectionString (command : SelectCommand<'ret>) =
        Command.exec connectionString command.Sql CommandType.Text command.Parameters (Command.readFirst command.Value)

    let executeScalar connectionString (command : ScalarCommand<'ret>) =
        Command.exec connectionString command.Sql CommandType.Text command.Parameters (Command.executeScalar command.ScalarValue)

    let execute connectionString (command : Command) =
        Command.exec connectionString command.Sql CommandType.Text command.Parameters Command.executeNonQuery
