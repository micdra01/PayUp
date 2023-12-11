using api.filters;
using api.models;
using infrastructure.dataModels;
using Microsoft.AspNetCore.Mvc;
using service.services;


namespace api.controllers;

[ApiController]
public class ExpenseController : ControllerBase
{
    private readonly ExpenseService _service;

    public ExpenseController(ExpenseService service)
    {
        _service = service;
    }

    [RequireAuthentication]
    [HttpPost]
    [Route("/api/expense/")]
    public FullExpense CreateExpense(CreateFullExpense expense)
    {
        return _service.CreateExpense(expense, HttpContext.GetSessionData()!);
    }
    
    [RequireAuthentication]
    [HttpPost]
    [Route("/api/group/{groupId}/settle/")]
    public FullExpense CreateSettle([FromRoute] int groupId, Transaction transaction)
    {
        return _service.CreateSettle(groupId, transaction, HttpContext.GetSessionData()!);
    }
    
    [RequireAuthentication]
    [HttpGet]
    [Route("/api/group/{groupId}/expenses")]
    public IEnumerable<FullExpense> GetAllExpenses([FromRoute] int groupId)
    {
        return _service.GetAllExpenses(groupId, HttpContext.GetSessionData()!);
    }

    [RequireAuthentication]
    [HttpGet]
    [Route("/api/group/{groupId}/balances")]
    public IEnumerable<BalanceDto> GetBalances([FromRoute] int groupId)
    {
        return _service.GetBalances(groupId, HttpContext.GetSessionData()!);
    }

    [HttpGet]
    [Route("/api/expense/currency")]
    public async Task<ResponseObject> GetAvailableCurrencies()
    {
        return await _service.GetAvailableCurrencies();
    }


    [RequireAuthentication]
    [HttpGet]
    [Route("/api/group/{groupId}/transactions")]
    public IEnumerable<Transaction> GetTransactions([FromRoute] int groupId)
    {
        return _service.GetTotalTransactions(groupId, HttpContext.GetSessionData()!);
    }

    
    [RequireAuthentication]
    [HttpGet]
    [Route("/api/group/{groupId}/debt")]
    public IEnumerable<Transaction> GetMyDebt([FromRoute] int groupId)
    {
        return _service.GetMyDebt(groupId, HttpContext.GetSessionData()!);
    }

    [RequireAuthentication]
    [HttpGet]
    [Route("/api/user/totalbalance")]
    public TotalBalanceDto GetTotalBalance()
    {
        return _service.GetTotalBalance(HttpContext.GetSessionData()!);
    }

    [RequireAuthentication]
    [HttpDelete]
    [Route("/api/expense/delete/{expenseId}")]
    public bool DeleteExpense([FromRoute] int expenseId)
    {
        return _service.DeleteExpense(expenseId, HttpContext.GetSessionData()!);
    }
   
}