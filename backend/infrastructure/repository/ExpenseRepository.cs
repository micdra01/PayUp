using System.Data.SqlTypes;
using api.models;
using Dapper;
using infrastructure.dataModels;
using Npgsql;

namespace infrastructure.repository;

public class ExpenseRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ExpenseRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<Expense> GetAllExpenses(int groupId)
    {
        var sql =
            $@"
            select 
                e.id as {nameof(Expense.Id)}, 
                e.user_id as {nameof(Expense.UserId)}, 
                e.group_id as {nameof(Expense.GroupId)},
                e.description as {nameof(Expense.Description)},
                e.amount as {nameof(Expense.Amount)},
                e.created_date as {nameof(Expense.CreatedDate)},
                u.full_name as {nameof(Expense.FullName)}, 
                e.is_settle as {nameof(Expense.IsSettle)}
            from expenses.expense as e 
                join users.user as u on e.user_id = u.id 
            where group_id = @groupId;
            ";

        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.Query<Expense>(sql, new { groupId });
        }
        catch (Exception e)
        {
            throw new SqlNullValueException(" read expenses from group", e);
        }
    }

    
    
    public IEnumerable<GetUserOnExpense> GetUsersOnExpenses(int groupId)
    {
        var sql =
            $@"
            select 
                uoe.user_id as {nameof(GetUserOnExpense.UserId)}, 
                expense_id as {nameof(GetUserOnExpense.ExpenseId)}, 
                uoe.amount as {nameof(GetUserOnExpense.Amount)}, 
                u.full_name as {nameof(GetUserOnExpense.FullName)},
                u.profile_url as {nameof(GetUserOnExpense.ImageUrl)}
            from expenses.expense 
                join expenses.user_on_expense as uoe on expense.id = uoe.expense_id 
                join users.user as u on uoe.user_id = u.id 
            where group_id = @groupId;
            ";

        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.Query<GetUserOnExpense>(sql, new { groupId });
        }
        catch (Exception e)
        {
            throw new SqlNullValueException(" read users on expenses from group", e);
        }
    }

    public IEnumerable<BalanceDto> GetBalances(int groupId)
    {
        var sql = 
            $@"
        SELECT 
            uoe.user_id as {nameof(BalanceDto.UserId)}, 
            u.full_name as {nameof(BalanceDto.FullName)}, 
            u.profile_url as {nameof(BalanceDto.ImageUrl)},
            SUM(uoe.amount) as {nameof(BalanceDto.Amount)}
        FROM expenses.user_on_expense as uoe
            JOIN expenses.expense as e ON uoe.expense_id = e.id 
            JOIN users.user as u ON uoe.user_id = u.id  
            JOIN groups.group as g ON e.group_id = g.id
        WHERE g.id = @groupId
        GROUP BY uoe.user_id, u.full_name, u.profile_url
        ORDER BY {nameof(BalanceDto.Amount)} DESC;
    ";

        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.Query<BalanceDto>(sql, new { groupId });
        }
        catch (Exception e)
        {
            throw new SqlNullValueException(" read balances from group", e);
        }
    }
    public Expense CreateExpense(CreateExpenseDto expenseDto)
    {
        var sql = $@"
        INSERT INTO expenses.expense (user_id, group_id, description, amount, created_date, is_settle)
        VALUES (@UserId, @GroupId, @Description, @Amount, @CreatedDate, @IsSettle)
        RETURNING
            id AS {nameof(Expense.Id)},
            user_id AS {nameof(Expense.UserId)},
            group_id AS {nameof(Expense.GroupId)},
            description AS {nameof(Expense.Description)},
            amount AS {nameof(Expense.Amount)},
            created_date AS {nameof(Expense.CreatedDate)},
            is_settle AS {nameof(Expense.IsSettle)};
            ";

        try
        {
            using var conn = _dataSource.OpenConnection();
            var expense = conn.QueryFirst<Expense>(sql, new
            {
                UserId = expenseDto.UserId,
                GroupId = expenseDto.GroupId,
                Description = expenseDto.Description,
                Amount = expenseDto.Amount,
                CreatedDate = expenseDto.CreatedDate,
                IsSettle = expenseDto.IsSettle
            });

            return expense;
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Could not create expense", e);
        }
    }

    public IEnumerable<GetUserOnExpense?> AddUsersToExpense(IEnumerable<CreateUserOnExpense>? usersOnExpense)
    {
        var sql1 = "";
        var expenseId = 0;
        
        if (usersOnExpense == null) throw new SqlNullValueException();
        
        foreach (var u in usersOnExpense!)
        {
            sql1 += $@"
            INSERT INTO expenses.user_on_expense (user_id, expense_id, amount)
            VALUES ({u.UserId}, {u.ExpenseId}, {u.Amount}); 
            ";
            
            expenseId = u.ExpenseId;
        }

        var sql2 = 
            $@"
            select 
                uoe.user_id as {nameof(GetUserOnExpense.UserId)}, 
                expense_id as {nameof(GetUserOnExpense.ExpenseId)}, 
                uoe.amount as {nameof(GetUserOnExpense.Amount)},
                u.profile_url as {nameof(GetUserOnExpense.ImageUrl)}
            from expenses.expense 
                join expenses.user_on_expense as uoe on expense.id = uoe.expense_id 
                join users.user as u on uoe.user_id = u.id 
            where expense_id = {expenseId};
        ";

        try
        {
            using var conn = _dataSource.OpenConnection();
            conn.Execute(sql1);
            return conn.Query<GetUserOnExpense?>(sql2);
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Could not add users to expense", e);
        }
    }
    
    public TotalBalanceDto GetTotalBalance(int userId)
    {
        var sql =
            $@"
            select SUM(expenses.user_on_expense.amount) as {nameof(TotalBalanceDto.Amount)}
            from expenses.user_on_expense
            where user_id = {userId}
            group by user_id;";

        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.QueryFirst<TotalBalanceDto>(sql);
        }
        catch (InvalidOperationException ioe)
        {
            return new TotalBalanceDto()
            {
                Amount = 0
            };
        }
        catch (Exception e)
        {
            throw new SqlNullValueException(" read total balance", e);
        }
    }

    public bool DeleteExpense(int expenseId)
    {
        var sql = 
            $@"
            DELETE FROM expenses.expense 
            WHERE id = {@expenseId};
            ";

        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.Execute(sql, new { expenseId }) == 1;
        }
        catch (Exception e)
        {
            throw new SqlNullValueException(" delete expense", e);
        }
    }

    public bool IsUserOnExpense(int expenseId, int userId)
    {
        var sql =
            @$"
            SELECT COUNT(*) 
            FROM expenses.expense 
            WHERE id = {@expenseId} AND user_id = {@userId} ;
            ";

        try
        {
            using var conn = _dataSource.OpenConnection();
            return conn.QueryFirst<int>(sql, new {expenseId, userId}) == 1;
        }
        catch (Exception e)
        {
            throw new SqlNullValueException(" check access", e);
        }
    }
    
}