using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class VacanciesModel : PageModel
{
    private readonly AppDbContext _db;

    public VacanciesModel(AppDbContext db)
    {
        _db = db;
    }

    public List<Vacancy> Vacancies { get; set; } = new();

    [BindProperty]
    public Vacancy NewVacancy { get; set; } = new();

    public async Task OnGetAsync()
    {
        Vacancies = await _db.Vacancies
            .OrderBy(vacancy => vacancy.Order)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreateVacancyAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        await _db.Vacancies.AddAsync(NewVacancy);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteVacancyAsync(int id)
    {
        var vacancy = await _db.Vacancies.FindAsync(id);

        if (vacancy is null)
            return RedirectToPage();

        _db.Vacancies.Remove(vacancy);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateVacancyAsync(int id, string title, string workFormat, string url, int order)
    {
        if (string.IsNullOrWhiteSpace(title) ||
            string.IsNullOrWhiteSpace(workFormat) ||
            string.IsNullOrWhiteSpace(url))
        {
            ModelState.AddModelError(string.Empty, "Название, формат работы и ссылка обязательны");
            await OnGetAsync();
            return Page();
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            ModelState.AddModelError(string.Empty, "Введите корректную ссылку");
            await OnGetAsync();
            return Page();
        }

        var vacancy = await _db.Vacancies.FindAsync(id);

        if (vacancy is null)
            return RedirectToPage();

        vacancy.Title = title;
        vacancy.WorkFormat = workFormat;
        vacancy.Url = url;
        vacancy.Order = order;

        await _db.SaveChangesAsync();

        return RedirectToPage();
    }
}
