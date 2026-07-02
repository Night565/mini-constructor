using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var tests = new List<(string Name, Func<Task> Run)>
{
    ("Home page service returns stats ordered by Order", HomePageServiceReturnsStatsOrderedByOrder),
    ("Home page service returns default hero when database is empty", HomePageServiceReturnsDefaultHeroWhenDatabaseIsEmpty),
    ("Home page service returns empty lists when database is empty", HomePageServiceReturnsEmptyListsWhenDatabaseIsEmpty),
    ("Hero content can be created and updated", HeroContentCanBeCreatedAndUpdated),
    ("Team member can be added, updated, and deleted", TeamMemberCanBeAddedUpdatedAndDeleted),
    ("Vacancy can be added, updated, and deleted", VacancyCanBeAddedUpdatedAndDeleted),
    ("Direction and benefit lists are ordered", DirectionAndBenefitListsAreOrdered),
    ("Required fields are validated", RequiredFieldsAreValidated),
    ("Vacancy URL is validated", VacancyUrlIsValidated)
};

var failed = 0;

foreach (var test in tests)
{
    try
    {
        await test.Run();
        Console.WriteLine($"PASS: {test.Name}");
    }
    catch (Exception ex)
    {
        failed++;
        Console.WriteLine($"FAIL: {test.Name}");
        Console.WriteLine(ex.Message);
    }
}

if (failed > 0)
{
    Console.WriteLine($"{failed} test(s) failed.");
    Environment.Exit(1);
}

Console.WriteLine("All tests passed.");

static async Task HomePageServiceReturnsStatsOrderedByOrder()
{
    await using var database = await TestDatabase.CreateAsync();

    database.Context.HeroStats.AddRange(
        new HeroStat { Value = "12 000+", Label = "клиентов", Order = 2 },
        new HeroStat { Value = "300+", Label = "сотрудников", Order = 1 });
    await database.Context.SaveChangesAsync();

    var service = new HomePageContentService(database.Context);
    var stats = await service.GetStatsAsync();

    AssertEqual(2, stats.Count, "Stats count");
    AssertEqual("300+", stats[0].Value, "First stat should have the smallest Order");
    AssertEqual("12 000+", stats[1].Value, "Second stat should have the largest Order");
}

static async Task HomePageServiceReturnsDefaultHeroWhenDatabaseIsEmpty()
{
    await using var database = await TestDatabase.CreateAsync();

    var service = new HomePageContentService(database.Context);
    var hero = await service.GetHeroContentAsync();

    AssertEqual("Создаем технологии для отелей и путешествий", hero.Title, "Default hero title");
    AssertEqual(
        "Стабильно растем, развиваем платформу TravelLine и принимаем новые вызовы TravelTech-индустрии.",
        hero.Subtitle,
        "Default hero subtitle");
}

static async Task HomePageServiceReturnsEmptyListsWhenDatabaseIsEmpty()
{
    await using var database = await TestDatabase.CreateAsync();

    var service = new HomePageContentService(database.Context);

    AssertEqual(0, (await service.GetStatsAsync()).Count, "Stats list should be empty");
    AssertEqual(0, (await service.GetTeamMembersAsync()).Count, "Team members list should be empty");
    AssertEqual(0, (await service.GetDirectionsAsync()).Count, "Directions list should be empty");
    AssertEqual(0, (await service.GetVacanciesAsync()).Count, "Vacancies list should be empty");
    AssertEqual(0, (await service.GetBenefitsAsync()).Count, "Benefits list should be empty");
}

static async Task HeroContentCanBeCreatedAndUpdated()
{
    await using var database = await TestDatabase.CreateAsync();

    database.Context.HeroContents.Add(new HeroContent
    {
        Title = "Первый заголовок",
        Subtitle = "Первый подзаголовок"
    });
    await database.Context.SaveChangesAsync();

    var hero = await database.Context.HeroContents.FirstAsync();
    hero.Title = "Новый заголовок";
    hero.Subtitle = "Новый подзаголовок";
    await database.Context.SaveChangesAsync();

    var updatedHero = await database.Context.HeroContents.FirstAsync();
    AssertEqual("Новый заголовок", updatedHero.Title, "Hero title should be updated");
    AssertEqual("Новый подзаголовок", updatedHero.Subtitle, "Hero subtitle should be updated");
}

static async Task TeamMemberCanBeAddedUpdatedAndDeleted()
{
    await using var database = await TestDatabase.CreateAsync();

    var member = new TeamMember
    {
        FullName = "Иван Петров",
        Position = "Backend-разработчик",
        PhotoPath = "/uploads/team/ivan.jpg",
        Order = 1
    };

    database.Context.TeamMembers.Add(member);
    await database.Context.SaveChangesAsync();
    AssertEqual(1, await database.Context.TeamMembers.CountAsync(), "Team member should be added");

    member.Position = "Lead Backend-разработчик";
    await database.Context.SaveChangesAsync();

    var updatedMember = await database.Context.TeamMembers.FirstAsync();
    AssertEqual("Lead Backend-разработчик", updatedMember.Position, "Team member position should be updated");

    database.Context.TeamMembers.Remove(updatedMember);
    await database.Context.SaveChangesAsync();
    AssertEqual(0, await database.Context.TeamMembers.CountAsync(), "Team member should be deleted");
}

