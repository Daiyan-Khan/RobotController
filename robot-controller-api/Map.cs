namespace robot_controller_api;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

public class Map
{
    /// Implement <see cref="Map"> here following the task sheet requirements
    public int Id { get; set; }
    public int Columns { get; set; }
    public int Rows { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public bool IsSquare{ get; set; }
    public Map(int id, int columns, int rows, string name, string? description, DateTime createdDate, DateTime modifiedDate)
    {
        Id = id;
        Columns = columns;
        Rows = rows;
        Name = name;
        IsSquare = rows > 0 && rows == columns;
        Description = description;
        CreatedDate = createdDate;
        ModifiedDate = modifiedDate;
    }

    //Empty implementation as deafult for repository
    public Map()
    {
        //no code
    }
}
