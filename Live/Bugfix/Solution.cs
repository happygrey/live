using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

// To execute C#, please define "static void Main" on a class
// named Solution.

class Solution
{
	static async Task Main(string[] args)
	{
        using var db = new MyContext();
        Console.WriteLine();
        Console.WriteLine("Init logs:");
        var transactions = new List<TransactionsLog>();
        for (int i = 1; i < 1000; i++)
        {
            transactions.Add(new TransactionsLog()
            {
                Id = i,
                Amount = 100 + i,
                Currency = "EUR"
            });
        }
        db.TransactionsLogs.AddRange(transactions);
        db.SaveChanges();

        // read and clean all transactions logs
        var allTransactions = db.TransactionsLogs.AsAsyncEnumerable();
        db.TransactionsLogs.RemoveRange(transactions);
        db.SaveChanges();

        // some work with logs
        var sumTransaction = await GetSumAsync(allTransactions);

        Console.WriteLine(sumTransaction);
    }


	private static async Task<decimal> GetSumAsync(IAsyncEnumerable<TransactionsLog> allTransactions)
	{
		decimal sum = 0;
		await foreach (var transaction in allTransactions)
		{
			sum += transaction.Amount;
		}
		return sum;
	}
}

// Transaction model (same in DB)
[Table("transactionlogs")]
public class TransactionsLog
{
	[Column("id")]
	public int Id { get; set; }

	[Column("currency")]
	public string Currency { get; set; }

	[Column("amount")]
	public decimal Amount { get; set; }

}


public class MyContext : DbContext
{
	public DbSet<TransactionsLog> TransactionsLogs { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql("Host=/tmp/postgresql/socket; Database=coderpad; Username=coderpad");
	}
}
