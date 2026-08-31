using System;
using System.Linq;
using System.Threading.Tasks;
using CinemaSystem.Domain.Entities;
using CinemaSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.Infrastructure.Persistence;

public static class CinemaDataSeeder
{
    public static async Task SeedAsync(CinemaDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return; // DB has been seeded
        }

        // 1. Seed Users
        // Using a dummy hash string as approved by the user
        var dummyHash = "AQAAAAIAAYagAAAAEGadmin123";
        var admin = new User("System Admin", "admin@cinema.com", dummyHash, "+1234567890");
        var staff = new User("Cinema Staff", "staff@cinema.com", dummyHash, "+1987654321");
        var customer = new User("Loyal Customer", "customer@cinema.com", dummyHash, "+1122334455");
        
        await context.Users.AddRangeAsync(admin, staff, customer);

        // 2. Seed Cinema Branch & Hall
        var branch = new CinemaBranch("City Stars Cinema", "Cairo", "Nasr City", 30.0733, 31.3458);
        var hall = new Hall(branch.Id, "IMAX Hall 1", "IMAX", 40);
        branch.AddHall(hall);

        // Seed 40 Seats (Rows A-E, Numbers 1-8)
        char[] rows = { 'A', 'B', 'C', 'D', 'E' };
        for (int rowIdx = 0; rowIdx < rows.Length; rowIdx++)
        {
            for (int colIdx = 1; colIdx <= 8; colIdx++)
            {
                var seatType = SeatType.Standard;
                // Middle seats (row C-D, cols 3-6) VIP, back row E couple
                if (rowIdx == 4) seatType = SeatType.Couple;
                else if ((rowIdx == 2 || rowIdx == 3) && (colIdx >= 3 && colIdx <= 6)) seatType = SeatType.VIP;

                var seat = new Seat(hall.Id, rows[rowIdx].ToString(), colIdx, seatType, rowIdx, colIdx - 1);
                hall.AddSeat(seat);
            }
        }

        await context.CinemaBranches.AddAsync(branch);
        await context.Halls.AddAsync(hall);
        
        // 3. Seed Genres & Movies
        var action = new Genre("Action");
        var scifi = new Genre("Sci-Fi");
        var drama = new Genre("Drama");

        await context.Genres.AddRangeAsync(action, scifi, drama);

        var inception = new Movie("Inception", "A thief who steals corporate secrets through the use of dream-sharing technology.", 148, new DateTime(2010, 7, 16, 0, 0, 0, DateTimeKind.Utc), "English", "PG-13", MovieStatus.NowShowing);
        inception.SetMediaUrls("https://trailer.url/inception", "https://poster.url/inception");
        
        var interstellar = new Movie("Interstellar", "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.", 169, new DateTime(2014, 11, 7, 0, 0, 0, DateTimeKind.Utc), "English", "PG-13", MovieStatus.NowShowing);
        interstellar.SetMediaUrls("https://trailer.url/interstellar", "https://poster.url/interstellar");

        await context.Movies.AddRangeAsync(inception, interstellar);

        // Associate genres using DbContext directly since MovieGenre relies on constructor
        await context.MovieGenres.AddRangeAsync(
            new MovieGenre(inception.Id, action.Id),
            new MovieGenre(inception.Id, scifi.Id),
            new MovieGenre(interstellar.Id, scifi.Id),
            new MovieGenre(interstellar.Id, drama.Id)
        );

        // 4. Seed Showtimes
        var today = DateTime.UtcNow.Date;
        var st1 = new Showtime(inception.Id, hall.Id, today.AddHours(14), today.AddHours(16).AddMinutes(28), "IMAX 2D", 150m);
        var st2 = new Showtime(interstellar.Id, hall.Id, today.AddHours(20), today.AddHours(22).AddMinutes(49), "IMAX 2D", 200m);
        var st3 = new Showtime(inception.Id, hall.Id, today.AddDays(1).AddHours(15), today.AddDays(1).AddHours(17).AddMinutes(28), "IMAX 2D", 150m);

        hall.AddShowtime(st1);
        hall.AddShowtime(st2);
        hall.AddShowtime(st3);

        await context.Showtimes.AddRangeAsync(st1, st2, st3);

        // 5. Food & Beverages
        var popMed = new FoodItem("Popcorn (Medium)", "Snacks", 50m);
        var popLrg = new FoodItem("Popcorn (Large)", "Snacks", 75m);
        var nachos = new FoodItem("Nachos", "Snacks", 60m);
        var softDrink = new FoodItem("Soft Drink", "Beverages", 35m);
        var combo = new FoodItem("Combo Offer", "Combos", 120m);

        await context.FoodItems.AddRangeAsync(popMed, popLrg, nachos, softDrink, combo);

        // 6. Coupons
        var coupon = new Coupon("CINEMA50", DiscountType.Percentage, 50m, DateTime.UtcNow.AddMonths(1), 100);
        await context.Coupons.AddAsync(coupon);

        await context.SaveChangesAsync();
    }
}
