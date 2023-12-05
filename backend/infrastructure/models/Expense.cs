﻿using System.ComponentModel.DataAnnotations;

namespace api.models;

public class Expense
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public required string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? FullName { get; set; }
}

public class CreateExpenseDto
{
    [Required]
    public int UserId { get; set; }
    [Required]
    public int GroupId { get; set; }
    [Required]
    public required string Description { get; set; }
    [Required]
    [Range(0.00, Double.MaxValue)]
    public decimal Amount { get; set; }
    [Required]
    public DateTime CreatedDate { get; set; }
}

public class CreateUserOnExpense
{
    public required int UserId { get; set; }
    public required int ExpenseId { get; set; }
    public required decimal Amount { get; set; }
}

public class GetUserOnExpense
{
    public required int UserId { get; set; }
    public required int ExpenseId { get; set; }
    public required decimal Amount { get; set; }
    public required string ImageUrl { get; set; }
}

public class FullExpense
{
    public required Expense Expense { get; set; }
    public required IEnumerable<GetUserOnExpense>? UsersOnExpense { get; set; }
    public required int LoggedInUser { get; set; }
}

public class CreateFullExpense
{
    public required CreateExpenseDto Expense { get; set; }
    public required IEnumerable<int> UserIdsOnExpense { get; set; }
}

public class BalanceDto
{
    public required int UserId { get; set; }
    public required string FullName { get; set; }
    public required string ImageUrl { get; set; }
    public required decimal Amount { get; set; }
}

public class Transaction
{
    public int PayerId { get; set; }
    public string PayerName { get; set; }
    public decimal Amount { get; set; }
    public int PayeeId { get; set; }
    public string PayeeName { get; set; }

}