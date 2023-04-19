using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MVC_Expense_Tracker.Data;
using MVC_Expense_Tracker.Models;

namespace MVC_Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        public DashboardController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Index()
        {
            //last seven days transactions
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            //retrieve all transactions in date range
            List<Transaction> Transactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            //Find the total Income
            int TotalIncome = Transactions.Where(a => a.Category.Type == "Income").Sum(b => b.Amount);

            //Save to the ViewBag and convert to a currency format
            ViewBag.TotalIncome = TotalIncome.ToString("C0");

            //Find the total Expense
            int TotalExpense = Transactions.Where(a => a.Category.Type == "Expense").Sum(b => b.Amount);

            //save to the ViewBag and convert to a currency format
            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            //Find the Total Balance
            int Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = Balance.ToString("C0");

            //Total transactions
            List<Transaction> TotalTransactions = await _context.Transactions.ToListAsync();
            ViewBag.TotalTransactions = TotalTransactions.Count;

            //Doughnut chart
            ViewBag.DoughnutChartData = Transactions.Where(x => x.Category.Type == "Expense")
                .GroupBy(y => y.Category.CategoryId).Select(i => new
                {
                    amount = i.Sum(y => y.Amount),
                    formattedAmount = i.Sum(y => y.Amount).ToString("C0"),
                    CategoryTitle = i.FirstOrDefault().Category.Title
                }).OrderByDescending(j => j.amount).ToList();


            //Spline chart
            //income
            List<SplineChartData> Income = Transactions.Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date).Select(k => new SplineChartData()
                {
                    Day = k.First().Date.ToString("dd-MMM"),
                    Income = k.Sum(l => l.Amount)
                }).ToList();

            //Expense
            List<SplineChartData> Expense = Transactions.Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date).Select(k => new SplineChartData()
                {
                    Day = k.First().Date.ToString("dd-MMM"),
                    Expense = k.Sum(l => l.Amount)
                }).ToList();

            //Combine Income and Expense lists by their date
            string[] LastSevenDays = Enumerable.Range(0, 7).Select(x => StartDate.AddDays(x).ToString("dd-MMM")).ToArray();

            //Add array to viewbag
            ViewBag.SplineChartData = from day in LastSevenDays
                                      join income in Income on day equals income.Day
                                      into daysIncomeJoined
                                      from income in daysIncomeJoined.DefaultIfEmpty()
                                      join expense in Expense on day equals expense.Day into expenseJoined
                                      from
                                      expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.Income,
                                          expense = expense == null ? 0 : expense.Expense
                                      };

            //Recent transactions
            ViewBag.RecentTransactions = await _context.Transactions.Include(i => i.Category)
                .OrderByDescending(j => j.Date)
                .Take(5)
                .ToListAsync();

            return View();
        }
        
    }
}
