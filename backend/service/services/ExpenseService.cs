using System.Security.Authentication;
using api.models;
using infrastructure.dataModels;
using infrastructure.repository;

namespace service.services;

public class ExpenseService
{
    private readonly GroupRepository _groupRepo;
    private readonly ExpenseRepository _expenseRepo;
    private readonly UserRepository _userRepository;
    private readonly CurrencyApiRepository _currencyApiRepository;
    private readonly TransactionCalculator _calculator;

    public ExpenseService(GroupRepository groupRepo, ExpenseRepository expenseRepo, UserRepository userRepo, TransactionCalculator calculator, CurrencyApiRepository currencyApiRepository)

    {
        _groupRepo = groupRepo;
        _expenseRepo = expenseRepo;
        _userRepository = userRepo;
        _calculator = calculator;
        _currencyApiRepository = currencyApiRepository;

    }

    public FullExpense CreateExpense(CreateFullExpense createFullExpense, SessionData sessionData)
    {
        var loggedInUser = sessionData.UserId;
        if (!_groupRepo.IsUserInGroup(loggedInUser, createFullExpense.Expense.GroupId))
            throw new AuthenticationException();

        if (!createFullExpense.UserIdsOnExpense.Contains(loggedInUser)) throw new AuthenticationException();

        createFullExpense.Expense.UserId = loggedInUser;

        var responseExpense = _expenseRepo.CreateExpense(createFullExpense.Expense);

        var userList = AddUsersOnExpense(responseExpense.Id, createFullExpense);

        var usersOnExpense = _expenseRepo.AddUsersToExpense(userList);

        var fullExpense = new FullExpense()
        {
            Expense = responseExpense,
            UsersOnExpense = usersOnExpense!,
            LoggedInUser = sessionData.UserId
        };
        return fullExpense;
    }

    private List<CreateUserOnExpense> AddUsersOnExpense(int expenseId, CreateFullExpense createFullExpense)
    {
        // Fordeling af expense amount ud på antal brugere
        var numberOfUsers = createFullExpense.UserIdsOnExpense.Count();
        var share = createFullExpense.Expense.Amount / numberOfUsers;

        var userList = new List<CreateUserOnExpense>();
        foreach (var u in createFullExpense.UserIdsOnExpense)
        {
            if (createFullExpense.Expense.UserId == u)
            {
                var user = new CreateUserOnExpense()
                {
                    Amount = (share * numberOfUsers) - share,
                    ExpenseId = expenseId,
                    UserId = u,
                };
                userList.Add(user);
            }
            else
            {
                var user = new CreateUserOnExpense()
                {
                    Amount = share * -1,
                    ExpenseId = expenseId,
                    UserId = u,
                };
                userList.Add(user);
            }
        }

        return userList;
    }

    public FullExpense CreateSettle(int groupId, Transaction transaction, SessionData sessionData)
    {
        var loggedInUser = sessionData.UserId;
        if (!_groupRepo.IsUserInGroup(loggedInUser, groupId))
            throw new AuthenticationException();
        
        if (!transaction.PayerId.Equals(loggedInUser)) throw new AuthenticationException();

        var createExpense = new CreateExpenseDto()
        {
            UserId = loggedInUser,
            Amount = transaction.Amount * 2,
            CreatedDate = DateTime.Now,
            Description = $"{transaction.PayerName} paid {transaction.Amount} to {transaction.PayeeName}",
            GroupId = groupId,
            IsSettle = true,
        };
        
        var usersOnTransaction = new List<int> {transaction.PayerId, transaction.PayeeId};
        
        var createFullExpense = new CreateFullExpense()
        {
            Expense = createExpense,
            UserIdsOnExpense = usersOnTransaction
        };
        
        var responseExpense = _expenseRepo.CreateExpense(createExpense);

        var userList = AddUsersOnExpense(responseExpense.Id, createFullExpense);

        var usersOnExpense = _expenseRepo.AddUsersToExpense(userList);
        
        var fullExpense = new FullExpense()
        {
            Expense = responseExpense,
            UsersOnExpense = usersOnExpense!,
            LoggedInUser = sessionData.UserId
        };
        return fullExpense;
    }

    public IEnumerable<FullExpense> GetAllExpenses(int groupId, SessionData sessionData)
    {
        //Assert logged in user is authorized to access this group (api checked authentication)
        if (!_groupRepo.IsUserInGroup(sessionData.UserId, groupId)) throw new AuthenticationException();

        //Query all expenses & users on expenses from db
        IEnumerable<Expense> expenseDtos = _expenseRepo.GetAllExpenses(groupId).ToList();
        IEnumerable<GetUserOnExpense> usersOnExpenses = _expenseRepo.GetUsersOnExpenses(groupId).ToList();

        var fullExpenses = new List<FullExpense>();
        var userId = sessionData.UserId;

        //Loop through each expense
        foreach (var expense in expenseDtos)
        {
            //Create a temp list of users linked to the current expense from outer loop
            List<GetUserOnExpense> usersOnExpense = new List<GetUserOnExpense>();

            //Loop through each user on expense entry
            foreach (var uoe in usersOnExpenses)
            {
                //Add linked users to the temp list
                if (expense.Id.Equals(uoe.ExpenseId))
                {
                    usersOnExpense.Add(uoe);
                }
            }

            //Combine the expense with the list of linked users to make a full expense
            FullExpense fullExpense = new FullExpense()
            {
                Expense = expense,
                UsersOnExpense = usersOnExpense!,
                LoggedInUser = userId
            };

            //Add to the final list of full expenses
            fullExpenses.Add(fullExpense);
        }

        return fullExpenses;
    }

    public IEnumerable<BalanceDto> GetBalances(int groupId, SessionData sessionData)
    {
        //Assert logged in user is authorized to access this group (api checked authentication)
        if (!_groupRepo.IsUserInGroup(sessionData.UserId, groupId)) throw new AuthenticationException();
        return _expenseRepo.GetBalances(groupId);
    }

    public TotalBalanceDto GetTotalBalance(SessionData sessionData)
    {
        return _expenseRepo.GetTotalBalance(sessionData.UserId);
    }

    public async Task<ResponseObject> GetAvailableCurrencies()
    {
        return await _currencyApiRepository.GetCurrencyList();
    }


    public IEnumerable<Transaction> GetTotalTransactions(int groupId, SessionData sessionData)
    {
        //Assert logged in user is authorized to access this group (api checked authentication)
        if (!_groupRepo.IsUserInGroup(sessionData.UserId, groupId)) throw new AuthenticationException();

        var balances = _expenseRepo.GetBalances(groupId);

        return _calculator.CalculateTransActions(balances);
    }
    
    public IEnumerable<Transaction> GetMyDebt(int groupId, SessionData sessionData)
    {
        var transactions = GetTotalTransactions(groupId, sessionData);
        
        var myDebt = new List<Transaction>();

        foreach (var t in transactions)
        {
            if (t.PayerId == sessionData.UserId)
            {
                myDebt.Add(t);
            }
        }
        return myDebt;
    }

    public bool DeleteExpense(int expenseId, SessionData sessionData)
    {
        if (!_expenseRepo.IsUserOnExpense(expenseId, sessionData.UserId)) throw new AuthenticationException();

        return _expenseRepo.DeleteExpense(expenseId);
    }
    
}