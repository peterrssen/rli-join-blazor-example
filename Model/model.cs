using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Database
{
    [JsonPropertyName("tables")]
    public List<Table>? Tables { get; set; }

    public static Table DoLeftJoin(Table t1, Table t2)
    {
        Table table = InitTabale(t1, t2);

        Console.WriteLine(table.Name);

var joinedRows =
    from l in t1.Rows
    join r in t2.Rows
        on l[0].ToString() equals r[1].ToString() into gj
    from r in gj.DefaultIfEmpty()
    select l.Concat(
        r ?? Enumerable.Repeat<object?>(null, t2.ColumnNames.Count)
    ).ToList();

        Console.WriteLine(joinedRows.Count());

        table.Rows.AddRange(joinedRows);

        foreach (var row in table.Rows)
        {
            Console.WriteLine(row.Count);
            Console.WriteLine(string.Join(", ", row));
        }

        return table;
    }

    private static Table InitTabale(Table t1, Table t2) {
        Table table;

        table = new Table{
            Name = "",
            ColumnNames = t1.ColumnNames.Concat(t2.ColumnNames!).ToList(),
            Colors = t1.Colors.Concat(t2.Colors!).ToList(),
            Rows = new List<List<object>>()
        };

        return table;
    }
}

public class Table
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("columns")]
    public List<string>? ColumnNames { get; set; } = new List<string>();

    [JsonPropertyName("colors")]
    public List<string>? Colors { get; set; } = new List<string>();

    [JsonPropertyName("rows")]
    public List<List<object>>? Rows { get; set; } = new List<List<object>>();

}