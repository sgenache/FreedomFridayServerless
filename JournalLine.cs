using System;
using FreedomFridayServerless.Domain.Core;

namespace FreedomFridayServerless.Function
{
    public sealed class JournalLine : Entity
    {
        public Amount Amount { get; private set; }

        public DateTime Date { get; private set; }

        public AccountDescriptor AccountDescriptor { get; private set; }

        internal JournalLine(Amount amount, DateTime date, AccountDescriptor accountDescriptor)
        {
            AccountDescriptor = accountDescriptor ?? throw new ArgumentNullException(nameof(accountDescriptor));
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
            Date = date;
        }

        public void UpdateDescriptor(string code, string name)
        {
            AccountDescriptor = AccountDescriptor.As(AccountDescriptor.SourceId, code, name);
        }

        public string Description { get; set; }
    }

    public class JournalLineBuilder : 
        IHoldAmount, IHoldLineDate, IHoldAccountId, IHoldAccountCode, IHoldAccountName, ITransactionLineBuilder
    {
        private string _accountSourceId;
        private string _accountCode;
        private string _accountName;
        private string _decription;
        private Amount _amount;
        private DateTime _date;
        private AccountDescriptor _accountDescriptor;

        private JournalLineBuilder()
        {
        }

        public static IHoldAmount Init()
        {
            return new JournalLineBuilder();
        }

        public IHoldLineDate WithAmount(Amount amount)
        {
            _amount = amount;
            return this;
        }

        public IHoldLineDate WithNetAmount(decimal netAmount)
        {
            _amount = Amount.Allocate(netAmount);
            return this;
        }

        public IHoldLineDate WithDebitCreditAmount(decimal debitAmount, decimal creditAmount)
        {
            _amount = Amount.Allocate(debitAmount, creditAmount);
            return this;
        }

        public IHoldAccountId WithDate(DateTime date)
        {
            _date = date;
            return this;
        }

        public IHoldAccountCode WithAccountSourceId(string accountSourceId)
        {
            _accountSourceId = accountSourceId;
            return this;
        }

        public ITransactionLineBuilder WithAccount(AccountDescriptor descriptor)
        {
            _accountDescriptor = descriptor;
            return this;
        }

        public IHoldAccountName WithAccountCode(string accountCode)
        {
            _accountCode = accountCode;
            return this;
        }

        public ITransactionLineBuilder WithAccountName(string accountName)
        {
            _accountName = accountName;
            return this;
        }

        public ITransactionLineBuilder WithDescription(string decription)
        {
            _decription = decription;
            return this;
        }

        public JournalLine Build()
        {
            var account = _accountDescriptor ?? AccountDescriptor.As(_accountSourceId, _accountCode, _accountName);
            
            return new JournalLine(_amount, _date, account)
            {
                Description = _decription ?? string.Empty
            };
        }
    }

    public interface IHoldAmount
    {
        IHoldLineDate WithAmount(Amount amount);
        IHoldLineDate WithNetAmount(decimal netAmount);
        IHoldLineDate WithDebitCreditAmount(decimal debitAmount, decimal creditAmount);
    }

    public interface IHoldLineDate
    {
        IHoldAccountId WithDate(DateTime date);
    }

    public interface IHoldAccountId
    {
        IHoldAccountCode WithAccountSourceId(string accountSourceId);
        ITransactionLineBuilder WithAccount(AccountDescriptor descriptor);
    }

    public interface IHoldAccountCode
    {
        IHoldAccountName WithAccountCode(string accountCode);
    }

    public interface IHoldAccountName
    {
        ITransactionLineBuilder WithAccountName(string accountName);
    }

    public interface ITransactionLineBuilder
    {
        //ITransactionLineBuilder WithAccountType(string accountType);
        ITransactionLineBuilder WithDescription(string decription);

        JournalLine Build();
    }
}