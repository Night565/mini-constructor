using System.ComponentModel.DataAnnotations;

public class HeroContent
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Заголовок обязателен")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подзаголовок обязателен")]
    public string Subtitle { get; set; } = string.Empty;
}
