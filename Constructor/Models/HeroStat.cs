using System.ComponentModel.DataAnnotations;

public class HeroStat
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Заголовок обязательно")]
    public string Value { get; set; } = string.Empty;

    [Required(ErrorMessage = "Описание обязательно")]
    public string Label { get; set; } = string.Empty;

    public int Order { get; set; }
}
