namespace Mutex.DeclarativeSql.SqlClient

open Mutex.DeclarativeSql

module StoredProcedure =

    val executeReadArray : ConnectionString -> StoredProcedureSelectCommand<'ret> -> Result<'ret array, exn>
    
    val executeReadFirst : ConnectionString -> StoredProcedureSelectCommand<'ret> -> Result<'ret option, exn>
    
    val executeScalar : ConnectionString -> StoredProcedureScalarCommand<'ret> -> Result<'ret, exn>
    
    val execute : ConnectionString -> StoredProcedureCommand -> Result<RowsAffected, exn>
    