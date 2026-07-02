using System.ComponentModel.DataAnnotations;

public class TeamMember
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Имя обязательно")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Должность обязательна")]
    public string Position { get; set; } = string.Empty;

    public string PhotoPath { get; set; } = string.Empty;

    public int Order { get; set; }
}
