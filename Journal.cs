using System;
using System.Collections.Generic;
using System.Linq;
using FreedomFridayServerless.Contracts;
using FreedomFridayServerless.Domain.Core;

namespace FreedomFridayServerless.Function
{
    public class Journal : Entity 
    {
        private readonly List<JournalLine> _lines = new List<JournalLine>();

        public new Guid Id
        {
            get { return Guid.Parse(base.Id); }
            private set { base.Id = value.ToString(); }
        }

		public DateTime Date { get; private set; }

        private bool _wasPosted;

        internal Journal(Guid id, DateTime date)
        {
            Id = id;
            Date = date;
        }

        public IReadOnlyList<JournalLine> Lines { get { return _lines; } }
        public Maybe<string> Number { get; set; }
        public Maybe<string> Reference { get; set; }
        public Maybe<string> SourceType { get; set; }

        public Result Add(Amount amount, AccountDescriptor account, string description)
        {
            if (amount == null) throw new ArgumentNullException("amount");
            if (account == null) throw new ArgumentNullException("account");

            if (_wasPosted) return Result.Failure(KnownErrors.TransactionImmutable.ToString());

            _lines.Add(new JournalLine(amount, Date, account)
            {
                Description = description
            });

            return Result.Ok();
        }

        public Amount GetBalance()
        {
            return _lines
                .Select(entry => entry.Amount)
                .Sum();
        }

        public bool CanAdd()
        {
            return !_wasPosted;
        }

        private bool HasLines()
        {
            return _lines.Any();
        }

        private bool IsBalanced()
        {
            return GetBalance().IsZero();
        }

        public bool CanPost()
        {
            return HasLines() && IsBalanced();
        }

        public Result Post()
        {
            if (!IsBalanced()) return Result.Failure("TransactionNotBalanced");
            if (!HasLines()) return Result.Failure("TransactionEmpty");

            _wasPosted = true;
            return Result.Ok();
        }

        public Result<Journal> Reverse()
        {
            return JournalBuilder
                .Init()
                .WithTransactionId(Guid.NewGuid())
                .WithDate(Date)
                .WithTransactionLines(Lines
                    .Select(l => JournalLineBuilder
                        .Init()
                        .WithAmount(l.Amount.Reverse())
                        .WithDate(l.Date)
                        .WithAccount(l.AccountDescriptor)
                        .WithDescription(l.Description)))
                .WithNumber(Number)
                .WithReference(Reference)
                .WithSource(string.Format("Reversing {0}", Id).Trim())
                .Build();
        }
    }

    public class JournalBuilder:
        IHoldTransactionId, IHoldTransactionDate, IHoldTransactionLine, ITransactionBuilder
    {
        private Guid _id;
        private Maybe<string> _source;
        private Maybe<string> _number;
        private Maybe<string> _reference;
		private DateTime? _updatedDate;
        private DateTime _transactionDate;        
        private readonly List<ITransactionLineBuilder> _lineBuilders;
        private bool _shouldPost = true;

        private JournalBuilder()
        {
            _lineBuilders = new List<ITransactionLineBuilder>();
            _source = Maybe<string>.Nothing;
            _number = Maybe<string>.Nothing;
            _reference = Maybe<string>.Nothing;
        }

		public static IHoldTransactionId Init()
        {
            return new JournalBuilder();
        }

        public IHoldTransactionDate WithTransactionId(Guid id)
        {
            _id = id;
            return this;
        }

		public IHoldTransactionLine WithDate(DateTime transactionDate)
        {
            _transactionDate = transactionDate;
            return this;
        }

	    public IHoldTransactionLine WithTransactionLine(ITransactionLineBuilder lineBuilder)
        {
            _lineBuilders.Add(lineBuilder);
            return this;
        }

        public ITransactionBuilder WithTransactionLines(IEnumerable<ITransactionLineBuilder> lineBuilders)
        {
            _lineBuilders.AddRange(lineBuilders);
            return this;
        }

        public ITransactionBuilder NoMoreLines()
        {
            return this;
        }

        public ITransactionBuilder WithNumber(string number)
        {
            if (!string.IsNullOrEmpty(number)) _number = number.ToMaybe();
            return this;
        }

        public ITransactionBuilder WithNumber(Maybe<string> number)
        {
            _number = number;
            return this;
        }

        public ITransactionBuilder WithReference(string reference)
        {
            if (!string.IsNullOrEmpty(reference)) _reference = reference.ToMaybe();
            return this;
        }

        public ITransactionBuilder WithReference(Maybe<string> reference)
        {
            _reference = reference;
            return this;
        }

        public ITransactionBuilder WithSource(string source)
        {
            if (!string.IsNullOrEmpty(source)) _source = source.ToMaybe();
            return this;
        }

        public ITransactionBuilder WithSource(Maybe<string> source)
        {
            _source = source;
            return this;
        }

        public ITransactionBuilder WithDateUpdated(DateTime? dateUpdated)
		{
			_updatedDate = dateUpdated;
            return this;
		}

        public Result<Journal> Build()
        {
            var transaction = new Journal(_id, _transactionDate)
            {
                Number = _number,
                Reference = _reference,
                SourceType = _source
            };

            Result dateResult = _transactionDate != DateTime.MinValue && _transactionDate != DateTime.MaxValue 
                ? Result.Ok()
                : Result.Failure(KnownErrors.TransactionInvalidDate.ToString());

            return _lineBuilders
                .Select(builder =>
                {
                    var line = builder.Build();
                    return transaction.Add(line.Amount, line.AccountDescriptor, line.Description);
                })
                .Aggregate(dateResult, (current, result) => Result.Combine(current, result))
                .OnSuccess(() => _shouldPost ? transaction.Post() : Result.Ok())
                .ToResult(() => transaction, containsFaultyValue: true);                             
        }
    }

