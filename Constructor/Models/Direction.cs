using System.ComponentModel.DataAnnotations;

public class Direction
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Описание обязательно")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Технологии обязательны")]
    public string Technologies { get; set; } = string.Empty;

    public int Order { get; set; }
}
