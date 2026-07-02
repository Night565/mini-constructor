using System.ComponentModel.DataAnnotations;

public class Benefit
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Заголовок обязателен")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Описание обязательно")]
    public string Description { get; set; } = string.Empty;

    public int Order { get; set; }
}
