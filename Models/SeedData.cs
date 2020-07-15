using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SellingSolutions.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            // --- Seeding into Identity Context ---

            // --- KeyDerivation Pbkdf2 Properties ---
            /*
            Reference: https://andrewlock.net/exploring-the-asp-net-core-identity-passwordhasher/
            */

            const int iterationCount = 1000;
            const int keyLength = 256 / 8;
            const int saltSize = 128 / 8;

            byte[] salt = new byte[saltSize];

            using (var identityContext = new SellingSolutionsIdentityContext(serviceProvider.GetRequiredService<DbContextOptions<SellingSolutionsIdentityContext>>()))
            {
                identityContext.Users.AddRange(
                    // 'PasswordHash': https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-2.2
                    // 'SecurityStamp': https://stackoverflow.com/questions/29350167/how-to-create-a-security-stamp-value-for-asp-net-identity-iusersecuritystampsto?rq=1
                    new Areas.Identity.Data.SellingSolutionsUser
                    {
                        UserName = "admin@ecorp.com.au",
                        NormalizedUserName = "ADMIN@ECORP.COM.AU",
                        Email = "admin@ecorp.com.au",
                        NormalizedEmail = "ADMIN@ECORP.COM.AU",
                        PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: "@Welcome1",
                            salt: salt,
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: iterationCount,
                            numBytesRequested: keyLength
                        )),
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        LockoutEnabled = true,
                        Firstname = "Admin",
                        Lastname = "Admin",
                        Age = 100,
                        Street = "99 Ecorp Cct",
                        Suburb = "Golden Lake",
                        State = "WA",
                        Postcode = "6006",
                        Country = "Australia"
                    },
                    new Areas.Identity.Data.SellingSolutionsUser
                    {
                        UserName = "james@gmail.com",
                        NormalizedUserName = "JAMES@GMAIL.COM",
                        Email = "james@gmail.com",
                        NormalizedEmail = "JAMES@GMAIL.COM",
                        PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: "@Welcome1",
                            salt: salt,
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: iterationCount,
                            numBytesRequested: keyLength
                        )),
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        LockoutEnabled = true,
                        Firstname = "James",
                        Lastname = "Sander",
                        Age = 35,
                        Street = "59 Woodlake Blvd",
                        Suburb = "Durack",
                        State = "NT",
                        Postcode = "0830",
                        Country = "Australia"
                    }
                 );

                if (identityContext.Users.Any())
                {
                    return;
                }

                identityContext.SaveChanges();
            }

            // --- Seeding into Application Context ---
            // Identify the context to use
            using (var context = new SellingSolutionsContext(serviceProvider.GetRequiredService<DbContextOptions<SellingSolutionsContext>>()))
            {
                // Setup pre-loaded content to seed
                context.Product.AddRange(
                    new Product
                    {
                        Image = "https://www.scan.co.uk/images/products/2973906-a.jpg",
                        Name = "RTX 2080 Ti",
                        Category = "GPU",
                        Quantity = 20,
                        Price = 2200,
                        Seller = "john.wick@gmail.com"
                    },
                    new Product
                    {
                        Image = "https://files.pccasegear.com/images/1564627864-RTX2080-GAMING-TRIO-thb.jpg",
                        Name = "RTX 2080",
                        Category = "GPU",
                        Quantity = 10,
                        Price = 1300,
                        Seller = "adam.wick@gmail.com"
                    },
                    new Product
                    {
                        Image = "https://cdn.cplonline.com.au/media/catalog/product/cache/1/image/9df78eab33525d08d6e5fb8d27136e95/v/g/vga-asu-2060t6g.jpg",
                        Name = "RTX 2060",
                        Category = "GPU",
                        Quantity = 50,
                        Price = 550,
                        Seller = "cheap.wick@hotmail.com"
                    },
                    new Product
                    {
                        Image = "https://www.ikea.com/au/en/images/products/fenomen-unscented-block-candle-set-of-beige__0577466_PE668898_S4.JPG",
                        Name = "Smelly Candles",
                        Category = "Household",
                        Quantity = 100,
                        Price = 5,
                        Seller = "stinky.sarah@gmail.com"
                    },
                    new Product
                    {
                        Image = "https://c4d709dd302a2586107d-f8305d22c3db1fdd6f8607b49e47a10c.ssl.cf1.rackcdn.com/thumbnails/stock-images/cc1a2c6c271a0d6736f4bd9ea942cd12.png",
                        Name = "Toyota Corolla",
                        Category = "Car",
                        Quantity = 10,
                        Price = 5500,
                        Seller = "cheapskate.tim@hotmail.com"
                    },
                    new Product
                    {
                        Image = "https://banner2.kisspng.com/20180329/vzq/kisspng-ferrari-250-gto-car-dino-ferrari-275-ferrari-5abceb73122ce5.1653899115223304830745.jpg",
                        Name = "Ferrari GTO",
                        Category = "Car",
                        Quantity = 1,
                        Price = 1000000,
                        Seller = "contact@ferrari.com"
                    },
                    new Product
                    {
                        Image = "https://www.lenovo.com/medias/lenovo-laptop-thinkpad-t580-hero.png?context=bWFzdGVyfHJvb3R8Njg4MDR8aW1hZ2UvcG5nfGgyYi9oYTIvOTYxMTE4OTgxMzI3OC5wbmd8N2M5MGFkMjMwMmQ3ZTljNzhkMzBiOTJmM2M2NTJmMGM4YzYxOGFmMDVhNzc0OTk3ZTM2MjE4MTkxMWRhYWMwMg",
                        Name = "Lenovo T580",
                        Category = "Laptop",
                        Quantity = 15,
                        Price = 1200,
                        Seller = "melissa@hotmail.com"
                    },
                    new Product
                    {
                        Image = "https://www.jbhifi.com.au/FileLibrary/ProductResources/Images/219749-L-LO.jpg",
                        Name = "MX Master 2",
                        Category = "Electronics",
                        Quantity = 25,
                        Price = 120,
                        Seller = "jacob.ringo@hotmail.com"
                    },
                    new Product
                    {
                        Image = "https://images-wixmp-ed30a86b8c4ca887773594c2.wixmp.com/f/6208f106-928e-47bb-80b1-ce55b9e82b98/dmfolf-881cd05c-4f31-43b1-82ab-79c430b83dc5.jpg?token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ1cm46YXBwOjdlMGQxODg5ODIyNjQzNzNhNWYwZDQxNWVhMGQyNmUwIiwiaXNzIjoidXJuOmFwcDo3ZTBkMTg4OTgyMjY0MzczYTVmMGQ0MTVlYTBkMjZlMCIsIm9iaiI6W1t7InBhdGgiOiJcL2ZcLzYyMDhmMTA2LTkyOGUtNDdiYi04MGIxLWNlNTViOWU4MmI5OFwvZG1mb2xmLTg4MWNkMDVjLTRmMzEtNDNiMS04MmFiLTc5YzQzMGI4M2RjNS5qcGcifV1dLCJhdWQiOlsidXJuOnNlcnZpY2U6ZmlsZS5kb3dubG9hZCJdfQ.g5dCkVwAGedqdKeyPpe2uyxwoJQTV7nAeW0xmOs-HvM",
                        Name = "Harley Davidson Hog",
                        Category = "Motor Bike",
                        Quantity = 35,
                        Price = 500,
                        Seller = "hogriderdan@hotmail.com"
                    },
                    new Product
                    {
                        Image = "https://static.evanscycles.com/production/bikes/mountain-bikes/product-image/Original/norco-fluid-fs-4-2019-mountain-bike-black-EV337727-8500-1.jpg",
                        Name = "Norco Fluid FS 4",
                        Category = "Mountain Bike",
                        Quantity = 10,
                        Price = 500,
                        Seller = "dirty.steve@gmail.com"
                    }
                );

                // --- Seeding Data Content into Database ---
                if (context.Product.Any())
                {
                    return;
                }

                // Save the changes
                context.SaveChanges();
            };
        }
    }
}
