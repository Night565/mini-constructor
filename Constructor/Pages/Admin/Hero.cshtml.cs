using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class HeroModel : PageModel
{
    private readonly AppDbContext _db;

    public HeroModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public HeroContent Hero { get; set; } = new();

    public async Task OnGetAsync()
    {
        Hero = await _db.HeroContents.FirstOrDefaultAsync()
            ?? GetDefaultHeroContent();
    }

    public async Task<IActionResult> OnPostSaveHeroContentAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var hero = await _db.HeroContents.FirstOrDefaultAsync();

        if (hero is null)
        {
            await _db.HeroContents.AddAsync(Hero);
        }
        else
        {
            hero.Title = Hero.Title;
            hero.Subtitle = Hero.Subtitle;
        }

        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    private static HeroContent GetDefaultHeroContent()
    {
        return new HeroContent
        {
            Title = "Создаем технологии для отелей и путешествий",
            Subtitle = "Стабильно растем, развиваем платформу TravelLine и принимаем новые вызовы TravelTech-индустрии."
        };
    }
}
