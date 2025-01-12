﻿using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using  NUnit.Framework;

namespace test.integration.UserTest;

public class EditTest
{
    private HttpClient _httpClient;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        Helper.TriggerRebuild();
    }

    [TestCase("teste@email.com", "Valid name", "1234567890","testeteste@email.com", TestName = "Valid")]
    [TestCase("teste@email.com", "Valid name", "1234567890","testeemailom", TestName = "InvalidEmail")]
    [TestCase("teste@email.com", "Valid name", "1234567890","" ,TestName = "EmptyEmail" )]
    public async Task Edit(
        string email,
        string name,
        string phone,
        string newEmail
    )
    {
        var user = new
        {
            Email = email,
            FullName = name,
            phoneNumber = phone,
        };
        
        var token = await Helper.Authorize(email);
        
        
        var editUser = new
        {
            Email = newEmail,
            FullName = name,
            phoneNumber = phone,
        };
        
        string url = "http://localhost:5100/api/user/profileinfo";
        HttpResponseMessage response;

        try
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            response = await _httpClient.PutAsJsonAsync(url, editUser);
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
                case "InvalidEmail":
                    response.IsSuccessStatusCode.Should().BeFalse();
                    break;
                case "EmptyEmail":
                    response.IsSuccessStatusCode.Should().BeFalse();
                    break;
                default:
                    response.IsSuccessStatusCode.Should().BeFalse();
                    break;
            }
        }
    }
}