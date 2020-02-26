namespace Mutex.DeclarativeSql.SqlClient

open Mutex.DeclarativeSql

module Sql =

    val executeReadArray : ConnectionString -> SelectCommand<'ret> -> Result<'ret array, exn>

    val executeReadFirst : ConnectionString -> SelectCommand<'ret> -> Result<'ret option, exn>

    val executeScalar : ConnectionString -> ScalarCommand<'ret> -> Result<'ret, exn>

    val execute : ConnectionString -> Command -> Result<RowsAffected, exn>