    public interface IHoldTransactionId
    {
        IHoldTransactionDate WithTransactionId(Guid id);
    }

    public interface IHoldTransactionDate
    {
        IHoldTransactionLine WithDate(DateTime transactionDate);
    }

    public interface IHoldTransactionLine
    {
        IHoldTransactionLine WithTransactionLine(ITransactionLineBuilder lineBuilder);
        ITransactionBuilder WithTransactionLines(IEnumerable<ITransactionLineBuilder> lineBuilders);
        ITransactionBuilder NoMoreLines();
    }

    public interface ITransactionBuilder
    {
        ITransactionBuilder WithNumber(string number);
        ITransactionBuilder WithNumber(Maybe<string> number);
        ITransactionBuilder WithReference(string reference);
        ITransactionBuilder WithReference(Maybe<string> reference);
        ITransactionBuilder WithSource(string source);
        ITransactionBuilder WithSource(Maybe<string> source);

        ITransactionBuilder WithDateUpdated(DateTime? dateUpdated);

        Result<Journal> Build();
    }

    public static class JournalExt
    {      
        public static JournalDTO ToJournalDto(this Journal t)
        {
            return new JournalDTO()
            {
                Id = t.Id.ToString(),
                Date = t.Date,
                Reference = t.Reference.HasValue ? t.Reference.Value : string.Empty,
                Number = t.Number.HasValue ? t.Number.Value : string.Empty,
                Lines = t.Lines
                    .Select(l =>
                        new JournalLineDTO()
                        {
                            AccountId = l.AccountDescriptor.SourceId,
                            AccountCode = l.AccountDescriptor.Code,
                            AccountName = l.AccountDescriptor.Name,
                            AmountDebit = l.Amount.Debit,
                            AmountCredit = l.Amount.Credit,
                            Description = l.Description ?? string.Empty
                        })
                    .ToList()
            };
        }

        public static Result<Journal> ToTransaction(this JournalDTO dto, Guid? journalGuid = null)
        {
            return JournalBuilder
                .Init()
                .WithTransactionId(journalGuid ?? Guid.Parse(dto.Id))
                .WithDate(dto.Date)
                .WithTransactionLines(dto.Lines.Select(l => JournalLineBuilder
                    .Init()
                    .WithDebitCreditAmount(l.AmountDebit, l.AmountCredit)
                    .WithDate(dto.Date)
                    .WithAccountSourceId(l.AccountId)
                    .WithAccountCode(l.AccountCode)
                    .WithAccountName(l.AccountName)
                    .WithDescription(l.Description)))
                .WithNumber(dto.Number)
                .WithReference(dto.Reference)
                .Build();
        }
    }
}