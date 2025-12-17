using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public enum JoinType
{
    Left,
    Right,
    Inner
}

public class Database
{
    [JsonPropertyName("tables")]
    public List<Table>? Tables { get; set; }

    public static Table Join(Table t1, Table t2, JoinType joinType)
    {
        Table resultTable;
        List<List<object?>> joinedRows; 

        if (t1.Rows == null || t2.Rows == null) {
            throw new ArgumentException("Table rows cannot be null");
        }
        
        resultTable = InitTabale(t1, t2);

        joinedRows = joinType switch
        {
            JoinType.Left  => JoinLeft(t1, t2),
            JoinType.Right => JoinRight(t1, t2),
            JoinType.Inner => JoinInner(t1, t2),
            _ => throw new NotSupportedException()
        };

        resultTable.Rows.AddRange(joinedRows);

        return resultTable;
    }

    public static List<List<object?>> JoinLeft(Table t1, Table t2)
    {
        var joinedRows = 
            (
                from l in t1.Rows
                join r in t2.Rows on l[0].ToString() equals r[1].ToString() into gj
                from r in gj.DefaultIfEmpty()
                select l.Concat(
                    r ?? Enumerable.Repeat<object?>(null, t2.ColumnNames.Count)
                ).ToList()
            ).ToList();

        return joinedRows;
    }

    public static List<List<object>> JoinRight(Table t1, Table t2)
    {
        var joinedRows = 
        (
            from r in t2.Rows!             
            join l in t1.Rows! on r[1]?.ToString() equals l[0]?.ToString() into gj
            from l2 in gj.DefaultIfEmpty()            // linke Row oder null
            select (l2?.Cast<object?>()               // linke Row casten, NULL wenn kein Match
                ?? Enumerable.Repeat<object?>(null, t1.ColumnNames.Count))
                .Concat(r.Cast<object?>())        // rechte Row anhängen
                .ToList()
        ).ToList();

        return joinedRows;
    }

    public static List<List<object>> JoinInner(Table t1, Table t2)
    {
        List<List<object>> joinedRows = 
        (
            from r in t2.Rows
            join l in t1.Rows on r[1]!.ToString() equals l[0]!.ToString()
            select l.Concat(r).ToList()
        ).ToList();

        return joinedRows;
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
    public List<string?> ColumnNames { get; set; } = new List<string?>();

    [JsonPropertyName("colors")]
    public List<string?> Colors { get; set; } = new List<string?>();

    [JsonPropertyName("rows")]
    public List<List<object?>> Rows { get; set; } = new List<List<object?>>();

}