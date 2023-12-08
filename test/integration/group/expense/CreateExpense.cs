using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;

namespace test.integration.group;

public class CreateExpense
{
    private HttpClient _httpClient;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        Helper.TriggerRebuild();
    }

    [TestCase (1, 1, "ValidBajkkere", 15, "2023-10-24T15:58:41.045Z", TestName = "Valid")]
    [TestCase (1, 1, "", 15, "2023-10-24T15:58:41.045Z", TestName = "InvalidDescription")]
    [TestCase (1, 1, "ValidBajekre", -10, "2023-11-24T15:58:41.045Z", TestName = "InvalidAmount")] 
    
    public async Task Create(
        int userId,
        int groupId,
        string description,
        int amount,
        string createdDate
    )
    
    {
        var token = await Helper.Authorize("user@example.com");
        await Helper.Authorize("secondAccount@example.com");
        
        Helper.RunScript(Helper.GroupsScript);
        
        var expense = new
        {
            UserId = userId,
            GroupId = groupId,
            Description = description,
            Amount = amount,
            CreatedDate = createdDate,
            IsSettle = false
        };

        var userIdsOnExpense = new int[]
        {
            1, 2
        };

        var createFullExpense = new
        {
            expense,
            userIdsOnExpense
        };
        
        string url = "http://localhost:5100/api/expense";
        HttpResponseMessage response;

        try
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            response = await _httpClient.PostAsJsonAsync(url, createFullExpense);
            TestContext.WriteLine("The full body response: "
                                  + await response.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            throw new Exception(Helper.NoResponseMessage, e);
        }
        
        using (new AssertionScope())
        {
            string testName = TestContext.CurrentContext.Test.Name;

            switch (testName)
            {
                case "Valid":
                    response.IsSuccessStatusCode.Should().BeTrue();
                    break;
                default:
                    response.IsSuccessStatusCode.Should().BeFalse();
                    break;
            }
        }
    }
}