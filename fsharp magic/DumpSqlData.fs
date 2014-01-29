module public DumpSqlData

open System.Data.Linq
open System.Data.EntityClient
open Microsoft.FSharp.Data.TypeProviders
open System.IO
open Microsoft.FSharp.Reflection
open System.Data.Linq.SqlClient
open System.Linq

//Let's create some connection
type EntityConnection = SqlEntityConnection<ConnectionString="Server=sqlServer;Initial Catalog=dbName;User=user;password=password;MultipleActiveResultSets=true",
                                                      Pluralize = false>

//now we have to get some context
let context = EntityConnection.GetDataContext()
    
//checks if given field is simple type, ie. if it is some of System.Int32, System.Boolean or if it is Nullable type
//later on we will use this to get values from entieties that are database fields
let isSimpleType (typeName:System.Reflection.PropertyInfo) = 
    let tn = typeName.PropertyType.ToString()
    tn.StartsWith("System.") && (2 = tn.Split('.').Length || tn.Contains("Nullable"))  //types like System.Int32, System.String etc

//function that get entity from database, get all its properties, filter it for simple types, use mapping funcion on all of them and then concatenate them for one line string (as line is CSV file)
let getLine mappingFunc x  =
    x.GetType().GetProperties() 
    |> Array.filter isSimpleType 
    |> Array.map (mappingFunc x)
    |> String.concat ","
   
//convert value to so it can be used in cvs file (ie add " to string and escape existing ones)
let convertValue value (typeName:System.Reflection.PropertyInfo) =
    match typeName.PropertyType.ToString() with
    | "System.Int32" -> value
    | "System.String" -> sprintf "\"%s\"" (value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n"))
    | _ -> value

//get field name (same as column name in database)
let headerValue x (p:System.Reflection.PropertyInfo) = p.Name

//get value of property and convert it if it is not null
let lineValue x (p:System.Reflection.PropertyInfo) = 
    match (p.GetValue(x, null)) with
    | null -> "NULL"
    | a -> convertValue (a.ToString()) p

//main function that dumps data from sequence taken from database into cvs format
let dumpData  dumper sequence =
    sequence |> Seq.head |> (getLine headerValue) |> dumper
    sequence |> Seq.map (getLine lineValue) |> Seq.iter dumper

//function that get all records from given table and dumps it using passed function
let dumpDataFromTable dumper table =
    let sequence = query { for sev in table do 
                           select sev }
    dumpData  dumper sequence

//save data from given table, using dumpData into file 
let saveFile (table:System.Data.Objects.ObjectSet<'a>) =
    let tableName = table.EntitySet.ToString()
    use writer = new StreamWriter((sprintf @"C:\temp\%s.csv" tableName), false)
    dumpDataFromTable writer.WriteLine table 
    printfn "table %s done" tableName

//save data from given collection to file
let saveFileWithData tableName data =
    use writer = new StreamWriter((sprintf @"C:\temp\%s.csv" tableName), false)
    dumpData writer.WriteLine data 
    printfn "table %s done" tableName



//dump full table
saveFile context.List_Status

//dump selected data from big table
let items = query { for el in context.Table_With_Contents do
                    take 20 
                    sortByDescending el.Date
                    where (el.ID < 200)
                    select el } 

saveFileWithData "Table_With_Contents" items

System.Console.ReadKey() |> ignore