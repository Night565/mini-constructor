using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class TeamModel : PageModel
{
    private readonly AppDbContext _db;

    public TeamModel(AppDbContext db)
    {
        _db = db;
    }

    public List<TeamMember> TeamMembers { get; set; } = new();

    [BindProperty]
    public TeamMember NewTeamMember { get; set; } = new();

    [BindProperty]
    public IFormFile? NewPhotoFile { get; set; }

    public async Task OnGetAsync()
    {
        TeamMembers = await _db.TeamMembers
            .OrderBy(member => member.Order)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreateTeamMemberAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        if (NewPhotoFile is not null)
            NewTeamMember.PhotoPath = await SaveTeamPhotoAsync(NewPhotoFile);

        await _db.TeamMembers.AddAsync(NewTeamMember);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }   

    public async Task<IActionResult> OnPostDeleteTeamMemberAsync(int id)
    {
        var member = await _db.TeamMembers.FindAsync(id);

        if (member is null)
            return RedirectToPage();

        DeleteTeamPhoto(member.PhotoPath);

        _db.TeamMembers.Remove(member);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateTeamMemberAsync(int id, string fullName, string position, IFormFile? photoFile, int order)
    {
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(position))
        {
            ModelState.AddModelError(string.Empty, "Имя и должность обязательны");
            await OnGetAsync();
            return Page();
        }

        var member = await _db.TeamMembers.FindAsync(id);

        if (member is null)
            return RedirectToPage();

        member.FullName = fullName;
        member.Position = position;
        member.Order = order;

        if (photoFile is not null)
        {
            DeleteTeamPhoto(member.PhotoPath);
            member.PhotoPath = await SaveTeamPhotoAsync(photoFile);
        }

        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    private static async Task<string> SaveTeamPhotoAsync(IFormFile photoFile)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "team");

        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photoFile.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using var stream = System.IO.File.Create(filePath);
        await photoFile.CopyToAsync(stream);

        return $"/uploads/team/{fileName}";
    }

    private static void DeleteTeamPhoto(string photoPath)
    {
        if (string.IsNullOrWhiteSpace(photoPath))
            return;

        var relativePath = photoPath.TrimStart('/');
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        if (System.IO.File.Exists(fullPath))
            System.IO.File.Delete(fullPath);
    }
}
