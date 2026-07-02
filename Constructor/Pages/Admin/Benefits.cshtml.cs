using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class BenefitsModel : PageModel
{
    private readonly AppDbContext _db;

    public BenefitsModel(AppDbContext db)
    {
        _db = db;
    }

    public List<Benefit> Benefits { get; set; } = new();

    [BindProperty]
    public Benefit NewBenefit { get; set; } = new();

    public async Task OnGetAsync()
    {
        Benefits = await _db.Benefits
            .OrderBy(benefit => benefit.Order)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreateBenefitAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        await _db.Benefits.AddAsync(NewBenefit);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteBenefitAsync(int id)
    {
        var benefit = await _db.Benefits.FindAsync(id);

        if (benefit is null)
            return RedirectToPage();

        _db.Benefits.Remove(benefit);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateBenefitAsync(int id, string title, string description, int order)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
        {
            ModelState.AddModelError(string.Empty, "Заголовок и описание обязательны");
            await OnGetAsync();
            return Page();
        }

        var benefit = await _db.Benefits.FindAsync(id);

        if (benefit is null)
            return RedirectToPage();

        benefit.Title = title;
        benefit.Description = description;
        benefit.Order = order;

        await _db.SaveChangesAsync();

        return RedirectToPage();
    }
}
