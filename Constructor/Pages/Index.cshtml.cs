using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    private readonly HomePageContentService _contentService;

    public IndexModel(HomePageContentService contentService)
    {
        _contentService = contentService;
    }

    public List<HeroStat> Stats { get; set; } = new();
    public HeroContent Hero { get; set; } = new();
    public List<TeamMember> TeamMembers { get; set; } = new();
    public List<Vacancy> Vacancies { get; set; } = new();
    public List<Direction> Directions { get; set; } = new();
    public List<Benefit> Benefits { get; set; } = new();

    public async Task OnGetAsync()
    {
        Hero = await _contentService.GetHeroContentAsync();
        Stats = await _contentService.GetStatsAsync();
        TeamMembers = await _contentService.GetTeamMembersAsync();
        Vacancies = await _contentService.GetVacanciesAsync();
        Directions = await _contentService.GetDirectionsAsync();
        Benefits = await _contentService.GetBenefitsAsync();
    }
}
