[![Build status](https://ci.appveyor.com/api/projects/status/j8uap8rf0y337b39/branch/master?svg=true)](https://ci.appveyor.com/project/programmersdigest/datamapper/branch/master)
# DataMapper
A slim yet powerful ORM layer.

## Contributors
Awesome-Source
programmersdigest

## Description
This project implements an object-relational-mapper. The goal of this project is to allow fluid work with (preferably strongly typed) objects instead of DataTables and DataRows. It does not provide database abstraction, as SQL-Code is still required.

More often than not, ORM is used as a name for two very different aspects of working with a database:
1. Mapping relational (row-based) data to objects in the programming language you are working with
2. Abstracting away SQL-Code to make switching the database easier.

In my experience, point 1. is usually a good idea, because it makes working with the data more fluid and thus easier. Database abstraction however is not a requirement for most small to medium projects. Additionally, SQL is a very powerful language and a lot of this power gets lost by abstracting too much of it away. For various reasons (performance being one the top ones) most larger projects seem to become a mess of abstracted (e.g. LINQ2SQL) and direct SQL, in which the latter is typically not database independent. In the end you get the worst of both worlds.
This project is thus based on the idea, that object mapping is a worthy goal, whereas SQL abstraction (to gain database independence) is often not required.

**Disclaimer**: This project is provided "as is". It may be incomplete and/or faulty. The author(s) of this project cannot be held responsible for any damage occurring due to using this software.

## Features
- Mapping of relational data to strongly typed objects or dynamic
- Various helper methods for CRUD operations (all using parameterized queries)
- Full control over the SQL sent to the database
- Makes extensive use of async
- Interoperability with most databases available via ADO.NET (mostly tested on SQLite, but should work with MSSQL, Oracle, MySQL, etc.)
- Small and easy to use

## Usage
Grab the latest version from NuGet https://www.nuget.org/packages/programmersdigest.DataMapper

**Initialization**
```C#
// Create the database (e.g. for SQLite), which takes
// - a ConnectionBuilder delegate to create the SqlConnection and
// - a LastInsertIdSelector delegate to change insert commands to retrieve the primary key of the inserted item.
var database = new Database(() => new SQLiteConnection(@"Data Source=data.db;Version=3"),
                            (cmd) => cmd.CommandText += " SELECT Last_Insert_Rowid();");

// The same using programmersdigest.Injector
diContainer.RegisterSingleton<ConnectionBuilder>(() => new SQLiteConnection(@"Data Source=data.db;Version=3"));
diContainer.RegisterSingleton<LastInsertIdSelector>(cmd => cmd.CommandText += " SELECT Last_Insert_Rowid();");
diContainer.RegisterType<Database>();

var database = diContainer.Get<Database>();
```

**Executing SQL**
```C#
// Get data from the database (as dynamic)
return await database.Select("SELECT * FROM \"MyTable\"");

// Just provide a type to get strongly typed data
return await database.Select<MyTable>("SELECT * FROM \"MyTable\"");

// To execute arbitrary SQL, use the method Execute()
await database.Execute(@"CREATE TABLE IF NOT EXISTS Employee (
    Id INTEGER PRIMARY KEY,
    FirstName TEXT NOT NULL,
    FamilyName TEXT NOT NULL,
    Gender TEXT NOT NULL,
    DepartmentId INTEGER NOT NULL,
    FOREIGN KEY(DepartmentId) REFERENCES Department(Id)
);");

// There are various additional helper methods for CRUD.
// A data service from my personal housekeeping book:
public async Task<IEnumerable<Account>> GetAccounts() {
    return await _database.Select<Account>("SELECT * FROM \"Account\" ORDER BY Name DESC");
}

public async Task<Account> GetAccountById(long id) {
    return await _database.SelectSingle<Account>("SELECT * FROM \"Account\" WHERE Id = @Id", new Dictionary<string, object> {
        ["@Id"] = id
    });
}

public async Task<Account> AddAccount(Account account) {
    account.CreationDate = DateTime.Now;
    account.ModificationDate = DateTime.Now;

    var id = await _database.Insert(account);
    return await GetAccountById(id);
}

public async Task UpdateAccount(Account account) {
    account.ModificationDate = DateTime.Now;

    await _database.Update(account);
}

public async Task DeleteAccount(Account account) {
    await _database.Delete(account);
}
```

**Creating models**
```C#
// The model class should be named like the table. If this is not possible, the
// NameAttribute allows names differing from the database model.
public class Employee {
    // The primary key has to be defined. There must always be a single primary key
    // column. Support for multiple primary key columns may be added in the future.
    [PrimaryKey]
    public long Id { get; set; }
    
    // Columns are mapped to Properties. These properties must be public and have a
    // getter as well as a setter. Data types need to match the table columns types
    // (the actual type mapping can be seen in DataExtensions.GetFieldValue).
    // Properties should be named like the matching column. If a matching name cannot
    // be used, the NameAttribute allows names differing from the database model.
    public string FirstName { get; set; }
    [Name("FamilyName")]
    public string SecondName { get; set; }
    
    // Enums can be saved as numbers (byte, int, long) or as strings
    public Gender Gender { get; set; }
    
    // Foreign keys are normal fields with no inherent special meaning. DataMapper
    // does not enforce foreign keys. This is a job handled very well by any database.
    public long DepartmentId { get; set; }
    
    // For convenience, additional data (like foreign table rows) can be included in the model
    // class. Properties which are not part of the database model must be decorated with the
    // ForeignDataAttribute and will be ignored by DataMapper.
    // Note: DataMapper does not automatically lazy load foreign key data as is done by e.g.
    // Entity Framework or LINQ2SQL.
    [ForeignData]
    public Department Department { get; set; }
}
```

## Todos
- Add comments on public members
- Add unit and integration tests
- Add support for insert scenarios with multiple primary keys
