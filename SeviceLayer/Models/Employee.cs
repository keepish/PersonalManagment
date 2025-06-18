namespace SeviceLayer.Models;

public partial class Employee
{
    public int Id { get; set; }

    public int DepartmentId { get; set; }

    public int PositionId { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateTime? Birthday { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;

    public virtual Position Position { get; set; } = null!;

    public string FullName
    {
        get
        {
            var parts = new List<string> { Surname, Name };
            if (!string.IsNullOrWhiteSpace(Patronymic))
                parts.Add(Patronymic);

            return string.Join(" ", parts);
        }
    }
}
