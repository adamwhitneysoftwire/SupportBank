using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace SupportBank
{
    public class Transaction
    {
        public DateTime Date;
        public Account From;
        public Account To;
        public string Narrative;
        public double Amount;

        public Transaction(DateTime date, Account fromAccount, Account toAccount, string narrative, double amount)
        {
            Date = date;
            From = fromAccount;
            To = toAccount;
            Narrative = narrative;
            Amount = amount;
        }
    }

    public class Account
    {
        public string Name;
        public double Balance => _CalculateBalance();
        public List<Transaction> Transactions = new List<Transaction>();

        public Account(string name)
        {
            Name = name;
        }

        private double _CalculateBalance()
        {
            double balance = 0;
            foreach (Transaction transaction in Transactions)
            {
                if (transaction.To == this)
                {
                    balance += transaction.Amount;
                }
                else if (transaction.From == this)
                {
                    balance -= transaction.Amount;
                }
            }
            return balance;
        }
    }

    public class Bank
    {
        public List<Account> Accounts { get; } = new List<Account>();

        public Account GetAccount(string name)
        {
            if (Accounts.Exists(a => a.Name == name))
            {
                return Accounts.Find(a => a.Name == name);
            }
            else
            {
                Account account = new Account(name);
                Accounts.Add(account);
                return account;
            }
        }

        public void CreateTransaction(DateTime date, Account fromAccount, Account toAccount, string narrative,
            double amount)
        {
            Transaction transaction = new Transaction(date, fromAccount, toAccount, narrative, amount);
            fromAccount.Transactions.Add(transaction);
            toAccount.Transactions.Add(transaction);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Work\Training\SupportBank\InputFiles\Transactions2014.csv";
            TextFieldParser textParser = new TextFieldParser(path);
            textParser.SetDelimiters(",");
            
            // Skip the header line
            textParser.ReadLine();

            Bank bank = new Bank();

            while (!textParser.EndOfData)
            {
                string[] fields = textParser.ReadFields();
                DateTime date = Convert.ToDateTime(fields[0]);
                string from = fields[1];
                string to = fields[2];
                string narrative = fields[3];
                double amount = Convert.ToDouble(fields[4]);

                Account fromAccount = bank.GetAccount(from);
                Account toAccount = bank.GetAccount(to);
                bank.CreateTransaction(date, fromAccount, toAccount, narrative, amount);
            }

            if (args[0].ToLower() == "list")
            {
                if (args[1].ToLower() == "all")
                {
                    foreach (Account account in bank.Accounts)
                    {
                        Console.WriteLine(String.Format("{0}: {1:C}", account.Name, account.Balance));
                    }
                }
                else
                {
                    string name = args[1] + " " + args[2];
                    Account account = bank.GetAccount(name);
                    
                    Console.WriteLine("Transactions for: " + name);
                    
                    foreach (Transaction transaction in account.Transactions)
                    {
                        string direction = "IN ";
                        string qualifier = "from";
                        string otherPerson = transaction.From.Name;
                        if (transaction.From == account)
                        {
                            direction = "OUT";
                            qualifier = "to";
                            otherPerson = transaction.To.Name;
                        }
                        Console.WriteLine(String.Format("{0} {1:d} - {2:C} {3} {4} - {5}", direction, transaction.Date, transaction.Amount, qualifier, otherPerson, transaction.Narrative));
                    }
                }
            }
        }
    }
}