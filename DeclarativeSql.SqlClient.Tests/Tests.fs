module Tests

open System
open Xunit

open Mutex.DeclarativeSql
open Mutex.DeclarativeSql.Obj
open Mutex.DeclarativeSql.SqlClient

type OrganizationId = OrganizationId of int

type TestType = {
    Id : int
    RequireString20 : string
    NullableString10 : string
}

module Customer =

    let private columns = "Id, RequireString20, NullableString10"

    let private testType (values : obj array) =
        {
            Id = int values.[0]
            RequireString20 = string values.[1]
            NullableString10 = string values.[2]
        }

    let selectGreaterThanId id =
        {
            Sql = "select " + columns + " from Test where Id > @Id"
            Parameters = [
                "Id", Value.ofInt32 id
            ]
            Value = ObjArray testType
        }

module Sql =
   
    let connectionString =
        "server=(local); database=Mutex.Data; Integrated Security=SSPI"
   
    let executeReadArray command =
        Sql.executeReadArray connectionString command

[<Fact>]
let ``executeReadArray should return rows (assume that we have data in the database)`` () =
    let result = 
        Customer.selectGreaterThanId 0
        |> Sql.executeReadArray

    match result with
    | Ok rows ->
        Assert.True(rows.Length > 0)
    | Error ex ->
        failwithf "%A" ex

[<Fact>]
let ``when server is not found then returns Error exn Result`` () =
    let result = 
        Customer.selectGreaterThanId 0
        |> Sql.executeReadFirst "server=(doesnotexist); database=any; Integrated Security=SSPI"

    match result with
    | Ok _ ->
        failwith "Connection should have failed!"
    | Error ex ->
        ()

[<Fact>]
let ``when database is not found then returns Error exn Result`` () =
    let result = 
        Customer.selectGreaterThanId 0
        |> Sql.executeReadFirst "server=(local); database=doesnotexist; Integrated Security=SSPI"

    match result with
    | Ok _ ->
        failwith "Connection should have failed!"
    | Error ex ->
        ()