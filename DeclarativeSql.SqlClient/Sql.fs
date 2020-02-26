namespace Mutex.DeclarativeSql.SqlClient

open System.Collections.Generic
open System.Data
open System.Data.SqlClient

open Mutex.DeclarativeSql

type Parameter = BindName * Value

module Sql =

    let private bindParameters (sqlCommand : SqlCommand)
                               (parameters : Parameter list) =
        let bind (param : BindName * Value) =
            let value = snd param
            let sqlParam = SqlParameter()
            sqlParam.ParameterName <- fst param
            sqlParam.DbType <- value.DbType
            sqlParam.Value <- value.Value
            sqlCommand.Parameters.Add(sqlParam) |> ignore

        List.iter bind parameters

    let private read (reader : IDataReader) (getValue : GetValue<'ret>) =
        match getValue with
        | ObjArray get ->
            let values = Array.init reader.FieldCount (fun _ -> null)
            reader.GetValues(values) |> ignore
            get values
        | Indexed get ->
            let valueByIndex index =
                reader.GetValue(index)
            get valueByIndex
        | Named get ->
            let valueByName name =
                let index = reader.GetOrdinal(name)
                reader.GetValue(index)
            get valueByName

    let private exec connectionString (sql : string)
                     (parameters : Parameter list)
                     (exec : SqlCommand -> 'r) =
        try
            use connection = new SqlConnection(connectionString)
            connection.Open()

            use sqlCommand = new SqlCommand(sql, connection)

            bindParameters sqlCommand parameters

            exec sqlCommand
            |> Ok
        with
            | ex -> Error ex

    let executeReadArray connectionString (command : SelectCommand<'ret>) =
        let readArray (sqlCommand : SqlCommand) =
            use reader = sqlCommand.ExecuteReader(CommandBehavior.SingleResult)
            let items = List<'ret>()
            while reader.Read() do
                let item = read reader command.Value
                items.Add(item)
            items.ToArray()

        exec connectionString command.Sql command.Parameters readArray

    let executeReadFirst connectionString (command : SelectCommand<'ret>) =
        let readFirst (sqlCommand : SqlCommand) =
            use reader = sqlCommand.ExecuteReader(CommandBehavior.SingleRow)
            if reader.Read() then
                let item = read reader command.Value
                Some item
            else
                None

        exec connectionString command.Sql command.Parameters readFirst

    let executeScalar connectionString (command : ScalarCommand<'ret>) =
        let executeScalar (sqlCommand : SqlCommand) =
            sqlCommand.ExecuteScalar()
            |> command.ScalarValue

        exec connectionString command.Sql command.Parameters executeScalar

    let execute connectionString (command : Command) =
        let executeNonQuery (sqlCommand : SqlCommand) =
            sqlCommand.ExecuteNonQuery()

        exec connectionString command.Sql command.Parameters executeNonQuery
