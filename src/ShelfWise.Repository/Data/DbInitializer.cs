using Microsoft.EntityFrameworkCore;
using ShelfWise.Domain.Models;

namespace ShelfWise.Repository.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.Migrate();
            EnsureSchema(context);

            SeedBooks(context);

            if (!context.Users.Any())
            {
                var devUser = new ShelfWise.Domain.Models.User
                {
                    FirstName = "Dev",
                    LastName = "User"
                };
                context.Users.Add(devUser);
                context.SaveChanges();
            }
        }

        private static void SeedBooks(AppDbContext context)
        {
            var books = new List<Book>
            {
                new Book { Title = "The Pragmatic Programmer", Author = "Andrew Hunt and David Thomas", Category = Category.NonFiction, Genre = "Software Engineering", TotalCopies = 4 },
                new Book { Title = "Clean Code", Author = "Robert C. Martin", Category = Category.NonFiction, Genre = "Software Engineering", TotalCopies = 3 },
                new Book { Title = "Clean Architecture", Author = "Robert C. Martin", Category = Category.NonFiction, Genre = "Software Architecture", TotalCopies = 2 },
                new Book { Title = "Domain-Driven Design", Author = "Eric Evans", Category = Category.NonFiction, Genre = "Software Architecture", TotalCopies = 2 },
                new Book { Title = "Refactoring", Author = "Martin Fowler", Category = Category.NonFiction, Genre = "Software Engineering", TotalCopies = 3 },
                new Book { Title = "Design Patterns", Author = "Erich Gamma, Richard Helm, Ralph Johnson, and John Vlissides", Category = Category.NonFiction, Genre = "Software Architecture", TotalCopies = 2 },
                new Book { Title = "Patterns of Enterprise Application Architecture", Author = "Martin Fowler", Category = Category.NonFiction, Genre = "Software Architecture", TotalCopies = 2 },
                new Book { Title = "Working Effectively with Legacy Code", Author = "Michael Feathers", Category = Category.NonFiction, Genre = "Software Engineering", TotalCopies = 2 },
                new Book { Title = "Test Driven Development", Author = "Kent Beck", Category = Category.NonFiction, Genre = "Software Testing", TotalCopies = 3 },
                new Book { Title = "The Phoenix Project", Author = "Gene Kim, Kevin Behr, and George Spafford", Category = Category.Fiction, Genre = "Technology Fiction", TotalCopies = 4 },
                new Book { Title = "The Unicorn Project", Author = "Gene Kim", Category = Category.Fiction, Genre = "Technology Fiction", TotalCopies = 3 },
                new Book { Title = "Accelerate", Author = "Nicole Forsgren, Jez Humble, and Gene Kim", Category = Category.NonFiction, Genre = "DevOps", TotalCopies = 2 },
                new Book { Title = "Site Reliability Engineering", Author = "Betsy Beyer, Chris Jones, Jennifer Petoff, and Niall Murphy", Category = Category.NonFiction, Genre = "DevOps", TotalCopies = 2 },
                new Book { Title = "Release It!", Author = "Michael Nygard", Category = Category.NonFiction, Genre = "Systems Design", TotalCopies = 2 },
                new Book { Title = "Designing Data-Intensive Applications", Author = "Martin Kleppmann", Category = Category.NonFiction, Genre = "Data Engineering", TotalCopies = 3 },
                new Book { Title = "Database Internals", Author = "Alex Petrov", Category = Category.NonFiction, Genre = "Data Engineering", TotalCopies = 1 },
                new Book { Title = "Fundamentals of Data Engineering", Author = "Joe Reis and Matt Housley", Category = Category.NonFiction, Genre = "Data Engineering", TotalCopies = 2 },
                new Book { Title = "Building Microservices", Author = "Sam Newman", Category = Category.NonFiction, Genre = "Distributed Systems", TotalCopies = 3 },
                new Book { Title = "Monolith to Microservices", Author = "Sam Newman", Category = Category.NonFiction, Genre = "Distributed Systems", TotalCopies = 2 },
                new Book { Title = "Microservices Patterns", Author = "Chris Richardson", Category = Category.NonFiction, Genre = "Distributed Systems", TotalCopies = 2 },
                new Book { Title = "System Design Interview", Author = "Alex Xu", Category = Category.NonFiction, Genre = "Systems Design", TotalCopies = 4 },
                new Book { Title = "Grokking Algorithms", Author = "Aditya Bhargava", Category = Category.NonFiction, Genre = "Algorithms", TotalCopies = 4 },
                new Book { Title = "Introduction to Algorithms", Author = "Thomas H. Cormen, Charles E. Leiserson, Ronald L. Rivest, and Clifford Stein", Category = Category.NonFiction, Genre = "Algorithms", TotalCopies = 2 },
                new Book { Title = "Cracking the Coding Interview", Author = "Gayle Laakmann McDowell", Category = Category.NonFiction, Genre = "Interview Preparation", TotalCopies = 3 },
                new Book { Title = "Effective C#", Author = "Bill Wagner", Category = Category.NonFiction, Genre = "CSharp", TotalCopies = 2 },
                new Book { Title = "C# in Depth", Author = "Jon Skeet", Category = Category.NonFiction, Genre = "CSharp", TotalCopies = 2 },
                new Book { Title = "Concurrency in C# Cookbook", Author = "Stephen Cleary", Category = Category.NonFiction, Genre = "CSharp", TotalCopies = 1 },
                new Book { Title = "You Don't Know JS Yet", Author = "Kyle Simpson", Category = Category.NonFiction, Genre = "JavaScript", TotalCopies = 3 },
                new Book { Title = "Eloquent JavaScript", Author = "Marijn Haverbeke", Category = Category.NonFiction, Genre = "JavaScript", TotalCopies = 3 },
                new Book { Title = "Learning React", Author = "Alex Banks and Eve Porcello", Category = Category.NonFiction, Genre = "Frontend", TotalCopies = 2 },
                new Book { Title = "Designing Interfaces", Author = "Jenifer Tidwell, Charles Brewer, and Aynne Valencia", Category = Category.NonFiction, Genre = "User Experience", TotalCopies = 2 },
                new Book { Title = "Don't Make Me Think", Author = "Steve Krug", Category = Category.NonFiction, Genre = "User Experience", TotalCopies = 3 },
                new Book { Title = "The Design of Everyday Things", Author = "Don Norman", Category = Category.NonFiction, Genre = "User Experience", TotalCopies = 3 },
                new Book { Title = "Inspired", Author = "Marty Cagan", Category = Category.NonFiction, Genre = "Product Management", TotalCopies = 2 },
                new Book { Title = "Continuous Discovery Habits", Author = "Teresa Torres", Category = Category.NonFiction, Genre = "Product Management", TotalCopies = 2 },
                new Book { Title = "Lean Startup", Author = "Eric Ries", Category = Category.NonFiction, Genre = "Business", TotalCopies = 3 },
                new Book { Title = "The Mythical Man-Month", Author = "Frederick P. Brooks Jr.", Category = Category.NonFiction, Genre = "Engineering Management", TotalCopies = 2 },
                new Book { Title = "Peopleware", Author = "Tom DeMarco and Timothy Lister", Category = Category.NonFiction, Genre = "Engineering Management", TotalCopies = 2 },
                new Book { Title = "The Manager's Path", Author = "Camille Fournier", Category = Category.NonFiction, Genre = "Engineering Management", TotalCopies = 2 },
                new Book { Title = "Staff Engineer", Author = "Will Larson", Category = Category.NonFiction, Genre = "Engineering Leadership", TotalCopies = 2 },
                new Book { Title = "An Elegant Puzzle", Author = "Will Larson", Category = Category.NonFiction, Genre = "Engineering Leadership", TotalCopies = 2 },
                new Book { Title = "Artificial Intelligence: A Modern Approach", Author = "Stuart Russell and Peter Norvig", Category = Category.NonFiction, Genre = "Artificial Intelligence", TotalCopies = 2 },
                new Book { Title = "Deep Learning", Author = "Ian Goodfellow, Yoshua Bengio, and Aaron Courville", Category = Category.NonFiction, Genre = "Artificial Intelligence", TotalCopies = 1 },
                new Book { Title = "Hands-On Machine Learning", Author = "Aurelien Geron", Category = Category.NonFiction, Genre = "Machine Learning", TotalCopies = 2 },
                new Book { Title = "Designing Machine Learning Systems", Author = "Chip Huyen", Category = Category.NonFiction, Genre = "Machine Learning", TotalCopies = 2 },
                new Book { Title = "The Alignment Problem", Author = "Brian Christian", Category = Category.NonFiction, Genre = "Artificial Intelligence", TotalCopies = 2 },
                new Book { Title = "Life 3.0", Author = "Max Tegmark", Category = Category.NonFiction, Genre = "Artificial Intelligence", TotalCopies = 2 },
                new Book { Title = "Neuromancer", Author = "William Gibson", Category = Category.Fiction, Genre = "Science Fiction", TotalCopies = 3 },
                new Book { Title = "Snow Crash", Author = "Neal Stephenson", Category = Category.Fiction, Genre = "Science Fiction", TotalCopies = 3 },
                new Book { Title = "The Diamond Age", Author = "Neal Stephenson", Category = Category.Fiction, Genre = "Science Fiction", TotalCopies = 2 },
                new Book { Title = "Project Hail Mary", Author = "Andy Weir", Category = Category.Fiction, Genre = "Science Fiction", TotalCopies = 4 },
                new Book { Title = "The Martian", Author = "Andy Weir", Category = Category.Fiction, Genre = "Science Fiction", TotalCopies = 4 },
                new Book { Title = "Dune", Author = "Frank Herbert", Category = Category.Fiction, Genre = "Science Fiction", TotalCopies = 4 },
                new Book { Title = "Foundation", Author = "Isaac Asimov", Category = Category.Fiction, Genre = "Science Fiction", TotalCopies = 3 },
                new Book { Title = "I, Robot", Author = "Isaac Asimov", Category = Category.Fiction, Genre = "Science Fiction", TotalCopies = 2 },
                new Book { Title = "The Left Hand of Darkness", Author = "Ursula K. Le Guin", Category = Category.Fiction, Genre = "Science Fiction", TotalCopies = 2 },
                new Book { Title = "The Hobbit", Author = "J.R.R. Tolkien", Category = Category.Fiction, Genre = "Fantasy", TotalCopies = 4 },
                new Book { Title = "The Fellowship of the Ring", Author = "J.R.R. Tolkien", Category = Category.Fiction, Genre = "Fantasy", TotalCopies = 3 },
                new Book { Title = "A Game of Thrones", Author = "George R.R. Martin", Category = Category.Fiction, Genre = "Fantasy", TotalCopies = 3 },
                new Book { Title = "The Name of the Wind", Author = "Patrick Rothfuss", Category = Category.Fiction, Genre = "Fantasy", TotalCopies = 2 },
                new Book { Title = "The Fifth Season", Author = "N.K. Jemisin", Category = Category.Fiction, Genre = "Fantasy", TotalCopies = 2 },
                new Book { Title = "Pride and Prejudice", Author = "Jane Austen", Category = Category.Fiction, Genre = "Classic Literature", TotalCopies = 3 },
                new Book { Title = "To Kill a Mockingbird", Author = "Harper Lee", Category = Category.Fiction, Genre = "Classic Literature", TotalCopies = 3 },
                new Book { Title = "1984", Author = "George Orwell", Category = Category.Fiction, Genre = "Dystopian", TotalCopies = 3 },
                new Book { Title = "Brave New World", Author = "Aldous Huxley", Category = Category.Fiction, Genre = "Dystopian", TotalCopies = 2 }
            };

            var existingTitles = context.Books
                .Select(book => book.Title)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var missingBooks = books
                .Where(book => !existingTitles.Contains(book.Title))
                .ToList();

            if (missingBooks.Count == 0) return;

            context.Books.AddRange(missingBooks);
            context.SaveChanges();
        }

        private static void EnsureSchema(AppDbContext context)
        {
            context.Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "Books" (
                    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
                    "Title" text NOT NULL,
                    "Author" text NOT NULL,
                    "Category" integer NOT NULL,
                    "Genre" text NOT NULL,
                    "TotalCopies" integer NOT NULL,
                    CONSTRAINT "PK_Books" PRIMARY KEY ("Id")
                );

                CREATE TABLE IF NOT EXISTS "Users" (
                    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
                    "FirstName" text NOT NULL,
                    "LastName" text NOT NULL,
                    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
                );

                CREATE TABLE IF NOT EXISTS "BorrowRecords" (
                    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
                    "BookId" integer NOT NULL,
                    "UserId" integer NOT NULL,
                    "CheckedOutAt" timestamp with time zone NOT NULL,
                    "DueAt" timestamp with time zone NOT NULL,
                    "ReturnedAt" timestamp with time zone NULL,
                    CONSTRAINT "PK_BorrowRecords" PRIMARY KEY ("Id"),
                    CONSTRAINT "FK_BorrowRecords_Books_BookId" FOREIGN KEY ("BookId") REFERENCES "Books" ("Id") ON DELETE CASCADE,
                    CONSTRAINT "FK_BorrowRecords_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS "HoldRecords" (
                    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
                    "BookId" integer NOT NULL,
                    "UserId" integer NOT NULL,
                    "CreatedAt" timestamp with time zone NOT NULL,
                    "NotifiedAt" timestamp with time zone NULL,
                    "ExpiresAt" timestamp with time zone NULL,
                    CONSTRAINT "PK_HoldRecords" PRIMARY KEY ("Id"),
                    CONSTRAINT "FK_HoldRecords_Books_BookId" FOREIGN KEY ("BookId") REFERENCES "Books" ("Id") ON DELETE CASCADE,
                    CONSTRAINT "FK_HoldRecords_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
                );

                CREATE INDEX IF NOT EXISTS "IX_BorrowRecords_BookId" ON "BorrowRecords" ("BookId");
                CREATE INDEX IF NOT EXISTS "IX_BorrowRecords_UserId" ON "BorrowRecords" ("UserId");
                CREATE INDEX IF NOT EXISTS "IX_HoldRecords_BookId" ON "HoldRecords" ("BookId");
                CREATE INDEX IF NOT EXISTS "IX_HoldRecords_UserId" ON "HoldRecords" ("UserId");
                """);
        }
    }
}
