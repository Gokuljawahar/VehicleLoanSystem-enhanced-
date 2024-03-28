using Microsoft.AspNetCore.Mvc;
using VehicleLoanSystem.Data;
using VehicleLoanSystem.Models;
using Microsoft.EntityFrameworkCore;
using Scrypt;

namespace VehicleLoanSystem.Controllers.customer
{
    public class CustomerController : Controller
    {
        private readonly VehicleLoanSystemContext _context;

        public CustomerController(VehicleLoanSystemContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.HomeActive = "active";
            if (HttpContext.Session.GetInt32("userId") == null || HttpContext.Session.GetInt32("userId") == -1)
            {
                return RedirectToAction("Login", "Login");
            }
            int? id = HttpContext.Session.GetInt32("userId");

            if (_context.Loans.Any(e => e.UserId == id))
            {
                var latestLoan = _context.Loans.OrderBy(e => e.Id).LastOrDefault(e => e.UserId == id);

                if (latestLoan.LoanGrant == "ACCEPTED")
                {
                    TempData["monthlyPayment"] = Math.Round(Convert.ToDouble(latestLoan.MonthlyPayableAmount), 2);
                    TempData["loanStatus"] = latestLoan.LoanGrant;
                }
                else
                {
                    TempData["monthlyPayment"] = 0;
                    TempData["loanStatus"] = latestLoan.LoanGrant;
                }

                TempData["nextPayment"] = DateTime.Now.AddMonths(1).ToString("MMMM dddd");
            }
            else
            {
                TempData["monthlyPayment"] = 0;
                TempData["loanStatus"] = null;
                TempData["nextPayment"] = "No Active Loan";
            }

            if (_context.Payments.Any(e => e.UserId == id))
            {
                var latestPayment = _context.Payments.OrderBy(e => e.Id).LastOrDefault(e => e.UserId == id);
                TempData["nextPayment"] = latestPayment.NextPaymentDate.ToString("MMMM dd dddd");
                TempData["totalRemainingLoan"] = latestPayment.RemainingLoanAmount.ToString();
            }
            else
            {
                TempData["totalRemainingLoan"] = null;
            }

            return View();
        }

        public IActionResult Account()
        {
            ViewBag.AccountActive = "active";
            if (HttpContext.Session.GetInt32("userId") == null || HttpContext.Session.GetInt32("userId") == -1)
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccount(UserAccount userAccount)
        {
            ViewBag.AccountActive = "active";
            ScryptEncoder encoder = new ScryptEncoder();
            userAccount.User_Password = encoder.Encode(userAccount.User_Password);

            _context.Update(userAccount);
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Account Updated Successfully.";
            TempData["AlertType"] = "success";

            return RedirectToAction("Account");
        }

        public IActionResult Loan()
        {
            ViewBag.LoanActive = "active";
            var LoanPlan = _context.LoanPlans.ToList();
            var LoanType = _context.LoanTypes.ToList();

            ViewData["plan"] = LoanPlan;
            ViewData["type"] = LoanType;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestLoan(Loan Loan, IFormFile identityImageFile, IFormFile incomeImageFile, IFormFile cibilImageFile)
        {
            Console.WriteLine("RequestLoan method called.");
            ViewBag.LoanActive = "active";
            var loanPlans = _context.LoanPlans.ToList();
            var loanTypes = _context.LoanTypes.ToList();

            ViewData["plan"] = loanPlans;
            ViewData["type"] = loanTypes;

            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState is valid.");
                var loanPlan = LoanPlanExists(Loan.LoanPlanId);
                if (loanPlan == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Loan plan.");
                    return View("Loan", Loan);
                }
                if (identityImageFile != null && identityImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await identityImageFile.CopyToAsync(memoryStream);
                        byte[] imageData = memoryStream.ToArray();
                        Loan.IdentityImage = imageData; // Assign byte array directly
                    }
                }

                // Handle income image
                if (incomeImageFile != null && incomeImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await incomeImageFile.CopyToAsync(memoryStream);
                        byte[] imageData = memoryStream.ToArray();
                        Loan.IncomeImage = imageData; // Assign byte array directly
                    }
                }

                if (cibilImageFile != null && cibilImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await cibilImageFile.CopyToAsync(memoryStream);
                        byte[] imageData = memoryStream.ToArray();
                        Loan.CibilImage = imageData; // Assign byte array directly
                    }
                }

            
                double interestRate = loanPlan.Interest;
                double monthlyOverduePenaltyRate = loanPlan.MonthlyOverDuePenalty;
                double principal = Loan.LoanAmount;
                int totalMonths = loanPlan.Month;

