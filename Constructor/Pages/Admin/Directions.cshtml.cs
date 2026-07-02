using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class DirectionsModel : PageModel
{
    private readonly AppDbContext _db;

    public DirectionsModel(AppDbContext db)
    {
        _db = db;
    }

    public List<Direction> Directions { get; set; } = new();

    [BindProperty]
    public Direction NewDirection { get; set; } = new();

    public async Task OnGetAsync()
    {
        Directions = await _db.Directions
            .OrderBy(direction => direction.Order)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreateDirectionAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        await _db.Directions.AddAsync(NewDirection);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteDirectionAsync(int id)
    {
        var direction = await _db.Directions.FindAsync(id);

        if (direction is null)
            return RedirectToPage();

        _db.Directions.Remove(direction);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateDirectionAsync(int id, string title, string description, string technologies, int order)
    {
        if (string.IsNullOrWhiteSpace(title) ||
            string.IsNullOrWhiteSpace(description) ||
            string.IsNullOrWhiteSpace(technologies))
        {
            ModelState.AddModelError(string.Empty, "Название, описание и технологии обязательны");
            await OnGetAsync();
            return Page();
        }

        var direction = await _db.Directions.FindAsync(id);

        if (direction is null)
            return RedirectToPage();

        direction.Title = title;
        direction.Description = description;
        direction.Technologies = technologies;
        direction.Order = order;

        await _db.SaveChangesAsync();

        return RedirectToPage();
    }
}
