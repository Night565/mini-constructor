using Microsoft.EntityFrameworkCore;

public class HomePageContentService
{
    private readonly AppDbContext _db;

    public HomePageContentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<HeroStat>> GetStatsAsync()
    {
        return await _db.HeroStats
            .OrderBy(stat => stat.Order)
            .ToListAsync();
    }

    public async Task<HeroContent> GetHeroContentAsync()
    {
        return await _db.HeroContents.FirstOrDefaultAsync()
            ?? new HeroContent
            {
                Title = "Создаем технологии для отелей и путешествий",
                Subtitle = "Стабильно растем, развиваем платформу TravelLine и принимаем новые вызовы TravelTech-индустрии."
            };
    }

    public async Task<List<TeamMember>> GetTeamMembersAsync()
    {
        return await _db.TeamMembers
            .OrderBy(member => member.Order)
            .ToListAsync();
    }

    public async Task<List<Vacancy>> GetVacanciesAsync()
    {
        return await _db.Vacancies
            .OrderBy(vacancy => vacancy.Order)
            .ToListAsync();
    }

    public async Task<List<Direction>> GetDirectionsAsync()
    {
        return await _db.Directions
            .OrderBy(direction => direction.Order)
            .ToListAsync();
    }

    public async Task<List<Benefit>> GetBenefitsAsync()
    {
        return await _db.Benefits
            .OrderBy(benefit => benefit.Order)
            .ToListAsync();
    }
}
