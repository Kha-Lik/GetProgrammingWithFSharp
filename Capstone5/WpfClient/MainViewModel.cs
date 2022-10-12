using System;
using System.Collections.ObjectModel;
using Capstone5.Domain;
using PropertyChanged;

namespace Capstone5;

[AddINotifyPropertyChangedInterface]
public class MainViewModel
{
    private RatedAccount _account;

    public MainViewModel()
    {
        Owner = new Customer("isaac");
        Transactions = new ObservableCollection<Transaction>();
        LoadTransactions();
        UpdateAccount(Api.LoadAccount(Owner));
        DepositCommand = new Command<int>(
            amount =>
            {
                UpdateAccount(Api.Deposit(amount, Owner));
                WithdrawCommand.Refresh();
            }, TryParseInt);
        WithdrawCommand = new Command<int>(
            amount => UpdateAccount(Api.Withdraw(amount, Owner)),
            TryParseInt,
            () => _account.IsInCredit);
    }

    public Customer Owner { get; }
    public Command<int> DepositCommand { get; }
    public Command<int> WithdrawCommand { get; }
    public int Balance { get; private set; }
    public ObservableCollection<Transaction> Transactions { get; }

    private Tuple<bool, int> TryParseInt(object value)
    {
        var parsed = int.TryParse(value as string, out var output);
        return Tuple.Create(parsed, output);
    }

    private void UpdateAccount(RatedAccount newAccount)
    {
        _account = newAccount;
        LoadTransactions();
        Balance = (int)_account.Balance;
    }

    private void LoadTransactions()
    {
        Transactions.Clear();
        foreach (var txn in Api.LoadTransactionHistory(Owner))
            Transactions.Add(txn);
    }
}