                double monthlyInterestRate = interestRate / 100 / 12;
                double monthlyPayment = principal * (monthlyInterestRate / (1 - Math.Pow(1 + monthlyInterestRate, -totalMonths)));
                double totalPayableAmount = monthlyPayment * totalMonths;
                double monthlyPenalty = principal * (monthlyOverduePenaltyRate / 100);
                monthlyPayment = Math.Round(Math.Ceiling(monthlyPayment), 1);
                totalPayableAmount = Math.Round(Math.Ceiling(totalPayableAmount), 1);
                monthlyPenalty = Math.Round(Math.Ceiling(monthlyPenalty), 1);

                Console.WriteLine("Loan parameters calculated successfully.");
                Loan.UserId = (int)HttpContext.Session.GetInt32("userId");

                Loan.LoanDate = DateTime.Now;
                Loan.TotalPayableAmount = totalPayableAmount;
                Loan.MonthlyPayableAmount = monthlyPayment;
                Loan.MonthlyPenalty = monthlyPenalty;

                Loan.credit_score = Loan.credit_score;
                Loan.LoanGrant = "PENDING";
                Loan.RejectionReason = "NONE";

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Loans.Add(Loan);
                        await _context.SaveChangesAsync();

                        transaction.Commit();
                        Console.WriteLine("Loan saved to database successfully.");

                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError(string.Empty, "An error occurred while saving to the database.");
                        return View("Loan", Loan);
                    }
                }
            }
            else
            {
                // ModelState is invalid, print errors to console
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"ModelState error: {error.ErrorMessage}");
                    }
                }
            }


            return View("Loan", Loan);
        }




        public async Task<IActionResult> ViewLoan()
        {
            ViewBag.LoanActive = "active";
            if (HttpContext.Session.GetInt32("userId") == null || HttpContext.Session.GetInt32("userId") == -1)
            {
                return RedirectToAction("Login", "Login");
            }
            int? id = HttpContext.Session.GetInt32("userId");
            return View(await _context.Loans.Where(e => e.UserId == id).ToListAsync());
        }

        public async Task<IActionResult> DetailLoan(int? id)
        {
            if (HttpContext.Session.GetInt32("userId") == null || HttpContext.Session.GetInt32("userId") == -1)
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null)
            {
                return NotFound();
            }

            var Loan = await _context.Loans.FirstOrDefaultAsync(m => m.Id == id);
            if (Loan == null)
            {
                return NotFound();
            }

            return View(Loan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            TempData["AlertType"] = "success";
            TempData["AlertMessage"] = "Loan Request Has Been Deleted Successfully";

            var Loan = await _context.Loans.FindAsync(id);
            _context.Loans.Remove(Loan);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Payment()
        {
            if (HttpContext.Session.GetInt32("userId") == null || HttpContext.Session.GetInt32("userId") == -1)
            {
                return RedirectToAction("Login", "Login");
            }
            int? id = HttpContext.Session.GetInt32("userId");
            return View(await _context.Payments.Where(e => e.UserId == id).ToListAsync());
        }


        public IActionResult Report()
        {
            if (HttpContext.Session.GetInt32("userId") == null || HttpContext.Session.GetInt32("userId") == -1)
            {
                return RedirectToAction("Login", "Login");
            }
            int? id = HttpContext.Session.GetInt32("userId");

            TempData["AcceptedLoan"] = _context.Loans.Where(e => e.LoanDate.Year == DateTime.Now.Year && e.LoanGrant == "ACCEPTED" && e.UserId == id).Count();
            TempData["RejectedLoan"] = _context.Loans.Where(e => e.LoanDate.Year == DateTime.Now.Year && e.LoanGrant == "REJECTED" && e.UserId == id).Count();
            TempData["CoveredLoan"] = _context.Payments.Where(e => e.LoanCovered && e.UserId == id).Count();

            TempData["TotalMoneyPaid"] = _context.Payments.Where(e => e.UserId == id).Select(e => e.PayedAmount).Sum();
            TempData["TotalPenaltyMoney"] = _context.Payments.Where(e => e.UserId == id).Select(e => e.PenaltyPaymentAmount).Sum();

            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "Login");
        }

        public LoanPlan LoanPlanExists(int id)
        {
            return _context.LoanPlans.FirstOrDefault(e => e.Id == id);
        }
    }
}
