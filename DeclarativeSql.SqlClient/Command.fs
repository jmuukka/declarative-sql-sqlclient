namespace Mutex.DeclarativeSql.SqlClient

open System.Collections.Generic
open System.Data
open System.Data.SqlClient

open Mutex.DeclarativeSql

module internal Command =

    let bindParameters (sqlCommand : SqlCommand)
                       (parameters : Parameter list) =
        let bind (param : Parameter) =
            let value = snd param
            let sqlParam = SqlParameter()
            sqlParam.ParameterName <- fst param
            sqlParam.DbType <- value.DbType
            sqlParam.Value <- value.Value
            sqlCommand.Parameters.Add(sqlParam) |> ignore

        List.iter bind parameters

    let read (reader : IDataReader)
             (getValue : GetValue<'ret>) =
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

    let exec connectionString (commandText : string)
             (commandType : CommandType)
             (parameters : Parameter list)
             (exec : SqlCommand -> 'r) =
        try
            use connection = new SqlConnection(connectionString)
            connection.Open()

            use sqlCommand = new SqlCommand(commandText, connection)
            sqlCommand.CommandType <- commandType

            bindParameters sqlCommand parameters

            exec sqlCommand
            |> Ok
        with
            | ex -> Error ex

    let executeNonQuery (sqlCommand : SqlCommand) =
        sqlCommand.ExecuteNonQuery()

    let executeScalar scalarValue (sqlCommand : SqlCommand) =
        sqlCommand.ExecuteScalar()
        |> scalarValue

    let readFirst getValue
                  (sqlCommand : SqlCommand) =
        use reader = sqlCommand.ExecuteReader(CommandBehavior.SingleRow)
        if reader.Read() then
            let item = read reader getValue
            Some item
        else
            None

    let readArray getValue
                  (sqlCommand : SqlCommand) =
        use reader = sqlCommand.ExecuteReader(CommandBehavior.SingleResult)
        let items = List<'ret>()
        while reader.Read() do
            let item = read reader getValue
            items.Add(item)
        items.ToArray()