static async Task VacancyCanBeAddedUpdatedAndDeleted()
{
    await using var database = await TestDatabase.CreateAsync();

    var vacancy = new Vacancy
    {
        Title = "Backend-разработчик .NET",
        WorkFormat = "Удаленно",
        Url = "https://hh.ru/",
        Order = 1
    };

    database.Context.Vacancies.Add(vacancy);
    await database.Context.SaveChangesAsync();
    AssertEqual(1, await database.Context.Vacancies.CountAsync(), "Vacancy should be added");

    vacancy.WorkFormat = "Удаленно или офис";
    await database.Context.SaveChangesAsync();

    var updatedVacancy = await database.Context.Vacancies.FirstAsync();
    AssertEqual("Удаленно или офис", updatedVacancy.WorkFormat, "Vacancy work format should be updated");

    database.Context.Vacancies.Remove(updatedVacancy);
    await database.Context.SaveChangesAsync();
    AssertEqual(0, await database.Context.Vacancies.CountAsync(), "Vacancy should be deleted");
}

static async Task DirectionAndBenefitListsAreOrdered()
{
    await using var database = await TestDatabase.CreateAsync();

    database.Context.Directions.AddRange(
        new Direction { Title = "Frontend", Description = "Клиентская часть", Technologies = "TypeScript", Order = 2 },
        new Direction { Title = "Backend", Description = "Серверная часть", Technologies = ".NET", Order = 1 });

    database.Context.Benefits.AddRange(
        new Benefit { Title = "Обучение", Description = "Помогаем развиваться", Order = 2 },
        new Benefit { Title = "Онбординг", Description = "Помогаем стартовать", Order = 1 });

    await database.Context.SaveChangesAsync();

    var service = new HomePageContentService(database.Context);
    var directions = await service.GetDirectionsAsync();
    var benefits = await service.GetBenefitsAsync();

    AssertEqual("Backend", directions[0].Title, "Directions should be ordered");
    AssertEqual("Онбординг", benefits[0].Title, "Benefits should be ordered");
}

static Task RequiredFieldsAreValidated()
{
    AssertInvalid(new HeroContent { Title = "", Subtitle = "Подзаголовок" }, "Hero title should be required");
    AssertInvalid(new HeroStat { Value = "", Label = "сотрудников", Order = 1 }, "Stat value should be required");
    AssertInvalid(new TeamMember { FullName = "", Position = "Backend", Order = 1 }, "Team member name should be required");
    AssertInvalid(new Direction { Title = "Backend", Description = "", Technologies = ".NET", Order = 1 }, "Direction description should be required");
    AssertInvalid(new Benefit { Title = "Обучение", Description = "", Order = 1 }, "Benefit description should be required");

    return Task.CompletedTask;
}

static Task VacancyUrlIsValidated()
{
    AssertInvalid(
        new Vacancy
        {
            Title = "Backend-разработчик .NET",
            WorkFormat = "Удаленно",
            Url = "not-a-url",
            Order = 1
        },
        "Vacancy URL should be valid");

    AssertValid(
        new Vacancy
        {
            Title = "Backend-разработчик .NET",
            WorkFormat = "Удаленно",
            Url = "https://hh.ru/",
            Order = 1
        },
        "Vacancy with valid URL should pass validation");

    return Task.CompletedTask;
}

static void AssertEqual<T>(T expected, T actual, string message)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new InvalidOperationException($"{message}. Expected: {expected}. Actual: {actual}.");
}

static void AssertValid(object model, string message)
{
    var results = Validate(model);

    if (results.Count > 0)
        throw new InvalidOperationException($"{message}. Validation errors: {string.Join("; ", results.Select(result => result.ErrorMessage))}");
}

static void AssertInvalid(object model, string message)
{
    var results = Validate(model);

    if (results.Count == 0)
        throw new InvalidOperationException($"{message}. Expected validation errors, but model is valid.");
}

static List<ValidationResult> Validate(object model)
{
    var results = new List<ValidationResult>();
    var context = new ValidationContext(model);

    Validator.TryValidateObject(model, context, results, validateAllProperties: true);

    return results;
}

internal sealed class TestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    private TestDatabase(SqliteConnection connection, AppDbContext context)
    {
        _connection = connection;
        Context = context;
    }

    public AppDbContext Context { get; }

    public static async Task<TestDatabase> CreateAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        return new TestDatabase(connection, context);
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
