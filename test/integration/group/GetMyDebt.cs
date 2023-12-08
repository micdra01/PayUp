using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;

namespace test.integration.group;

public class GetMyDebt
{
    private HttpClient _httpClient;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        Helper.TriggerRebuild();
    }

    [Test]
    public async Task GetMyDebtTest()
    {
        var token = await Helper.Authorize("user@example.com");
        Helper.RunScript(Helper.BalanceScript);
        
        string url = "http://localhost:5100/api/group/1/debt";
        HttpResponseMessage response;

        try
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            response = await _httpClient.GetAsync(url);
            TestContext.WriteLine("The full body response: "
                                  + await response.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            throw new Exception(Helper.NoResponseMessage, e);
        }

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();
            var body = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Models.Transaction[]>(body);

            // As per Helper.BalanceScript:
            // User 1's balance should be -10
            // User 2's balance should be -20
            // User 3's balance should be +30
            // Meaning that user 1 only has one debt transactions

            data.Length.Should().Be(1);
            data[0].PayerId.Should().Be(1);
            data[0].Amount.Should().Be(10);
            data[0].PayeeId.Should().Be(3);
        }
    }
}