using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class StatsModel : PageModel
{
    private readonly AppDbContext _db;

    public StatsModel(AppDbContext db)
    {
        _db = db;
    }

    public List<HeroStat> Stats { get; set; } = new();

    [BindProperty]
    public HeroStat NewStat { get; set; } = new();

    public async Task OnGetAsync()
    {
        Stats = await _db.HeroStats
            .OrderBy(stat => stat.Order)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreateStatAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        await _db.HeroStats.AddAsync(NewStat);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteStatAsync(int id)
    {
        var stat = await _db.HeroStats.FindAsync(id);

        if (stat is null)
            return RedirectToPage();

        _db.HeroStats.Remove(stat);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateStatAsync(int id, string value, string label, int order)
    {
        if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(label))
        {
            ModelState.AddModelError(string.Empty, "Заголовок и описание обязательны");
            await OnGetAsync();
            return Page();
        }

        var stat = await _db.HeroStats.FindAsync(id);

        if (stat is null)
            return RedirectToPage();

        stat.Value = value;
        stat.Label = label;
        stat.Order = order;

        await _db.SaveChangesAsync();

        return RedirectToPage();
    }
}
