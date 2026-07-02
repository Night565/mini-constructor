using System.ComponentModel.DataAnnotations;

public class Vacancy
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Формат работы обязателен")]
    public string WorkFormat { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ссылка обязательна")]
    [Url(ErrorMessage = "Введите корректную ссылку")]
    public string Url { get; set; } = string.Empty;

    public int Order { get; set; }
}
